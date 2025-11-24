namespace FitnessCalculationService.Domain.Entities
{
    /// <summary>
    /// Stores history of all calculations performed by users
    /// </summary>
    public class CalculationHistory : BaseEntity
    {
        public int? UserId { get; set; }
        public string CalculationType { get; set; } = string.Empty; // "BMI", "BMR", "TDEE", etc.
        public string InputParameters { get; set; } = string.Empty; // JSON string of input parameters
        public string Result { get; set; } = string.Empty; // JSON string of calculation result
        public DateTime CalculatedAt { get; set; } = DateTime.UtcNow;
        public string? Notes { get; set; }
    }
}

