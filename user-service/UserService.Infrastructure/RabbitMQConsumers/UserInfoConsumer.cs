
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;
using user_service.UserService.Application.DTOS;
using user_service.UserService.Application.Services;
using user_service.UserService.Infrastructure.IntegrationContracts;

namespace user_service.UserService.Infrastructure.RabbitMQConsumers
{
    public class UserInfoConsumer : BackgroundService
    {
        private readonly IConfiguration _iConfig;
        private readonly ILogger<UserInfoConsumer> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private IConnection _connection;
        private IChannel _channel;

        public UserInfoConsumer(IConfiguration configuration, ILogger<UserInfoConsumer> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _iConfig = configuration;        
            _logger = logger;
            _scopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {


            try
            {

                var factory = new ConnectionFactory
                {
                    HostName = _iConfig["RabbitMQ_Side_UserService:Host"]!,
                    UserName = _iConfig["RabbitMQ_Side_UserService:UserName"]!,
                    Password = _iConfig["RabbitMQ_Side_UserService:Password"]!,
                };

                _connection = await factory.CreateConnectionAsync();

                _channel = await _connection.CreateChannelAsync();

                await _channel.QueueDeclareAsync(
                       _iConfig["RabbitMQ_Side_UserService:Queue:UserInfo:QueueName"]!,
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null
                );

                await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false);

                var consumer = new AsyncEventingBasicConsumer(_channel);


                consumer.ReceivedAsync += async (model, ea) =>
                {
                    using var scope = _scopeFactory.CreateScope(); // t?o scope m?i
                    var userProfile = scope.ServiceProvider.GetRequiredService<IUserProfile>();

                    try
                    {
                        var body = ea.Body.ToArray();

                        var message = System.Text.Encoding.UTF8.GetString(body);

                        var content = JsonSerializer.Deserialize<UserInforDTO>(message);
                        if (content == null)
                        {
                            _logger.LogWarning("Received invalid message");
                            await _channel.BasicNackAsync(ea.DeliveryTag, false, true);
                            return;
                        }
                        var result = await userProfile.UserProfilHandle(new RequestUserProfile { Id = content.Id, Email = content.Email, FullName = content.Name, PhoneNumber = "0000000000" });

                        if (result)
                        {
                            _logger.LogInformation($"Create new user info {content.Email} is Successfully.");
                            await _channel.BasicAckAsync(ea.DeliveryTag, false);      //ACK  thông báo x? lý thành công và có th? xóa message 
                        }
                        else
                        {
                            _logger.LogInformation($"Create new user info {content.Email} is Faild");
                            await _channel.BasicNackAsync(ea.DeliveryTag, false, true);   // Nack l?i 
                        }

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"User Consumer Info Error: {e.Message}");
                        await _channel.BasicNackAsync(ea.DeliveryTag, false, true);       // NACK (thông báo v?i th?ng RabbitMQ là message chua x? lý thành công)
                    }

                };


                // dang ký consumer v?i queue dê nh?n message 
                await _channel.BasicConsumeAsync(
                 queue: _iConfig["RabbitMQ_Side_UserService:Queue:UserInfo:QueueName"]!,             // tên queue mà consumer l?ng nghe 
                 autoAck: false,                    // t? ack hay là th? công 
                 consumerTag: "UserInfoConsumer",            // tên d?nh danh c?a consumer
                 consumer: consumer                               // consumer object s? x? lý message khi nh?n du?c 
                );

                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(1000, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Consumer have Bug : {ex.Message}");
            }

        }
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("UserInfoConsumer stopping...");

            try
            {
                if (_channel is not null)
                    await _channel.CloseAsync(cancellationToken);

                if (_connection is not null)
                    await _connection.CloseAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while stopping UserInfoConsumer");
            }

            await base.StopAsync(cancellationToken);
        }

    }
}
