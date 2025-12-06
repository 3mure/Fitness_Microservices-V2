namespace ProgressTrackingService.Feature.GetUserProgress.DTOs
{
    public class WeeklyStatsDto
    {
        public int WorkoutsThisWeek { get; set; }
        public int TotalDuration { get; set; }
        public int CaloriesBurned { get; set; }
        public int AverageDuration { get; set; }
        public ComparisonDto ComparisonToPreviousWeek { get; set; } = new();
    }

    public class ComparisonDto
    {
        public string Workouts { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public string Calories { get; set; } = string.Empty;
    }
}
