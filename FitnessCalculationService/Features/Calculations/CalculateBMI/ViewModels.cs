namespace FitnessCalculationService.Features.Calculations.CalculateBMI
{
    public record BMIResult(
        double BMI,
        string Category,
        string Description
    );
}

