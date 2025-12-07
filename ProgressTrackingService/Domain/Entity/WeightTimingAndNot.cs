namespace ProgressTrackingService.Domain.Entity
{
    public class WeightTimingAndNot : BaseEntity
    {
        public int UserId { get; set; }
        public int WeightHistoryId { get; set; }
        public string Time { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        
        // Navigation property
        public WeightHistory? WeightHistory { get; set; }
    }
}
