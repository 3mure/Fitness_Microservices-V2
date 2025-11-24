using MediatR;
using FitnessCalculationService.Features.Shared;

namespace FitnessCalculationService.Features.Calculations.CalculateWaterIntake
{
    public record CalculateWaterIntakeQuery(
        double WeightInKg,
        string ActivityLevel, // "Sedentary", "LightlyActive", "ModeratelyActive", "VeryActive", "ExtraActive"
        int? ExerciseDurationMinutes = null
    ) : IRequest<RequestResponse<WaterIntakeResult>>;
}

