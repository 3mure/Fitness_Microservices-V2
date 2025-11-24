using MediatR;
using FitnessCalculationService.Features.Shared;

namespace FitnessCalculationService.Features.Calculations.CalculateTDEE
{
    public class CalculateTDEEHandler : IRequestHandler<CalculateTDEEQuery, RequestResponse<TDEEResult>>
    {
        public Task<RequestResponse<TDEEResult>> Handle(CalculateTDEEQuery request, CancellationToken cancellationToken)
        {
            if (request.BMR <= 0)
            {
                return Task.FromResult(RequestResponse<TDEEResult>.Fail("BMR must be greater than zero"));
            }

            var (multiplier, description) = GetActivityMultiplier(request.ActivityLevel);

            if (multiplier == 0)
            {
                return Task.FromResult(RequestResponse<TDEEResult>.Fail(
                    "Invalid activity level. Must be: Sedentary, LightlyActive, ModeratelyActive, VeryActive, or ExtraActive"));
            }

            var tdee = request.BMR * multiplier;

            var recommendations = new CalorieRecommendations(
                Maintenance: Math.Round(tdee, 2),
                WeightLoss: Math.Round(tdee - 500, 2),
                WeightLossAggressive: Math.Round(tdee - 1000, 2),
                WeightGain: Math.Round(tdee + 500, 2)
            );

            var result = new TDEEResult(
                Math.Round(tdee, 2),
                multiplier,
                description,
                recommendations
            );

            return Task.FromResult(RequestResponse<TDEEResult>.Success(result, "TDEE calculated successfully"));
        }

        private static (double Multiplier, string Description) GetActivityMultiplier(string activityLevel)
        {
            return activityLevel?.ToLower() switch
            {
                "sedentary" => (1.2, "Little or no exercise, desk job"),
                "lightlyactive" => (1.375, "Light exercise or sports 1-3 days/week"),
                "moderatelyactive" => (1.55, "Moderate exercise or sports 3-5 days/week"),
                "veryactive" => (1.725, "Hard exercise or sports 6-7 days/week"),
                "extraactive" => (1.9, "Very hard exercise, physical job, or training twice/day"),
                _ => (0, "")
            };
        }
    }
}

