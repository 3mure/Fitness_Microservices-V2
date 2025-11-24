using MediatR;
using FitnessCalculationService.Features.Shared;

namespace FitnessCalculationService.Features.Calculations.CalculateProteinRequirements
{
    public record CalculateProteinRequirementsQuery(
        double WeightInKg,
        string ActivityLevel, // "Sedentary", "LightlyActive", "ModeratelyActive", "VeryActive", "ExtraActive"
        string Goal // "Maintenance", "MuscleGain", "WeightLoss", "Athlete"
    ) : IRequest<RequestResponse<ProteinRequirementsResult>>;
}

