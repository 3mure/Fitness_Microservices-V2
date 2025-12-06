using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ProgressTrackingService.Feature.GetUserProgress
{
    [ApiController]
    [Route("api/v1/[Controller]")]
    public class progressController : ControllerBase
    {
        private readonly IMediator _mediator;
        public progressController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserProgress(int userId)
        {
            var query = new GetUserProgressQuery(userId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
