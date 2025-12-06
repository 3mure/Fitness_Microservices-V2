namespace ProgressTrackingService.Feature.GetUserProgress.DTOs
{
    public class WorkoutDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Duration { get; set; }   // minutes
        public int CaloriesBurned { get; set; }
    }
}
