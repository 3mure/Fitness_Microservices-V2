namespace FitnessCalculationService.Features.Calculations.CalculateCalorieBurn
{
    public record CalorieBurnResult(
        double CaloriesBurned,
        double CaloriesPerMinute,
        string ActivityType,
        int DurationInMinutes,
        string Description
    );
}

