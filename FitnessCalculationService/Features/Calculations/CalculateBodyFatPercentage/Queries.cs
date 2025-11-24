using MediatR;
using FitnessCalculationService.Features.Shared;

namespace FitnessCalculationService.Features.Calculations.CalculateBodyFatPercentage
{
    public record CalculateBodyFatPercentageQuery(
        string Gender, // "Male" or "Female"
        double WeightInKg,
        double HeightInCm,
        double WaistCircumferenceInCm,
        double? NeckCircumferenceInCm = null, // Optional for Navy method
        double? HipCircumferenceInCm = null // Optional for Navy method (required for females)
    ) : IRequest<RequestResponse<BodyFatPercentageResult>>;
}

