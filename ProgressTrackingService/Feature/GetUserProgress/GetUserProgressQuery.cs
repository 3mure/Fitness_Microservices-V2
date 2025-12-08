using MediatR;
using ProgressTrackingService.Feature.GetUserProgress.DTOs;

namespace ProgressTrackingService.Feature.GetUserProgress
{
    public record GetUserProgressQuery(
        int userId,
        string? period = "month",  // week, month, year, all
        DateTime? startDate = null,
        DateTime? endDate = null
    ) : IRequest<UserProgressDto>;
}
