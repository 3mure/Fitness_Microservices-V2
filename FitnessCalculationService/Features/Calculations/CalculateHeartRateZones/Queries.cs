using MediatR;
using FitnessCalculationService.Features.Shared;

namespace FitnessCalculationService.Features.Calculations.CalculateHeartRateZones
{
    public record CalculateHeartRateZonesQuery(
        int Age,
        int? RestingHeartRate = null // Optional: if provided, uses Karvonen method
    ) : IRequest<RequestResponse<HeartRateZonesResult>>;
}

