using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProgressTrackingService.Shared;

namespace ProgressTrackingService.Feature.LogWorkout.PlaceWorkoutOrchestrator
{
    [ApiController]
    [Route("api/v1/workout")]
    public class WorkoutController : ControllerBase 
    {
       
        private readonly IMediator _mediator;

        public WorkoutController(IMediator mediator) 
        {
            
            _mediator = mediator;
        }
        [HttpPost("log")]
        public async Task<IActionResult> PlaceWorkout([FromBody] WorkoutOrchestrator command) 
        {
            try
            {
                var result = await _mediator.Send(command);
                return Ok(EndpointResponse<DTos.WorkoutLogResponseDto>.SuccessResponse(result, "Workout logged successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(EndpointResponse<DTos.WorkoutLogResponseDto>.NotFoundResponse($"Error logging workout: {ex.Message}"));
            }
        }
    }
}
