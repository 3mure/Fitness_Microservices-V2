namespace ProgressTrackingService.Feature.GetUserProgress.DTOs
{
    public class UserProgressDto
    {
        public SummaryDto Summary { get; set; } = new();
        public IEnumerable<WeightHistoryDto>? WeightHistory { get; set; } 
        public IEnumerable<WorkoutHistoryDto>? WorkoutHistory { get; set; } 
        public WeeklyStatsDto? WeeklyStats { get; set; } = new();
        public IEnumerable<AchievementDto>? Achievements { get; set; } 
    }
}