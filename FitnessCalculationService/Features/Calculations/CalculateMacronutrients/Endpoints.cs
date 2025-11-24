using MediatR;
using Microsoft.AspNetCore.Mvc;
using FitnessCalculationService.Features.Shared;

namespace FitnessCalculationService.Features.Calculations.CalculateMacronutrients
{
    public static class Endpoints
    {
        public static void MapCalculateMacronutrientsEndpoint(this WebApplication app)
        {
            app.MapGet("/api/v1/calculations/macronutrients", async (
                [FromServices] IMediator mediator,
                [FromQuery] double totalCalories,
                [FromQuery] string goal,
                [FromQuery] string activityLevel) =>
            {
                var query = new CalculateMacronutrientsQuery(totalCalories, goal, activityLevel);
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
            .WithName("CalculateMacronutrients")
            .WithOpenApi()
            .Produces<EndpointResponse<MacronutrientResult>>(StatusCodes.Status200OK)
            .Produces<EndpointResponse<object>>(StatusCodes.Status400BadRequest);
        }
    }
}

