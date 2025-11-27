using MediatR;
using ProgressTrackingService.Domain.Entity;
using ProgressTrackingService.Domain.Interfaces;

namespace ProgressTrackingService.Feature.UserStatisticsfiles.GetWeightStatisticsQueryByUserId
{
    public class Handler : IRequestHandler<GetWeightStatisticsQuery, WeigtStatisticsDto>
    {
        private readonly IGenericRepository<Domain.Entity.UserStatistics> _repository;

        public Handler(IGenericRepository<Domain.Entity.UserStatistics> repository)
        {
            this._repository = repository;
        }
        public Task<WeigtStatisticsDto> Handle(GetWeightStatisticsQuery request, CancellationToken cancellationToken)
        {
            var userStatistics = _repository.GetAll()
                .FirstOrDefault(us => us.UserId == request.UserId);
            if (userStatistics == null)
                {
                return Task.FromResult<WeigtStatisticsDto>(null);
            }
            var dto = new WeigtStatisticsDto
            {
                LatestWeight = userStatistics.LatestWeight,
                StartingWeight = userStatistics.StartingWeight,
                GoalWeight = userStatistics.GoalWeight,
                startDate = userStatistics.CreatedAt,
                lastUpdate = userStatistics.UpdatedAt
            };
            return Task.FromResult(dto);

        }
    }
}
