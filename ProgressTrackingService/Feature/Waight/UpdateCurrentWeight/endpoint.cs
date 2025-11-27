using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ProgressTrackingService.Feature.Waight.UpdateCurrentWeight
{
    [ApiController]
    [Route("ProgressTrackingService/v1/Feature/Waight/UpdateCurrentWeight")]
    public class Waight : ControllerBase
    {
        private readonly IMediator _mediator;
        public Waight(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost]
        public async Task<IActionResult> UpdateCurrentWeight([FromBody] CreateWeightHistoryCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

    }
}
