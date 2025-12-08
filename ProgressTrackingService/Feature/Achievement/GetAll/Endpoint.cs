using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ProgressTrackingService.Feature.Achievement.GetAll
{
    [ApiController]
    [Route("api/v1/achievement")]
    public class AchievementController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AchievementController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllAchievementsQuery());
            return Ok(result);
        }
    }
}

