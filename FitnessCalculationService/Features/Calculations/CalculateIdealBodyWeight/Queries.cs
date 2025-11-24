using MediatR;
using FitnessCalculationService.Features.Shared;

namespace FitnessCalculationService.Features.Calculations.CalculateIdealBodyWeight
{
    public record CalculateIdealBodyWeightQuery(
        string Gender, // "Male" or "Female"
        double HeightInCm,
        string Formula = "Robinson" // "Robinson", "Miller", "Devine", "Hamwi"
    ) : IRequest<RequestResponse<IdealBodyWeightResult>>;
}

