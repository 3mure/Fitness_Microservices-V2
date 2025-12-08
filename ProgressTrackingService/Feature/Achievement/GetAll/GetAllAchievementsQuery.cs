using MediatR;

namespace ProgressTrackingService.Feature.Achievement.GetAll
{
    public record GetAllAchievementsQuery() : IRequest<List<AchievementListDto>>;
}

