namespace FitnessCalculationService.Features.Calculations.CalculateProteinRequirements
{
    public record ProteinRequirementsResult(
        double ProteinGramsPerDay,
        double ProteinGramsPerKg,
        double ProteinCaloriesPerDay,
        string Recommendation,
        Dictionary<string, string> MealDistribution
    );
}

