using MediatR;
using FitnessCalculationService.Features.Shared;

namespace FitnessCalculationService.Features.Calculations.CalculateMacronutrients
{
    public class CalculateMacronutrientsHandler : IRequestHandler<CalculateMacronutrientsQuery, RequestResponse<MacronutrientResult>>
    {
        public Task<RequestResponse<MacronutrientResult>> Handle(CalculateMacronutrientsQuery request, CancellationToken cancellationToken)
        {
            if (request.TotalCalories <= 0)
            {
                return Task.FromResult(RequestResponse<MacronutrientResult>.Fail("Total calories must be greater than zero"));
            }

            var (proteinPercent, carbPercent, fatPercent, description) = GetMacroPercentages(request.Goal, request.ActivityLevel);

            if (proteinPercent == 0)
            {
                return Task.FromResult(RequestResponse<MacronutrientResult>.Fail(
                    "Invalid goal. Must be: WeightLoss, WeightGain, Maintenance, or MuscleGain"));
            }

            // Calculate macros
            // Protein: 4 calories per gram
            // Carbohydrates: 4 calories per gram
            // Fat: 9 calories per gram

            var proteinCalories = request.TotalCalories * (proteinPercent / 100.0);
            var carbCalories = request.TotalCalories * (carbPercent / 100.0);
            var fatCalories = request.TotalCalories * (fatPercent / 100.0);

            var proteinGrams = proteinCalories / 4.0;
            var carbGrams = carbCalories / 4.0;
            var fatGrams = fatCalories / 9.0;

            var protein = new ProteinMacro(
                Math.Round(proteinGrams, 2),
                Math.Round(proteinCalories, 2),
                proteinPercent
            );

            var carbohydrates = new CarbohydrateMacro(
                Math.Round(carbGrams, 2),
                Math.Round(carbCalories, 2),
                carbPercent
            );

            var fat = new FatMacro(
                Math.Round(fatGrams, 2),
                Math.Round(fatCalories, 2),
                fatPercent
            );

            var result = new MacronutrientResult(
                request.TotalCalories,
                protein,
                carbohydrates,
                fat,
                request.Goal,
                description
            );

            return Task.FromResult(RequestResponse<MacronutrientResult>.Success(result, "Macronutrients calculated successfully"));
        }

        private static (double ProteinPercent, double CarbPercent, double FatPercent, string Description) GetMacroPercentages(string goal, string activityLevel)
        {
            return goal.ToLower() switch
            {
                "weightloss" => (30.0, 40.0, 30.0, "Higher protein to preserve muscle mass during weight loss"),
                "weightgain" => (25.0, 45.0, 30.0, "Balanced macros for healthy weight gain"),
                "maintenance" => (25.0, 45.0, 30.0, "Balanced macros for maintaining current weight"),
                "musclegain" => (35.0, 40.0, 25.0, "Higher protein for muscle building and recovery"),
                _ => (0, 0, 0, "")
            };
        }
    }
}

