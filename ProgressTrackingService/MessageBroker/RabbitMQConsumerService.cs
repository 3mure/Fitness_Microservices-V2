
using System.Text;

using MediatR;
using Microsoft.Extensions.Configuration;
using ProgressTrackingService.MessageBroker.Messages;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace InventoryService.MessageBroker
{
    public class RabbitMQConsumerService : IHostedService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfiguration _configuration;

        IConnection _connection;
        IChannel _channel;
        AsyncEventingBasicConsumer _consumer;

        private const string ExchangeName = "progress.exchange.events";
        private const string QueueName = "progress.tracking.weight.queue";
        private const string RoutingKey = "progress.weight.updated";

        public RabbitMQConsumerService(IServiceScopeFactory scopeFactory, IConfiguration configuration)
        {
            _scopeFactory = scopeFactory;
            _configuration = configuration;

            var rabbitMqConnectionString = _configuration.GetConnectionString("RabbitMQ") 
                ?? "amqp://guest:guest@localhost:5672/";

            var factory = new ConnectionFactory
            {
                Uri = new Uri(rabbitMqConnectionString),
            };

            _connection = factory.CreateConnectionAsync().Result;
            _channel = _connection.CreateChannelAsync().Result;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // Declare exchange - use Direct type to match existing exchange
            await _channel.ExchangeDeclareAsync(
                exchange: ExchangeName,
                type: ExchangeType.Direct,
                durable: true,
                autoDelete: false);

            // Declare queue
            await _channel.QueueDeclareAsync(
                queue: QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false);

            // Bind queue to exchange with routing key
            await _channel.QueueBindAsync(
                queue: QueueName,
                exchange: ExchangeName,
                routingKey: RoutingKey);

            _consumer = new AsyncEventingBasicConsumer(_channel);

            _consumer.ReceivedAsync += Consumer_ReceivedAync; // push mechanism

            await _channel.BasicConsumeAsync(QueueName, false, _consumer);

            Console.WriteLine($"RabbitMQ Consumer Service started. Listening on queue: {QueueName}");
        }

        private async Task Consumer_ReceivedAync(object sender, BasicDeliverEventArgs @event)
        {
            try
            {
                var message = Encoding.UTF8.GetString(@event.Body.ToArray());
                Console.WriteLine($"Received message: {message}");

                var basicMessage = GetMessage(message);
                
                if (basicMessage == null)
                {
                    Console.WriteLine("Failed to deserialize message. Acknowledging to prevent re-delivery.");
                    await _channel.BasicAckAsync(@event.DeliveryTag, false);
                    return;
                }

                await InvokeConsumer(basicMessage);

                // Acknowledge message only after successful processing
                await _channel.BasicAckAsync(@event.DeliveryTag, false);
                Console.WriteLine($"Successfully processed and acknowledged message: {basicMessage.Type}");
            }
            catch (Exception ex)
            {
                // Log the exception (you can use a logging framework here)
                Console.WriteLine($"Error processing message: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                
                // Negative acknowledgment - message will be requeued
                await _channel.BasicNackAsync(@event.DeliveryTag, false, true);
                Console.WriteLine($"Message negatively acknowledged and requeued");
            }
        }

        private async Task InvokeConsumer(BasicMessage basicMessage)
        {
            var namespaceName = "ProgressTrackingService.MessageBroker.Consumers";
            var typeName = basicMessage.Type.Replace("Message", "Consumer");

            Type type = Type.GetType($"{namespaceName}.{typeName},ProgressTrackingService");

            if (type == null)
            {
                Console.WriteLine($"Consumer type not found: {namespaceName}.{typeName}");
                return;
            }

            using var scope = _scopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            var consumer = Activator.CreateInstance(type, mediator);
            var methodInfo = type.GetMethod("ConsumeAsync");

            if (methodInfo == null)
            {
                Console.WriteLine($"Method ConsumeAsync not found on type: {type.Name}");
                return;
            }

            var result = methodInfo.Invoke(consumer, new object[] { basicMessage });

            if (result is Task task)
            {
                await task;
            }

        }

        private BasicMessage GetMessage(string message)
        {
            var basicMessage = System.Text.Json.JsonSerializer.Deserialize<BasicMessage>(message);
            var namesapce = "ProgressTrackingService.MessageBroker.Messages";
            Type type = Type.GetType($"{namesapce}.{basicMessage.Type},ProgressTrackingService");

            return System.Text.Json.JsonSerializer.Deserialize(message, type) as BasicMessage;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_consumer != null)
            {
                // Consumer will be disposed automatically
            }

            if (_channel != null && _channel.IsOpen)
            {
                await _channel.CloseAsync();
                _channel.Dispose();
            }

            if (_connection != null && _connection.IsOpen)
            {
                await _connection.CloseAsync();
                _connection.Dispose();
            }

            Console.WriteLine("RabbitMQ Consumer Service stopped.");
        }
    }
}
