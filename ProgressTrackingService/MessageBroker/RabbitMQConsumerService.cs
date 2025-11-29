
using MediatR;
using ProgressTrackingService.MessageBroker.Messages;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace ProgressTrackingService.MessageBroker
{
    public class RabbitMQConsumerService : IHostedService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        IConnection _connection;
        IChannel _channel;
        AsyncEventingBasicConsumer _consumer;

        public RabbitMQConsumerService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;

            var factory = new ConnectionFactory
            {
                Uri = new Uri("amqp://admin:admin123@rabbit:5672/"),
            };

            _connection = factory.CreateConnectionAsync().Result;
            _channel = _connection.CreateChannelAsync().Result;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
           // while (!cancellationToken.IsCancellationRequested)
            {

                _consumer = new AsyncEventingBasicConsumer(_channel);

                _consumer.ReceivedAsync += Consumer_ReceivedAync; // push mechanism

                await _channel.BasicConsumeAsync(Constants., false, _consumer);
            }
        }

        private async Task Consumer_ReceivedAync(object sender, BasicDeliverEventArgs @event)
        {
            try
            {
                var message = Encoding.UTF8.GetString(@event.Body.ToArray());

                var basicMessage = GetMessage(message);

                await InvokeConsumer(basicMessage);

                await _channel.BasicAckAsync(@event.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                // Log the exception (you can use a logging framework here)
                Console.WriteLine($"Error processing message: {ex.Message}");
            }
            finally
            {
                // Acknowledge the message regardless of success or failure to prevent re-delivery
            }
        }

        private async Task InvokeConsumer(BasicMessage basicMessage)
        {
            var namespaceName = "InventoryService.MessageBroker.Consumers";
            var typeName = basicMessage.Type.Replace("Message", "Consumer");

            Type type = Type.GetType($"{namespaceName}.{typeName},InventoryService");

            using var scope = _scopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            var consumer = Activator.CreateInstance(type, mediator);
            var methodInfo =  type.GetMethod("Consume");

            var result =  methodInfo.Invoke(consumer, new object[] { basicMessage });

            if (result is Task task)
            {
                await task;
            }

        }

        private BasicMessage GetMessage(string message)
        {
            var basicMessage = System.Text.Json.JsonSerializer.Deserialize<BasicMessage>(message);
            var namesapce = "InventoryService.MessageBroker.Messages";
            Type type = Type.GetType($"{namesapce}.{basicMessage.Type},InventoryService");

            return System.Text.Json.JsonSerializer.Deserialize(message, type) as BasicMessage;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
