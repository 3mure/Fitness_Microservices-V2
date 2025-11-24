namespace FitnessCalculationService.Domain.Entities
{
    /// <summary>
    /// Stores user preferences for calculations (default units, formulas, etc.)
    /// </summary>
    public class UserCalculationPreferences : BaseEntity
    {
        public int? UserId { get; set; }
        public string WeightUnit { get; set; } = "kg"; // "kg" or "lbs"
        public string HeightUnit { get; set; } = "cm"; // "cm" or "inches"
        public string DistanceUnit { get; set; } = "km"; // "km" or "miles"
        public string TemperatureUnit { get; set; } = "celsius"; // "celsius" or "fahrenheit"
        
        // BMR/TDEE Preferences
        public string? PreferredBMRFormula { get; set; } // "MifflinStJeor", "HarrisBenedict", etc.
        public string? DefaultActivityLevel { get; set; } // "Sedentary", "ModeratelyActive", etc.
        
        // One-Rep Max Preferences
        public string? Preferred1RMFormula { get; set; } // "Epley", "Brzycki", etc.
        
        // Ideal Body Weight Preferences
        public string? PreferredIBWFormula { get; set; } // "Robinson", "Miller", etc.
        
        // Body Fat Preferences
        public string? PreferredBodyFatMethod { get; set; } // "Navy", "BMI", etc.
        
        // Heart Rate Preferences
        public string? PreferredHeartRateMethod { get; set; } // "Karvonen", "Percentage"
        public int? RestingHeartRate { get; set; }
        
        // Macronutrient Preferences
        public string? DefaultMacroGoal { get; set; } // "WeightLoss", "MuscleGain", etc.
        public double? CustomProteinRatio { get; set; } // Custom protein percentage
        public double? CustomCarbRatio { get; set; } // Custom carb percentage
        public double? CustomFatRatio { get; set; } // Custom fat percentage
        
        public DateTime? LastUpdated { get; set; }
    }
}

