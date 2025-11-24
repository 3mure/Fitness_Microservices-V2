namespace FitnessCalculationService.Features.Calculations.CalculateOneRepMax
{
    public record OneRepMaxResult(
        double OneRepMax,
        string Formula,
        Dictionary<string, double> RepMaxEstimates,
        string Description
    );
}

