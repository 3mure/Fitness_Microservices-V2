namespace ProgressTrackingService.Feature.Waight.UpdateCurrentWeight.DTOs
{
    public class WeightEntryRequestDto
    {
        public int UserId { get; set; }
        public double Weight { get; set; }
        public double height { get; set; }
        public DateTime Date { get; set; }
        public string Time { get; set; }
        public string Notes { get; set; }

    }
}
