using MediatR;
using ProgressTrackingService.Feature.Waight.UpdateUserstatisticsCurrentWeight;
using ProgressTrackingService.MessageBroker.Messages;

namespace ProgressTrackingService.MessageBroker.Consumers
{
    public class WeightUpdatedConsumer
    {
        private readonly IMediator _mediator;

        public WeightUpdatedConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        public async Task ConsumeAsync(BasicMessage basicMessage)
        {
            var weightUpdatedMessage = basicMessage as WeightUpdatedMessage;
            
            if (weightUpdatedMessage == null)
            {
                Console.WriteLine($"Error: Failed to cast message to WeightUpdatedMessage. Message type: {basicMessage?.Type}");
                return;
            }

            try
            {
                await _mediator.Send(new UpdateUserCurrentWeightInUserstatisticTableCommand(weightUpdatedMessage.UserId, weightUpdatedMessage.NewWeight));
                Console.WriteLine($"Successfully processed WeightUpdatedMessage for UserId: {weightUpdatedMessage.UserId}, NewWeight: {weightUpdatedMessage.NewWeight}");
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"Error processing WeightUpdatedMessage: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw; // Re-throw to let the consumer service handle acknowledgment
            }
        }
    }
}
