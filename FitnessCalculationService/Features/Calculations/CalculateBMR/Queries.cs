using MediatR;
using FitnessCalculationService.Features.Shared;

namespace FitnessCalculationService.Features.Calculations.CalculateBMR
{
    public record CalculateBMRQuery(
        double WeightInKg,
        double HeightInCm,
        int Age,
        string Gender // "Male" or "Female"
    ) : IRequest<RequestResponse<BMRResult>>;
}

