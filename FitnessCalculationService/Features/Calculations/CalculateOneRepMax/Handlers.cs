using MediatR;
using FitnessCalculationService.Features.Shared;

namespace FitnessCalculationService.Features.Calculations.CalculateOneRepMax
{
    public class CalculateOneRepMaxHandler : IRequestHandler<CalculateOneRepMaxQuery, RequestResponse<OneRepMaxResult>>
    {
        public Task<RequestResponse<OneRepMaxResult>> Handle(CalculateOneRepMaxQuery request, CancellationToken cancellationToken)
        {
            if (request.WeightLifted <= 0 || request.RepsCompleted <= 0)
            {
                return Task.FromResult(RequestResponse<OneRepMaxResult>.Fail("Weight and reps must be greater than zero"));
            }

            if (request.RepsCompleted > 10)
            {
                return Task.FromResult(RequestResponse<OneRepMaxResult>.Fail("Reps should be 10 or less for accurate 1RM calculation"));
            }

            var oneRepMax = Calculate1RM(request.WeightLifted, request.RepsCompleted, request.Formula);

            if (oneRepMax == 0)
            {
                return Task.FromResult(RequestResponse<OneRepMaxResult>.Fail(
                    "Invalid formula. Must be: Epley, Brzycki, Lander, Lombardi, Mayhew, OConner, or Wathan"));
            }

            // Calculate rep max estimates
            var repMaxes = new Dictionary<string, double>
            {
                { "1RM", Math.Round(oneRepMax, 2) },
                { "2RM", Math.Round(oneRepMax * 0.95, 2) },
                { "3RM", Math.Round(oneRepMax * 0.93, 2) },
                { "4RM", Math.Round(oneRepMax * 0.90, 2) },
                { "5RM", Math.Round(oneRepMax * 0.87, 2) },
                { "6RM", Math.Round(oneRepMax * 0.85, 2) },
                { "8RM", Math.Round(oneRepMax * 0.80, 2) },
                { "10RM", Math.Round(oneRepMax * 0.75, 2) }
            };

            var result = new OneRepMaxResult(
                Math.Round(oneRepMax, 2),
                request.Formula,
                repMaxes,
                $"Estimated 1RM using {request.Formula} formula"
            );

            return Task.FromResult(RequestResponse<OneRepMaxResult>.Success(result, "One-rep max calculated successfully"));
        }

        private static double Calculate1RM(double weight, int reps, string formula)
        {
            return formula.ToLower() switch
            {
                "epley" => weight * (1 + reps / 30.0),
                "brzycki" => weight * (36.0 / (37 - reps)),
                "lander" => (100 * weight) / (101.3 - 2.67123 * reps),
                "lombardi" => weight * Math.Pow(reps, 0.1),
                "mayhew" => (100 * weight) / (52.2 + 41.9 * Math.Exp(-0.055 * reps)),
                "oconner" => weight * (1 + reps / 40.0),
                "wathan" => (100 * weight) / (48.8 + 53.8 * Math.Exp(-0.075 * reps)),
                _ => 0
            };
        }
    }
}

