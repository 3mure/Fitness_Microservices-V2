namespace FitnessCalculationService.Features.Calculations.CalculateWaterIntake
{
    public record WaterIntakeResult(
        double DailyWaterIntakeLiters,
        double DailyWaterIntakeOunces,
        double DailyWaterIntakeCups,
        string Recommendation,
        Dictionary<string, double> Breakdown
    );
}

