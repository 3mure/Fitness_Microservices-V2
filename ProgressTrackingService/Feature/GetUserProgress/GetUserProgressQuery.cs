using MediatR;
using ProgressTrackingService.Feature.GetUserProgress.DTOs;

namespace ProgressTrackingService.Feature.GetUserProgress
{
    public record   GetUserProgressQuery(int userId ) : IRequest <UserProgressDto>;


}
