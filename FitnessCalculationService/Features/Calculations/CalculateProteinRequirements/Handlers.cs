using MediatR;
using FitnessCalculationService.Features.Shared;

namespace FitnessCalculationService.Features.Calculations.CalculateProteinRequirements
{
    public class CalculateProteinRequirementsHandler : IRequestHandler<CalculateProteinRequirementsQuery, RequestResponse<ProteinRequirementsResult>>
    {
        public Task<RequestResponse<ProteinRequirementsResult>> Handle(CalculateProteinRequirementsQuery request, CancellationToken cancellationToken)
        {
            if (request.WeightInKg <= 0)
            {
                return Task.FromResult(RequestResponse<ProteinRequirementsResult>.Fail("Weight must be greater than zero"));
            }

            var (proteinPerKg, recommendation) = GetProteinRequirements(request.Goal, request.ActivityLevel);

            if (proteinPerKg == 0)
            {
                return Task.FromResult(RequestResponse<ProteinRequirementsResult>.Fail(
                    "Invalid goal. Must be: Maintenance, MuscleGain, WeightLoss, or Athlete"));
            }

            var proteinGramsPerDay = request.WeightInKg * proteinPerKg;
            var proteinCaloriesPerDay = proteinGramsPerDay * 4; // 4 calories per gram of protein

            // Meal distribution recommendations
            var mealDistribution = new Dictionary<string, string>
            {
                { "Breakfast", $"{Math.Round(proteinGramsPerDay * 0.25, 1)}g (25%)" },
                { "Lunch", $"{Math.Round(proteinGramsPerDay * 0.30, 1)}g (30%)" },
                { "Dinner", $"{Math.Round(proteinGramsPerDay * 0.30, 1)}g (30%)" },
                { "Snacks", $"{Math.Round(proteinGramsPerDay * 0.15, 1)}g (15%)" }
            };

            var result = new ProteinRequirementsResult(
                Math.Round(proteinGramsPerDay, 1),
                proteinPerKg,
                Math.Round(proteinCaloriesPerDay, 1),
                recommendation,
                mealDistribution
            );

            return Task.FromResult(RequestResponse<ProteinRequirementsResult>.Success(result, "Protein requirements calculated successfully"));
        }

        private static (double ProteinPerKg, string Recommendation) GetProteinRequirements(string goal, string activityLevel)
        {
            return goal.ToLower() switch
            {
                "maintenance" => (1.2, "Standard protein intake for maintaining muscle mass and general health."),
                "musclegain" => (2.0, "Higher protein intake to support muscle growth and recovery. Spread intake throughout the day."),
                "weightloss" => (1.6, "Higher protein intake helps preserve muscle mass during calorie deficit and increases satiety."),
                "athlete" => (2.2, "Elite athlete protein requirements for optimal performance and recovery."),
                _ => (0, "")
            };
        }
    }
}

