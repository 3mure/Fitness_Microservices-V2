using MediatR;
using Microsoft.AspNetCore.Mvc;
using FitnessCalculationService.Features.Shared;

namespace FitnessCalculationService.Features.Calculations.CalculateIdealBodyWeight
{
    public static class Endpoints
    {
        public static void MapCalculateIdealBodyWeightEndpoint(this WebApplication app)
        {
            app.MapGet("/api/v1/calculations/ideal-body-weight", async (
                [FromServices] IMediator mediator,
                [FromQuery] string gender,
                [FromQuery] double heightInCm,
                [FromQuery] string formula = "Robinson") =>
            {
                var query = new CalculateIdealBodyWeightQuery(gender, heightInCm, formula);
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
            .WithName("CalculateIdealBodyWeight")
            .WithOpenApi()
            .Produces<EndpointResponse<IdealBodyWeightResult>>(StatusCodes.Status200OK)
            .Produces<EndpointResponse<object>>(StatusCodes.Status400BadRequest);
        }
    }
}

