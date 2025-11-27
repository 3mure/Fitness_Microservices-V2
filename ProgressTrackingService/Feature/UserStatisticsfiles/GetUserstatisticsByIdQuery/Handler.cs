using MediatR;
using ProgressTrackingService.Domain.Entity;
using ProgressTrackingService.Domain.Interfaces;

namespace ProgressTrackingService.Feature.UserStatisticsfiles.GetUserstatisticsByIdQuery
{
    public class Handler : IRequestHandler<GetUserWeightStatisticsQuery, GetWeightstatisticsrelatedToUser>
    {
        private readonly IGenericRepository<Domain.Entity.UserStatistics> _repository;

        public Handler(IGenericRepository<Domain.Entity.UserStatistics>repository)
        {
            this._repository = repository;
        }
        public Task<GetWeightstatisticsrelatedToUser> Handle(GetUserWeightStatisticsQuery request, CancellationToken cancellationToken)
        {
            var userStatistics = _repository.GetByIdAsync(request.UserStatisticId).Result;
            if (userStatistics == null)
            {
                return Task.FromResult<GetWeightstatisticsrelatedToUser>(null);
            }

            var dto = new GetWeightstatisticsrelatedToUser
            {
                TotalWorkouts = userStatistics.TotalWorkouts,
                TotalCaloriesBurned = userStatistics.TotalCaloriesBurned,
                CurrentStreak = userStatistics.CurrentStreak,
                LongestStreak = userStatistics.LongestStreak,
                LatestWeight = userStatistics.LatestWeight,
                StartingWeight = userStatistics.StartingWeight,
                GoalWeight = userStatistics.GoalWeight
            };
            return Task.FromResult(dto);
        }
    }
}
