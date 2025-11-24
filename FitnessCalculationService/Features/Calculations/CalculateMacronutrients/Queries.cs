using MediatR;
using FitnessCalculationService.Features.Shared;

namespace FitnessCalculationService.Features.Calculations.CalculateMacronutrients
{
    public record CalculateMacronutrientsQuery(
        double TotalCalories,
        string Goal, // "WeightLoss", "WeightGain", "Maintenance", "MuscleGain"
        string ActivityLevel // "Sedentary", "LightlyActive", "ModeratelyActive", "VeryActive", "ExtraActive"
    ) : IRequest<RequestResponse<MacronutrientResult>>;
}

