using MediatR;
using Microsoft.AspNetCore.Mvc;
using FitnessCalculationService.Features.Shared;

namespace FitnessCalculationService.Features.Calculations.CalculateCalorieBurn
{
    public static class Endpoints
    {
        public static void MapCalculateCalorieBurnEndpoint(this WebApplication app)
        {
            app.MapGet("/api/v1/calculations/calorie-burn", async (
                [FromServices] IMediator mediator,
                [FromQuery] double weightInKg,
                [FromQuery] string activityType,
                [FromQuery] int durationInMinutes,
                [FromQuery] double? intensity = null) =>
            {
                var query = new CalculateCalorieBurnQuery(weightInKg, activityType, durationInMinutes, intensity);
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
            .WithName("CalculateCalorieBurn")
            .WithOpenApi()
            .Produces<EndpointResponse<CalorieBurnResult>>(StatusCodes.Status200OK)
            .Produces<EndpointResponse<object>>(StatusCodes.Status400BadRequest);
        }
    }
}

