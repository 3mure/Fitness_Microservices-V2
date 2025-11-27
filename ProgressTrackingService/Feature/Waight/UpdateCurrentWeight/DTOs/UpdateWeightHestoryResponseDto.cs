namespace ProgressTrackingService.Feature.Waight.UpdateCurrentWeight.DTOs
{
    public class UpdateWeightHestoryResponseDto
    {
        public int EntryId { get; set; }
        public double Weight { get; set; }
        public DateTime Date { get; set; }
        public double PreviousWeight { get; set; }
        public double Difference { get; set; }
        public double Bmi { get; set; }
        public double TotalWeightLost { get; set; }
        public double ProgressToGoal { get; set; }
        public double GoalRemaining { get; set; }
        public int EstimatedDaysToGoal { get; set; }
        public string Trend { get; set; }

    }
}