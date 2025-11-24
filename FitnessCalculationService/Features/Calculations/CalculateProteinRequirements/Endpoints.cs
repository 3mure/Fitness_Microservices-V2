using MediatR;
using Microsoft.AspNetCore.Mvc;
using FitnessCalculationService.Features.Shared;

namespace FitnessCalculationService.Features.Calculations.CalculateProteinRequirements
{
    public static class Endpoints
    {
        public static void MapCalculateProteinRequirementsEndpoint(this WebApplication app)
        {
            app.MapGet("/api/v1/calculations/protein-requirements", async (
                [FromServices] IMediator mediator,
                [FromQuery] double weightInKg,
                [FromQuery] string activityLevel,
                [FromQuery] string goal) =>
            {
                var query = new CalculateProteinRequirementsQuery(weightInKg, activityLevel, goal);
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
            .WithName("CalculateProteinRequirements")
            .WithOpenApi()
            .Produces<EndpointResponse<ProteinRequirementsResult>>(StatusCodes.Status200OK)
            .Produces<EndpointResponse<object>>(StatusCodes.Status400BadRequest);
        }
    }
}

