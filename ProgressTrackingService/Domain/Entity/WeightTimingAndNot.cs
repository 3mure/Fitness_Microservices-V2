namespace ProgressTrackingService.Domain.Entity
{
    public class WeightTimingAndNot
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Time { get; set; }
        public string Notes { get; set; }
    }
}
