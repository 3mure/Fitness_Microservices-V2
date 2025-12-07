using MediatR;
using ProgressTrackingService.Feature.GetUserProgress.DTOs;

namespace ProgressTrackingService.Feature.GetUserProgress
{
    public record GetUserProgressQuery(
        int userId,
        string? period = "month",  // week, month, year, all
        string? startDate = null,   
        string? endDate = null      
    ) : IRequest<UserProgressDto>;
}
