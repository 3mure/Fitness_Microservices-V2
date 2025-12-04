using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using ProgressTrackingService.MessageBroker.Messages;
using RabbitMQ.Client;

namespace InventoryService.MessageBroker
{
    public interface IMessageBrokerPublisher
    {
        Task PublishMessageAsync<T>(T message, string exchangeName, string routingKeyName) where T : BasicMessage;
    }

    public class MessageBrokerPublisher : IMessageBrokerPublisher, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IChannel _channel;

        public MessageBrokerPublisher(IConfiguration configuration)
        {
            var rabbitMqConnectionString = configuration.GetConnectionString("RabbitMQ") 
                ?? "amqp://guest:guest@localhost:5672/";
            
            var factory = new ConnectionFactory
            {
                Uri = new Uri(rabbitMqConnectionString),
            };

            _connection = factory.CreateConnectionAsync().Result;
            _channel = _connection.CreateChannelAsync().Result;
        }

        public async Task PublishMessageAsync<T>(T message, string exchangeName, string routingKeyName) where T : BasicMessage
        {
            // Ensure exchange exists (idempotent operation)
            // Use Direct type to match existing exchange
            await _channel.ExchangeDeclareAsync(
                exchange: exchangeName,
                type: ExchangeType.Direct,
                durable: true,
                autoDelete: false);

            message.Date = DateTime.UtcNow;
            message.Type = typeof(T).Name;

            var messageBody = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(messageBody);

            await _channel.BasicPublishAsync(
                exchange: exchangeName,
                routingKey: routingKeyName,
                body: body);

            Console.WriteLine($"Published message: {message.Type} to exchange: {exchangeName} with routing key: {routingKeyName}");
        }

        public async void Dispose()
        {
            await _channel?.CloseAsync();
            _channel?.Dispose();
            await _connection?.CloseAsync();
            _connection?.Dispose();
        }
    }
}
