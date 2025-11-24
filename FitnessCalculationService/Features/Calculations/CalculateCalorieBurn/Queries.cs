using MediatR;
using FitnessCalculationService.Features.Shared;

namespace FitnessCalculationService.Features.Calculations.CalculateCalorieBurn
{
    public record CalculateCalorieBurnQuery(
        double WeightInKg,
        string ActivityType,
        int DurationInMinutes,
        double? Intensity = null // Optional: 0.0 to 1.0 (0.5 = moderate, 1.0 = maximum)
    ) : IRequest<RequestResponse<CalorieBurnResult>>;
}

