
using auth_services.AuthService.Application.Interfaces;
using auth_services.AuthService.Infrastructure.IntegrationContracts;
using auth_services.AuthService.Infrastructure.RabbitMQs.Producer;
using Microsoft.Identity.Client.AppConfig;
using System.Text.Json;

namespace auth_services.AuthService.Infrastructure.BackgroundServices
{
    public class OutBoxWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scope;
        private readonly RabbitMQProducer _rabbitMQ;
        private readonly ILogger<OutBoxWorker> _logger;

        public OutBoxWorker(IServiceScopeFactory serviceScopeFactory, RabbitMQProducer rabbitMQProducer, ILogger<OutBoxWorker> logger)
        {
            _scope = serviceScopeFactory;
            _rabbitMQ = rabbitMQProducer;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var sope = _scope.CreateScope();

                    var outBoxMessage = sope.ServiceProvider.GetRequiredService<IOutBoxMessage>();

                    var message = await outBoxMessage.GetMessage();
                    if (message == null) continue;
                    foreach (var content in message)
                    {
                        if (content.TypeMessage == "Notification")
                        {
                            var payload = JsonSerializer.Deserialize<RegisterNotificationMessage>(content.Payload);
                            if (payload != null)
                            {
                                await _rabbitMQ.SendMessage(payload, "notificationemail-key");
                                await outBoxMessage.MartAsProccessed(content.Id);  // dánh d?u dã processed
                            }
                            else
                            {
                                continue;
                            }
                        }

                    }

                    await Task.Delay(1000, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Outbox worker failed");
                }
            }
        }
    }
}
