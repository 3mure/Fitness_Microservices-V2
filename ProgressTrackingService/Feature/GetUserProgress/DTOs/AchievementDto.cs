namespace ProgressTrackingService.Feature.GetUserProgress.DTOs
{
    public class AchievementDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public DateTime EarnedAt { get; set; }
        public string Category { get; set; } = string.Empty;
    }
}
