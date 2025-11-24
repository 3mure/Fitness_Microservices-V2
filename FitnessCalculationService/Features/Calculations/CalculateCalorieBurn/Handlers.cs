using MediatR;
using FitnessCalculationService.Features.Shared;

namespace FitnessCalculationService.Features.Calculations.CalculateCalorieBurn
{
    public class CalculateCalorieBurnHandler : IRequestHandler<CalculateCalorieBurnQuery, RequestResponse<CalorieBurnResult>>
    {
        public Task<RequestResponse<CalorieBurnResult>> Handle(CalculateCalorieBurnQuery request, CancellationToken cancellationToken)
        {
            if (request.WeightInKg <= 0 || request.DurationInMinutes <= 0)
            {
                return Task.FromResult(RequestResponse<CalorieBurnResult>.Fail("Weight and duration must be greater than zero"));
            }

            var metValue = GetMETValue(request.ActivityType, request.Intensity ?? 0.5);
            
            if (metValue == 0)
            {
                return Task.FromResult(RequestResponse<CalorieBurnResult>.Fail(
                    "Invalid activity type. Supported activities: Running, Walking, Cycling, Swimming, Weightlifting, Yoga, HIIT, Boxing, Rowing, Elliptical, StairClimbing, Dancing, Tennis, Basketball, Soccer, Golf, Skiing, Hiking, JumpingRope, Pilates"));
            }

            // Formula: Calories = MET × weight (kg) × time (hours)
            var durationInHours = request.DurationInMinutes / 60.0;
            var caloriesBurned = metValue * request.WeightInKg * durationInHours;
            var caloriesPerMinute = caloriesBurned / request.DurationInMinutes;

            var result = new CalorieBurnResult(
                Math.Round(caloriesBurned, 2),
                Math.Round(caloriesPerMinute, 2),
                request.ActivityType,
                request.DurationInMinutes,
                $"Estimated calories burned for {request.ActivityType} activity"
            );

            return Task.FromResult(RequestResponse<CalorieBurnResult>.Success(result, "Calorie burn calculated successfully"));
        }

        private static double GetMETValue(string activityType, double intensity)
        {
            // MET (Metabolic Equivalent of Task) values
            // Base MET values, adjusted by intensity
            var baseMet = activityType.ToLower() switch
            {
                "running" => 8.0,
                "walking" => 3.5,
                "cycling" => 6.0,
                "swimming" => 7.0,
                "weightlifting" => 3.0,
                "yoga" => 2.5,
                "hiit" => 10.0,
                "boxing" => 12.0,
                "rowing" => 7.0,
                "elliptical" => 5.0,
                "stairclimbing" => 8.0,
                "dancing" => 4.8,
                "tennis" => 7.0,
                "basketball" => 8.0,
                "soccer" => 7.0,
                "golf" => 4.8,
                "skiing" => 7.0,
                "hiking" => 6.0,
                "jumpingrope" => 12.0,
                "pilates" => 3.0,
                _ => 0
            };

            // Adjust MET based on intensity (0.5 = moderate, 1.0 = maximum)
            // Intensity multiplier: 0.5 = 0.8x, 0.75 = 1.0x, 1.0 = 1.3x
            var intensityMultiplier = intensity switch
            {
                <= 0.5 => 0.8,
                <= 0.75 => 1.0,
                _ => 1.3
            };

            return baseMet * intensityMultiplier;
        }
    }
}

