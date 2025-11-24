using MediatR;
using FitnessCalculationService.Features.Shared;

namespace FitnessCalculationService.Features.Calculations.CalculateBMI
{
    public class CalculateBMIHandler : IRequestHandler<CalculateBMIQuery, RequestResponse<BMIResult>>
    {
        public Task<RequestResponse<BMIResult>> Handle(CalculateBMIQuery request, CancellationToken cancellationToken)
        {
            if (request.WeightInKg <= 0 || request.HeightInCm <= 0)
            {
                return Task.FromResult(RequestResponse<BMIResult>.Fail("Weight and height must be greater than zero"));
            }

            // Convert height from cm to meters
            var heightInMeters = request.HeightInCm / 100.0;
            
            // Calculate BMI: weight (kg) / height (m)²
            var bmi = request.WeightInKg / (heightInMeters * heightInMeters);
            
            // Determine category
            var (category, description) = GetBMICategory(bmi);

            var result = new BMIResult(
                Math.Round(bmi, 2),
                category,
                description
            );

            return Task.FromResult(RequestResponse<BMIResult>.Success(result, "BMI calculated successfully"));
        }

        private static (string Category, string Description) GetBMICategory(double bmi)
        {
            return bmi switch
            {
                < 18.5 => ("Underweight", "You may need to gain weight. Consult with a healthcare provider."),
                >= 18.5 and < 25 => ("Normal", "You have a healthy weight for your height."),
                >= 25 and < 30 => ("Overweight", "You may need to lose weight. Consider a balanced diet and exercise."),
                _ => ("Obese", "You may need to lose weight. Consult with a healthcare provider for guidance.")
            };
        }
    }
}

