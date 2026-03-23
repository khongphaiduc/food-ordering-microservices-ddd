
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using tracking_service.Tracking.Application.DTO;
using System.Threading.Tasks.Dataflow;
using System.Net.WebSockets;
using tracking_service.Tracking.Domain.Repository;
using System.Linq.Expressions;
using tracking_service.Tracking.Infastructrure.Models;

namespace tracking_service.Tracking.Infastructrure.Worker
{
    public class TrakingComsumer : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScope;
        private readonly IConfiguration _iconfig;
        private readonly ILogger<TrakingComsumer> _logger;
        private IConnection _connection;
        private IChannel _channel;


        public TrakingComsumer(ILogger<TrakingComsumer> logger, IConfiguration configuration, IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScope = serviceScopeFactory;
            _iconfig = configuration;
            _logger = logger;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = _iconfig["RabbitMQ:Host"]!,
                    UserName = _iconfig["RabbitMQ:Username"]!,
                    Password = _iconfig["RabbitMQ:Password"]!,
                };

                _connection = await factory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync();

                await _channel.QueueDeclareAsync(
                      _iconfig["RabbitMQ:Queue:Name"]!,
                          durable: true,
                          exclusive: false,
                          autoDelete: false,
                          arguments: null
                );

                await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false);

                var consumer = new AsyncEventingBasicConsumer(_channel);

                var batchLock = new BatchBlock<TrackingDTO>(1);  // gom message lại thành 1 batch gồm 5 message khi đủ thì đẩy ra 1 mảng message

                var actionLock = new ActionBlock<TrackingDTO[]>(async batch =>
                {
                    using (var scope = _serviceScope.CreateScope())
                    {
                        var db = scope.ServiceProvider.GetRequiredService<FoodProductsDbContext>();

                        // 1. Lấy tất cả SessionId có trong batch này
                        var sessionIdsInBatch = batch.Select(x => x.IdSession).Distinct().ToList();

                        // 2. Tìm các Session đã tồn tại trong Database
                        var existingSessionIds = db.UserSessions
                            .Where(s => sessionIdsInBatch.Contains(s.Id))
                            .Select(s => s.Id)
                            .ToList();

                        // 3. Nếu session chưa tồn tại thì tạo mới UserSession
                        var missingSessions = batch
                            .Where(x => !existingSessionIds.Contains(x.IdSession))
                            .GroupBy(x => x.IdSession)
                            .Select(g => new UserSession
                            {
                                Id = g.Key,
                                UserId = g.First().IdUser,
                                StartedAt = DateTime.UtcNow.ToLocalTime()
                            });

                        if (missingSessions.Any())
                        {
                            db.UserSessions.AddRange(missingSessions);
                        }

                        // 4. Ánh xạ các Event từ DTO sang Entity
                        var trackingEvents = batch.SelectMany(x => x.PayLoad.Select(p => new TrackingEvent
                        {
                         
                            UserId = x.IdUser,
                            SessionId = x.IdSession,
                            EventType = p.EventType.ToString(),
                            ProductId = p.IdProduct,
                            CreatedAt = p.CreatedAt
                        }));

                        db.TrackingEvents.AddRange(trackingEvents);

                       
                        try
                        {
                            await db.SaveChangesAsync();
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"Lỗi khi lưu Batch: {ex.Message}");
                        }
                    }
                });

                batchLock.LinkTo(actionLock);

                consumer.ReceivedAsync += async (model, ea) =>
                {
                    var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                    try
                    {

                        var content = JsonSerializer.Deserialize<TrackingDTO>(message, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                        await batchLock.SendAsync(content);

                        await _channel.BasicAckAsync(ea.DeliveryTag, false);
                    }
                    catch (Exception ex)
                    {

                    }
                };
                await _channel.BasicConsumeAsync(
                 queue: _iconfig["RabbitMQ:Queue:Name"]!,
                 autoAck: false,
                 consumerTag: "Tracking Consumer",
                 consumer: consumer
                );

                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(1000, stoppingToken);
                }

            }
            catch (Exception ex)
            {

                _logger.LogInformation($"Tracking Consumer Bug : {ex.Message}");
            }
        }
    }
}
