using MediatR;
using FitnessCalculationService.Features.Shared;

namespace FitnessCalculationService.Features.Calculations.CalculateBMR
{
    public class CalculateBMRHandler : IRequestHandler<CalculateBMRQuery, RequestResponse<BMRResult>>
    {
        public Task<RequestResponse<BMRResult>> Handle(CalculateBMRQuery request, CancellationToken cancellationToken)
        {
            if (request.WeightInKg <= 0 || request.HeightInCm <= 0 || request.Age <= 0)
            {
                return Task.FromResult(RequestResponse<BMRResult>.Fail("Weight, height, and age must be greater than zero"));
            }

            if (request.Gender?.ToLower() != "male" && request.Gender?.ToLower() != "female")
            {
                return Task.FromResult(RequestResponse<BMRResult>.Fail("Gender must be 'Male' or 'Female'"));
            }

            // Using Mifflin-St Jeor Equation (most accurate)
            double bmr;
            var isMale = request.Gender.ToLower() == "male";

            // Mifflin-St Jeor Equation:
            // Men: BMR = 10 × weight(kg) + 6.25 × height(cm) - 5 × age(years) + 5
            // Women: BMR = 10 × weight(kg) + 6.25 × height(cm) - 5 × age(years) - 161
            if (isMale)
            {
                bmr = (10 * request.WeightInKg) + (6.25 * request.HeightInCm) - (5 * request.Age) + 5;
            }
            else
            {
                bmr = (10 * request.WeightInKg) + (6.25 * request.HeightInCm) - (5 * request.Age) - 161;
            }

            var result = new BMRResult(
                Math.Round(bmr, 2),
                "Mifflin-St Jeor Equation",
                "BMR represents the number of calories your body burns at rest to maintain basic physiological functions."
            );

            return Task.FromResult(RequestResponse<BMRResult>.Success(result, "BMR calculated successfully"));
        }
    }
}

