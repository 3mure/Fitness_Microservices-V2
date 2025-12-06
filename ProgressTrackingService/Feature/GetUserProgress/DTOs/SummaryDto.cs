namespace ProgressTrackingService.Feature.GetUserProgress.DTOs
{
    public class SummaryDto
    {
        public double CurrentWeight { get; set; }
        public double StartWeight { get; set; }
        public double GoalWeight { get; set; }
        public double WeightChange { get; set; }
        public double ProgressPercentage { get; set; }
        public int TotalWorkouts { get; set; }
        public int TotalCaloriesBurned { get; set; }
        public int CurrentStreak { get; set; }
        public int LongestStreak { get; set; }
        public int AverageWorkoutDuration { get; set; }
    }

}
