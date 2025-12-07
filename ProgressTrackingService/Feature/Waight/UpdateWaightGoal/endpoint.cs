using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProgressTrackingService.Shared;

namespace ProgressTrackingService.Feature.Waight.UpdateWaightGoal
{
    [ApiController]
    [Route("api/v1/waight")]
    public class WaightController : ControllerBase 
    {
        private readonly IMediator _mediator;
        public WaightController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPut("update-goal")]
        public async Task<IActionResult> UpdateWaightGoal([FromBody] UpdateWaightGoalCommand request)
        {
            try
            {
                var command = new UpdateWaightGoalCommand(request.userId, request.newGoalWeight);
                var result = await _mediator.Send(command);
                return Ok(EndpointResponse<DTOs.UpdateGoalWaightDtoResponse>.SuccessResponse(result, "Weight goal updated successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(EndpointResponse<DTOs.UpdateGoalWaightDtoResponse>.NotFoundResponse($"Error updating weight goal: {ex.Message}"));
            }
        }
    }
}
