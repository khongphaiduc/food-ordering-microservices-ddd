using notification_service.Notification.Domain.Interface;
using notification_service.Notifications.DTOS;
using notification_service.Notifications.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;

namespace notification_service.Notification.Infrastructure.Worker.EmailWorker
{
    public class EmailConsumer : BackgroundService
    {
        private readonly IConfiguration _iConfig;
        private readonly IEnumerable<INotifications> _iNotification;
        private readonly ILogger<EmailConsumer> _logger;
     
        private readonly IServiceScopeFactory _scopeFactory;
        private IChannel _channel;
        private IConnection _connection;

        public EmailConsumer(IConfiguration configuration, IEnumerable<INotifications> notifications, ILogger<EmailConsumer> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _iConfig = configuration;
            _iNotification = notifications;
            _logger = logger;
          
            _scopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = _iConfig["RabbitMQ_Side_Notification:Host"]!,
                    UserName = _iConfig["RabbitMQ_Side_Notification:UserName"]!,
                    Password = _iConfig["RabbitMQ_Side_Notification:Password"]!

                };

                _connection = await factory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync();



                await _channel.QueueDeclareAsync(
                                      queue: _iConfig["RabbitMQ_Side_Notification:Queue:Notification_Email:QueueName"]!,
                                      durable: true,
                                      exclusive: false,
                                      autoDelete: false,
                                      arguments: null);


                await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false);

                var consumer = new AsyncEventingBasicConsumer(_channel);

                consumer.ReceivedAsync += async (model, ea) =>
                {
                    try
                    {
                        var body = ea.Body.ToArray();
                        var message = System.Text.Encoding.UTF8.GetString(body);


                        var content = JsonSerializer.Deserialize<NotificationDTOS>(message);


                        var requestSendNotificaion = new RequestSendMessage(
                            to: content.Email,
                            subject: "Welcome to TRUNGDUCFOOD",
                            body: $"Hello {content.Name},\n\nThank you for registering with our service. We're excited to have you on board!\n\nBest regards,\nThe Team",
                            messageType: content.TypeService
                            );


                        if (content != null)
                        {
                            var service = _iNotification.FirstOrDefault(x => x.TypeService == "Email");
                            if (service != null)
                            {
                                var resultSendEmail = await service.SendRegisterAccount(requestSendNotificaion);



                                if (resultSendEmail)  // g?i thành công
                                {
                                    //using var scope = _scopeFactory.CreateScope(); // t?o scope m?i
                                    //var _iNotificationRecord = scope.ServiceProvider.GetRequiredService<INotificationRepository>();

                                    //var resultRecord = await _iNotificationRecord.AddNewRecordNotification(requestSendNotificaion);
                                    //_logger.LogInformation($"Record Information at Email Consumer : {resultRecord}");
                                    await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
                                }
                                else
                                {
                                    await _channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);  // Nack l?i cho RabbitMQ
                                }

                            }
                        }
                        else
                        {
                            await _channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
                        }

                    }
                    catch (Exception s)
                    {
                        _logger.LogInformation($"Email Consumer Info Error : {s.Message}");
                        await _channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
                    }

                };

                await _channel.BasicConsumeAsync(
                     queue: _iConfig["RabbitMQ_Side_Notification:Queue:Notification_Email:QueueName"]!,
                     autoAck: false,
                     consumerTag: "EmailConsumer",
                     consumer: consumer);


                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(1000, stoppingToken);
                }

            }
            catch (Exception ex)
            {

                Console.WriteLine($"Email Consumer have Bug : {ex.Message}");
            }

        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Email Consumer stopping...");

            try
            {
                if (_channel is not null)
                    await _channel.CloseAsync(cancellationToken);

                if (_connection is not null)
                    await _connection.CloseAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while stopping Email Consumer");
            }

            await base.StopAsync(cancellationToken);
        }

    }
}
