namespace ProgressTrackingService.Feature.Waight.UpdateCurrentWeight.DTOs
{
    public class BmiResponseDto
    {
        public double BMI { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}