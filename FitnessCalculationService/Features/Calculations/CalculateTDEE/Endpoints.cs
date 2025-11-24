using MediatR;
using Microsoft.AspNetCore.Mvc;
using FitnessCalculationService.Features.Shared;

namespace FitnessCalculationService.Features.Calculations.CalculateTDEE
{
    public static class Endpoints
    {
        public static void MapCalculateTDEEEndpoint(this WebApplication app)
        {
            app.MapGet("/api/v1/calculations/tdee", async (
                [FromServices] IMediator mediator,
                [FromQuery] double bmr,
                [FromQuery] string activityLevel) =>
            {
                var query = new CalculateTDEEQuery(bmr, activityLevel);
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
            .WithName("CalculateTDEE")
            .WithOpenApi()
            .Produces<EndpointResponse<TDEEResult>>(StatusCodes.Status200OK)
            .Produces<EndpointResponse<object>>(StatusCodes.Status400BadRequest);
        }
    }
}
