namespace ProgressTrackingService.MessageBroker.Messages
{
    public class BasicMessage
    {
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public string Type { get; set; } = string.Empty;
    }
}
