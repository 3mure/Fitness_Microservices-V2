using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProgressTrackingService.Shared;

namespace ProgressTrackingService.Feature.Waight.UpdateCurrentWeight
{
    [ApiController]
    [Route("api/v1/waight")]
    public class Waight : ControllerBase
    {
        private readonly IMediator _mediator;
        public Waight(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost("update-current-weight")]
        public async Task<IActionResult> UpdateCurrentWeight([FromBody] WeightEntryRequestDto request)
        {
            try
            {
                var command = new CreateWeightHistoryCommand(request);
                var result = await _mediator.Send(command);
                return Ok(EndpointResponse<UpdateWeightHestoryResponseDto>.SuccessResponse(result, "Weight updated successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(EndpointResponse<UpdateWeightHestoryResponseDto>.NotFoundResponse($"Error updating weight: {ex.Message}"));
            }
        }

    }
}
