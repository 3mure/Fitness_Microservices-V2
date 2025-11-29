using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
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

        public MessageBrokerPublisher()
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri("amqp://admin:admin123@rabbit:5672/"),
            };

            _connection = factory.CreateConnectionAsync().Result;
            _channel = _connection.CreateChannelAsync().Result;
        }

        public async Task PublishMessageAsync<T>(T message, string exchangeName, string routingKeyName) where T : BasicMessage
        {
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
