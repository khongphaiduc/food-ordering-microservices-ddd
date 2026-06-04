
using food_service.ProductService.Application.Interface;
using food_service.ProductService.Infrastructure.ProducerRabbitMQ;
using System.Text.Json;
using static System.Formats.Asn1.AsnWriter;

namespace food_service.ProductService.Infrastructure.BackgroundServices
{
    public class OutboxMessageProcessor : BackgroundService
    {
        private readonly FoodProducer _foodProducer;
        private readonly IServiceScopeFactory _ScopeService;
        private readonly ILogger<OutboxMessageProcessor> _logger;

        public OutboxMessageProcessor(FoodProducer foodProducer, IServiceScopeFactory serviceScopeFactory, ILogger<OutboxMessageProcessor> logger)
        {
            _foodProducer = foodProducer;
            _ScopeService = serviceScopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {

                try
                {
                    using var scope = _ScopeService.CreateScope();
                    var outBoxMessageTable = scope.ServiceProvider.GetRequiredService<IOutBoxPatternProduct>();

                    var Outbox = await outBoxMessageTable.GetMessageOutBoxMessage();

                    //_logger.LogInformation("Message c?n handle is :" + Outbox.Type);

                    if (Outbox == null)
                    {
                        _logger.LogInformation("OutBoxMessage is currently not exits message");
                        await Task.Delay(1000, stoppingToken);
                        continue;
                    }

                    if (Outbox.Type == "ProductCreated")
                    {


                        var content = JsonSerializer.Deserialize<ProductInternalDTO>(Outbox.PayLoad);

                        _logger.LogInformation("Message c?n handle is :" + content.Name);

                        if (content != null)
                        {
                            _logger.LogInformation(content.ToString());
                            await _foodProducer.SendMessageUpdateElasticSearch(content);   // send message to message broker 
                            await outBoxMessageTable.MarkAsProcessed(Outbox.Id);          // the  message  was  handled
                        }

                    }

                    await Task.Delay(1000, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogInformation($"Bug in backgroundService OutBoxMessage Type :{ex.Message}");
                }
                _logger.LogInformation("OutBoxBackground is running");
            }


        }
    }
}
