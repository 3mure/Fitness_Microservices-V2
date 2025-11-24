using MediatR;
using Microsoft.AspNetCore.Mvc;
using FitnessCalculationService.Features.Shared;

namespace FitnessCalculationService.Features.Calculations.CalculateOneRepMax
{
    public static class Endpoints
    {
        public static void MapCalculateOneRepMaxEndpoint(this WebApplication app)
        {
            app.MapGet("/api/v1/calculations/one-rep-max", async (
                [FromServices] IMediator mediator,
                [FromQuery] double weightLifted,
                [FromQuery] int repsCompleted,
                [FromQuery] string formula = "Epley") =>
            {
                var query = new CalculateOneRepMaxQuery(weightLifted, repsCompleted, formula);
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
            .WithName("CalculateOneRepMax")
            .WithOpenApi()
            .Produces<EndpointResponse<OneRepMaxResult>>(StatusCodes.Status200OK)
            .Produces<EndpointResponse<object>>(StatusCodes.Status400BadRequest);
        }
    }
}

