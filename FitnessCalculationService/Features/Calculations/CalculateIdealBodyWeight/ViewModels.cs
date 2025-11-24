namespace FitnessCalculationService.Features.Calculations.CalculateIdealBodyWeight
{
    public record IdealBodyWeightResult(
        double IdealWeightKg,
        double IdealWeightRangeMinKg,
        double IdealWeightRangeMaxKg,
        string Formula,
        string Description
    );
}

