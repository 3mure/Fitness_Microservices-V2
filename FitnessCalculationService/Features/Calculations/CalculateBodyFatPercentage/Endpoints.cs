using MediatR;
using Microsoft.AspNetCore.Mvc;
using FitnessCalculationService.Features.Shared;

namespace FitnessCalculationService.Features.Calculations.CalculateBodyFatPercentage
{
    public static class Endpoints
    {
        public static void MapCalculateBodyFatPercentageEndpoint(this WebApplication app)
        {
            app.MapGet("/api/v1/calculations/body-fat-percentage", async (
                [FromServices] IMediator mediator,
                [FromQuery] string gender,
                [FromQuery] double weightInKg,
                [FromQuery] double heightInCm,
                [FromQuery] double waistCircumferenceInCm,
                [FromQuery] double? neckCircumferenceInCm = null,
                [FromQuery] double? hipCircumferenceInCm = null) =>
            {
                var query = new CalculateBodyFatPercentageQuery(gender, weightInKg, heightInCm, waistCircumferenceInCm, neckCircumferenceInCm, hipCircumferenceInCm);
                var result = await mediator.Send(query);

                if (!result.IsSuccess)
                {
                    return EndpointResponse<object>.ErrorResponse(
                        message: result.Message,
                        errors: new List<string> { result.Message }
                    );
                }

                return EndpointResponse<object>.SuccessResponse(
                    data: result.Data!,
                    message: result.Message
                );
            })
            .WithName("CalculateBodyFatPercentage")
            .WithOpenApi()
            .Produces<EndpointResponse<BodyFatPercentageResult>>(StatusCodes.Status200OK)
            .Produces<EndpointResponse<object>>(StatusCodes.Status400BadRequest);
        }
    }
}

