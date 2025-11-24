namespace FitnessCalculationService.Features.Calculations.CalculateBodyFatPercentage
{
    public record BodyFatPercentageResult(
        double BodyFatPercentage,
        string Category,
        string Description,
        double LeanBodyMass,
        double FatMass
    );
}

