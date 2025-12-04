namespace ProgressTrackingService.MessageBroker.Messages
{
    public class WeightUpdatedMessage : BasicMessage
    {
        public int UserId { get; set; }
     
        public double NewWeight { get; set; }
    }
}
