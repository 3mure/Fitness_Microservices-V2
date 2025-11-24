using MediatR;
using FitnessCalculationService.Features.Shared;

namespace FitnessCalculationService.Features.Calculations.CalculateBMI
{
    public record CalculateBMIQuery(
        double WeightInKg,
        double HeightInCm
    ) : IRequest<RequestResponse<BMIResult>>;
}

