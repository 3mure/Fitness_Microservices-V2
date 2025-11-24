using MediatR;
using FitnessCalculationService.Features.Shared;

namespace FitnessCalculationService.Features.Calculations.CalculateIdealBodyWeight
{
    public class CalculateIdealBodyWeightHandler : IRequestHandler<CalculateIdealBodyWeightQuery, RequestResponse<IdealBodyWeightResult>>
    {
        public Task<RequestResponse<IdealBodyWeightResult>> Handle(CalculateIdealBodyWeightQuery request, CancellationToken cancellationToken)
        {
            if (request.HeightInCm <= 0)
            {
                return Task.FromResult(RequestResponse<IdealBodyWeightResult>.Fail("Height must be greater than zero"));
            }

            if (request.Gender?.ToLower() != "male" && request.Gender?.ToLower() != "female")
            {
                return Task.FromResult(RequestResponse<IdealBodyWeightResult>.Fail("Gender must be 'Male' or 'Female'"));
            }

            var heightInInches = request.HeightInCm / 2.54;
            var isMale = request.Gender.ToLower() == "male";

            var idealWeight = CalculateIdealWeight(heightInInches, isMale, request.Formula);

            if (idealWeight == 0)
            {
                return Task.FromResult(RequestResponse<IdealBodyWeightResult>.Fail(
                    "Invalid formula. Must be: Robinson, Miller, Devine, or Hamwi"));
            }

            // Ideal weight range: ±10%
            var minWeight = idealWeight * 0.9;
            var maxWeight = idealWeight * 1.1;

            var result = new IdealBodyWeightResult(
                Math.Round(idealWeight, 2),
                Math.Round(minWeight, 2),
                Math.Round(maxWeight, 2),
                request.Formula,
                $"Ideal body weight calculated using {request.Formula} formula"
            );

            return Task.FromResult(RequestResponse<IdealBodyWeightResult>.Success(result, "Ideal body weight calculated successfully"));
        }

        private static double CalculateIdealWeight(double heightInInches, bool isMale, string formula)
        {
            double baseWeight;

            switch (formula.ToLower())
            {
                case "robinson":
                    if (isMale)
                        baseWeight = 52 + 1.9 * (heightInInches - 60);
                    else
                        baseWeight = 49 + 1.7 * (heightInInches - 60);
                    break;

                case "miller":
                    if (isMale)
                        baseWeight = 56.2 + 1.41 * (heightInInches - 60);
                    else
                        baseWeight = 53.1 + 1.36 * (heightInInches - 60);
                    break;

                case "devine":
                    if (isMale)
                        baseWeight = 50 + 2.3 * (heightInInches - 60);
                    else
                        baseWeight = 45.5 + 2.3 * (heightInInches - 60);
                    break;

                case "hamwi":
                    if (isMale)
                        baseWeight = 48 + 2.7 * (heightInInches - 60);
                    else
                        baseWeight = 45.5 + 2.2 * (heightInInches - 60);
                    break;

                default:
                    return 0;
            }

            // Convert from pounds to kg
            return baseWeight * 0.453592;
        }
    }
}

