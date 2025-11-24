using MediatR;
using Microsoft.AspNetCore.Mvc;
using FitnessCalculationService.Features.Shared;

namespace FitnessCalculationService.Features.Calculations.CalculateBMR
{
    public static class Endpoints
    {
        public static void MapCalculateBMREndpoint(this WebApplication app)
        {
            app.MapGet("/api/v1/calculations/bmr", async (
                [FromServices] IMediator mediator,
                [FromQuery] double weightInKg,
                [FromQuery] double heightInCm,
                [FromQuery] int age,
                [FromQuery] string gender) =>
            {
                var query = new CalculateBMRQuery(weightInKg, heightInCm, age, gender);
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
            .WithName("CalculateBMR")
            .WithOpenApi()
            .Produces<EndpointResponse<BMRResult>>(StatusCodes.Status200OK)
            .Produces<EndpointResponse<object>>(StatusCodes.Status400BadRequest);
        }
    }
}

