using MediatR;
using Microsoft.AspNetCore.Mvc;
using FitnessCalculationService.Features.Shared;

namespace FitnessCalculationService.Features.Calculations.CalculateHeartRateZones
{
    public static class Endpoints
    {
        public static void MapCalculateHeartRateZonesEndpoint(this WebApplication app)
        {
            app.MapGet("/api/v1/calculations/heart-rate-zones", async (
                [FromServices] IMediator mediator,
                [FromQuery] int age,
                [FromQuery] int? restingHeartRate = null) =>
            {
                var query = new CalculateHeartRateZonesQuery(age, restingHeartRate);
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
            .WithName("CalculateHeartRateZones")
            .WithOpenApi()
            .Produces<EndpointResponse<HeartRateZonesResult>>(StatusCodes.Status200OK)
            .Produces<EndpointResponse<object>>(StatusCodes.Status400BadRequest);
        }
    }
}

