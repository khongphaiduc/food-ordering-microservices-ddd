using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using tracking_service.Tracking.Application.DTO;
using tracking_service.Tracking.Application.Interface;

namespace tracking_service.Tracking.Infastructrure.ImplementServices
{
    public class ProducerTracking : IProducerTracking
    {
        private readonly IConfiguration _config;

        public ProducerTracking(IConfiguration configuration)
        {
            _config = configuration;
        }

        public async Task SendMessage(TrackingDTO request)
        {


            var factory = new ConnectionFactory
            {
                HostName = _config["RabbitMQ:Account"]!,
                UserName = _config["RabbitMQ:UserName"]!,
                Password = _config["RabbitMQ:Password"]!
            };


            using var _connection = await factory.CreateConnectionAsync();
            using var _channel = await _connection.CreateChannelAsync();

            await _channel.ExchangeDeclareAsync(_config["RabbitMQ:ExchangeName"]!, type: ExchangeType.Direct, durable: true);

            await _channel.QueueDeclareAsync(
               _config["RabbitMQ:Queue:Name"]!,
               durable: true,
               exclusive: false,
               autoDelete: false
           );

            await _channel.QueueBindAsync(
                 queue: _config["RabbitMQ:Queue:Name"]!,
                 exchange: _config["RabbitMQ:ExchangeName"]!,
                 routingKey: _config["RabbitMQ:Queue:RoutingKey"]!
             );

            var jons = JsonSerializer.Serialize(request);
            var body = Encoding.UTF8.GetBytes(jons);

            var properties = new BasicProperties
            {
                Persistent = true
            };

            await _channel.BasicPublishAsync(
                    exchange: _config["RabbitMQ:ExchangeName"]!,
                    routingKey: _config["RabbitMQ:Queue:RoutingKey"]!,
                    mandatory: false,
                    basicProperties: properties,
                    body: body
            );

        }
    }
}
