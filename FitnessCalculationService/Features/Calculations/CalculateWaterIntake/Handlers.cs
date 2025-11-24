using MediatR;
using FitnessCalculationService.Features.Shared;

namespace FitnessCalculationService.Features.Calculations.CalculateWaterIntake
{
    public class CalculateWaterIntakeHandler : IRequestHandler<CalculateWaterIntakeQuery, RequestResponse<WaterIntakeResult>>
    {
        public Task<RequestResponse<WaterIntakeResult>> Handle(CalculateWaterIntakeQuery request, CancellationToken cancellationToken)
        {
            if (request.WeightInKg <= 0)
            {
                return Task.FromResult(RequestResponse<WaterIntakeResult>.Fail("Weight must be greater than zero"));
            }

            // Base water intake: 30-35ml per kg of body weight
            var baseWaterIntake = request.WeightInKg * 0.033; // 33ml per kg (average)

            // Activity level multiplier
            var activityMultiplier = request.ActivityLevel?.ToLower() switch
            {
                "sedentary" => 1.0,
                "lightlyactive" => 1.1,
                "moderatelyactive" => 1.2,
                "veryactive" => 1.3,
                "extraactive" => 1.4,
                _ => 1.0
            };

            // Additional water for exercise (0.5-1 liter per hour of exercise)
            var exerciseWater = 0.0;
            if (request.ExerciseDurationMinutes.HasValue && request.ExerciseDurationMinutes.Value > 0)
            {
                var exerciseHours = request.ExerciseDurationMinutes.Value / 60.0;
                exerciseWater = exerciseHours * 0.75; // 750ml per hour average
            }

            var totalWaterIntake = (baseWaterIntake * activityMultiplier) + exerciseWater;

            // Ensure minimum intake
            totalWaterIntake = Math.Max(totalWaterIntake, 2.0); // Minimum 2 liters

            var breakdown = new Dictionary<string, double>
            {
                { "Base Intake (ml/kg)", Math.Round(baseWaterIntake * 1000, 2) },
                { "Activity Adjustment", Math.Round((baseWaterIntake * activityMultiplier) - baseWaterIntake, 2) },
                { "Exercise Supplement", Math.Round(exerciseWater, 2) }
            };

            var recommendation = totalWaterIntake switch
            {
                < 2.5 => "You may need to increase your water intake. Aim for at least 2.5 liters daily.",
                >= 2.5 and < 3.5 => "Your water intake is within a healthy range. Continue maintaining good hydration.",
                _ => "Your water intake is excellent. Make sure to spread it throughout the day."
            };

            var result = new WaterIntakeResult(
                Math.Round(totalWaterIntake, 2),
                Math.Round(totalWaterIntake * 33.814, 2), // Convert to ounces
                Math.Round(totalWaterIntake * 4.227, 1), // Convert to cups (8oz cups)
                recommendation,
                breakdown
            );

            return Task.FromResult(RequestResponse<WaterIntakeResult>.Success(result, "Water intake calculated successfully"));
        }
    }
}

