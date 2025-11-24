using MediatR;
using FitnessCalculationService.Features.Shared;

namespace FitnessCalculationService.Features.Calculations.CalculateTDEE
{
    public record CalculateTDEEQuery(
        double BMR,
        string ActivityLevel // "Sedentary", "LightlyActive", "ModeratelyActive", "VeryActive", "ExtraActive"
    ) : IRequest<RequestResponse<TDEEResult>>;
}

