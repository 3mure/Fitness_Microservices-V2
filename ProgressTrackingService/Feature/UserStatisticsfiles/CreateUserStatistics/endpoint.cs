using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProgressTrackingService.Feature.UserStatistics.CreateUserStatistics;
using ProgressTrackingService.Feature.UserStatistics.CreateUserStatistics.DTOs;
using ProgressTrackingService.Shared;

namespace ProgressTrackingService.Feature.UserStatisticsfiles.CreateUserStatistics
{
    [ApiController]
    [Route("api/v1/userstatistics")]
    public class UserStatisticsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public UserStatisticsController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost]
        public async Task<IActionResult> CreateUserStatistics([FromBody] CreateUserStatisticsCommand request)
        {
            try
            {
                var command = new CreateUserStatisticsCommand(request.userId, request.currentWeight, request.goalWeight);
                var result = await _mediator.Send(command);
                return Ok(EndpointResponse<UserStatisticsResponseDto>.SuccessResponse(result, "User statistics created successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(EndpointResponse<UserStatisticsResponseDto>.NotFoundResponse($"Error creating user statistics: {ex.Message}"));
            }
        }
    }
}
