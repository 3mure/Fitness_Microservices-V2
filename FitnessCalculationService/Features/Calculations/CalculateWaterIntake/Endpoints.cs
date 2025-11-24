using MediatR;
using Microsoft.AspNetCore.Mvc;
using FitnessCalculationService.Features.Shared;

namespace FitnessCalculationService.Features.Calculations.CalculateWaterIntake
{
    public static class Endpoints
    {
        public static void MapCalculateWaterIntakeEndpoint(this WebApplication app)
        {
            app.MapGet("/api/v1/calculations/water-intake", async (
                [FromServices] IMediator mediator,
                [FromQuery] double weightInKg,
                [FromQuery] string activityLevel,
                [FromQuery] int? exerciseDurationMinutes = null) =>
            {
                var query = new CalculateWaterIntakeQuery(weightInKg, activityLevel, exerciseDurationMinutes);
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
            .WithName("CalculateWaterIntake")
            .WithOpenApi()
            .Produces<EndpointResponse<WaterIntakeResult>>(StatusCodes.Status200OK)
            .Produces<EndpointResponse<object>>(StatusCodes.Status400BadRequest);
        }
    }
}

