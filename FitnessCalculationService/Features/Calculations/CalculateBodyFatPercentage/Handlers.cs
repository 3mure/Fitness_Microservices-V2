using MediatR;
using FitnessCalculationService.Features.Shared;

namespace FitnessCalculationService.Features.Calculations.CalculateBodyFatPercentage
{
    public class CalculateBodyFatPercentageHandler : IRequestHandler<CalculateBodyFatPercentageQuery, RequestResponse<BodyFatPercentageResult>>
    {
        public Task<RequestResponse<BodyFatPercentageResult>> Handle(CalculateBodyFatPercentageQuery request, CancellationToken cancellationToken)
        {
            if (request.WeightInKg <= 0 || request.HeightInCm <= 0 || request.WaistCircumferenceInCm <= 0)
            {
                return Task.FromResult(RequestResponse<BodyFatPercentageResult>.Fail("Weight, height, and waist circumference must be greater than zero"));
            }

            if (request.Gender?.ToLower() != "male" && request.Gender?.ToLower() != "female")
            {
                return Task.FromResult(RequestResponse<BodyFatPercentageResult>.Fail("Gender must be 'Male' or 'Female'"));
            }

            double bodyFatPercentage;
            var isMale = request.Gender.ToLower() == "male";

            // Using Navy Body Fat Calculator method
            if (isMale)
            {
                if (!request.NeckCircumferenceInCm.HasValue || request.NeckCircumferenceInCm.Value <= 0)
                {
                    return Task.FromResult(RequestResponse<BodyFatPercentageResult>.Fail("Neck circumference is required for males"));
                }

                // Navy method for males
                var log10 = Math.Log10(request.WaistCircumferenceInCm - request.NeckCircumferenceInCm.Value);
                bodyFatPercentage = 495 / (1.0324 - 0.19077 * log10 + 0.15456 * Math.Log10(request.HeightInCm)) - 450;
            }
            else
            {
                if (!request.NeckCircumferenceInCm.HasValue || !request.HipCircumferenceInCm.HasValue)
                {
                    return Task.FromResult(RequestResponse<BodyFatPercentageResult>.Fail("Neck and hip circumference are required for females"));
                }

                if (request.NeckCircumferenceInCm.Value <= 0 || request.HipCircumferenceInCm.Value <= 0)
                {
                    return Task.FromResult(RequestResponse<BodyFatPercentageResult>.Fail("Neck and hip circumference must be greater than zero"));
                }

                // Navy method for females
                var log10 = Math.Log10(request.WaistCircumferenceInCm + request.HipCircumferenceInCm.Value - request.NeckCircumferenceInCm.Value);
                bodyFatPercentage = 495 / (1.29579 - 0.35004 * log10 + 0.22100 * Math.Log10(request.HeightInCm)) - 450;
            }

            // Ensure body fat percentage is within reasonable bounds
            bodyFatPercentage = Math.Max(5, Math.Min(50, bodyFatPercentage));

            var fatMass = request.WeightInKg * (bodyFatPercentage / 100.0);
            var leanBodyMass = request.WeightInKg - fatMass;

            var (category, description) = GetBodyFatCategory(bodyFatPercentage, isMale);

            var result = new BodyFatPercentageResult(
                Math.Round(bodyFatPercentage, 2),
                category,
                description,
                Math.Round(leanBodyMass, 2),
                Math.Round(fatMass, 2)
            );

            return Task.FromResult(RequestResponse<BodyFatPercentageResult>.Success(result, "Body fat percentage calculated successfully"));
        }

        private static (string Category, string Description) GetBodyFatCategory(double bodyFatPercentage, bool isMale)
        {
            if (isMale)
            {
                return bodyFatPercentage switch
                {
                    < 6 => ("Essential Fat", "Athletic level - very lean"),
                    >= 6 and < 14 => ("Athletes", "Excellent fitness level"),
                    >= 14 and < 18 => ("Fitness", "Good fitness level"),
                    >= 18 and < 25 => ("Average", "Average body fat percentage"),
                    _ => ("Obese", "Above average - consider reducing body fat")
                };
            }
            else
            {
                return bodyFatPercentage switch
                {
                    < 16 => ("Essential Fat", "Athletic level - very lean"),
                    >= 16 and < 20 => ("Athletes", "Excellent fitness level"),
                    >= 20 and < 24 => ("Fitness", "Good fitness level"),
                    >= 24 and < 32 => ("Average", "Average body fat percentage"),
                    _ => ("Obese", "Above average - consider reducing body fat")
                };
            }
        }
    }
}

