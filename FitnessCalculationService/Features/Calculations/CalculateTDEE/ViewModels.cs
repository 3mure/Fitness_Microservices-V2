namespace FitnessCalculationService.Features.Calculations.CalculateTDEE
{
    public record TDEEResult(
        double TDEE,
        double ActivityMultiplier,
        string ActivityDescription,
        CalorieRecommendations Recommendations
    );

    public record CalorieRecommendations(
        double Maintenance,
        double WeightLoss, // -500 calories
        double WeightLossAggressive, // -1000 calories
        double WeightGain // +500 calories
    );
}

