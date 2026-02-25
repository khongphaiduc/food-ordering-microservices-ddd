using RabbitMQ.Client;
using StackExchange.Redis;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;

namespace food_service.ProductService.Infastructure.ProducerRabbitMQ
{
    public class FoodProducer
    {
        private readonly IConfiguration _iconfig;
        private readonly ILogger<FoodProducer> _logger;


        public FoodProducer(IConfiguration configuration, ILogger<FoodProducer> logger)
        {
            _iconfig = configuration;
            _logger = logger;
        }


        public async Task SendMessageUpdateElasticSearch(ProductInternalDTO request)
        {

            var factory = new ConnectionFactory
            {
                HostName = _iconfig["RabbitMQ_Side_ProductService:Host"]!,
                UserName = _iconfig["RabbitMQ_Side_ProductService:UserName"]!,
                Password = _iconfig["RabbitMQ_Side_ProductService:Password"]!,
            };


            using var _connection = await factory.CreateConnectionAsync();
            using var _channel = await _connection.CreateChannelAsync();

            _logger.LogInformation($"Producer Product is Running");
            await _channel.ExchangeDeclareAsync(_iconfig["RabbitMQ_Side_ProductService:Exchange"]!, type: ExchangeType.Direct, durable: true);


            await _channel.QueueDeclareAsync(
                _iconfig["RabbitMQ_Side_ProductService:Queue:UpdateProduct:QueueName"]!,
                durable: true,
                exclusive: false,
                autoDelete: false
            );


            // map queue với exchange
            await _channel.QueueBindAsync(
                queue: _iconfig["RabbitMQ_Side_ProductService:Queue:UpdateProduct:QueueName"]!,
                exchange: _iconfig["RabbitMQ_Side_ProductService:Exchange"]!,
                routingKey: _iconfig["RabbitMQ_Side_ProductService:Queue:UpdateProduct:RoutingKey"]!
            );


            var json = JsonSerializer.Serialize(request);
            var body = Encoding.UTF8.GetBytes(json);


            var properties = new BasicProperties
            {
                Persistent = true
            };


            await _channel.BasicPublishAsync(
                    exchange: _iconfig["RabbitMQ_Side_ProductService:Exchange"]!,
                    routingKey: _iconfig["RabbitMQ_Side_ProductService:Queue:UpdateProduct:RoutingKey"]!,
                    mandatory: false,
                    basicProperties: properties,
                    body: body
            );


        }



    }
}
