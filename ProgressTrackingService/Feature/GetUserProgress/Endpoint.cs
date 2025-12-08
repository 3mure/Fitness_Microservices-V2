using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ProgressTrackingService.Feature.GetUserProgress
{
    [ApiController]
    [Route("api/v1/progress")]
    public class ProgressController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ProgressController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        /// <summary>
        /// Get user progress with optional period or custom date range.
        /// Examples:
        ///   GET /api/v1/progress/4?period=week
        ///   GET /api/v1/progress/4?startDate=2025-01-01&endDate=2025-01-31
        /// </summary>
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserProgress(
            int userId,
            [FromQuery] string? period = "month",
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var query = new GetUserProgressQuery(userId, period, startDate, endDate);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
