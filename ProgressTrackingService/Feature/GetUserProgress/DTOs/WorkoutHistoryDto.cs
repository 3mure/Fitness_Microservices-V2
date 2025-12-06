namespace ProgressTrackingService.Feature.GetUserProgress.DTOs
{
    public class WorkoutHistoryDto
    {
        public DateTime Date { get; set; }
        public int WorkoutsCompleted { get; set; }
        public int TotalDuration { get; set; }
        public int CaloriesBurned { get; set; }
        public List<WorkoutDto> Workouts { get; set; } = new();
    }

}
