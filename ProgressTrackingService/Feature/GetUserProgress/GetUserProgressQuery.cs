using MediatR;
using ProgressTrackingService.Feature.GetUserProgress.DTOs;

namespace ProgressTrackingService.Feature.GetUserProgress
{
    public record GetUserProgressQuery(
        int userId,
        string? period = "month",  // week, month, year, all
        string? startDate = null,   // ISO 8601 format
        string? endDate = null      // ISO 8601 format
    ) : IRequest<UserProgressDto>;
}
