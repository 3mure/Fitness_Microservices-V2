using MediatR;
using Microsoft.AspNetCore.Mvc;
using FitnessCalculationService.Features.Shared;

namespace FitnessCalculationService.Features.Calculations.CalculateBMI
{
    public static class Endpoints
    {
        public static void MapCalculateBMIEndpoint(this WebApplication app)
        {
            app.MapGet("/api/v1/calculations/bmi", async (
                [FromServices] IMediator mediator,
                [FromQuery] double weightInKg,
                [FromQuery] double heightInCm) =>
            {
                var query = new CalculateBMIQuery(weightInKg, heightInCm);
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
            .WithName("CalculateBMI")
            .WithOpenApi()
            .Produces<EndpointResponse<BMIResult>>(StatusCodes.Status200OK)
            .Produces<EndpointResponse<object>>(StatusCodes.Status400BadRequest);
        }
    }
}

