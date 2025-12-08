using MediatR;
using ProgressTrackingService.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ProgressTrackingService.Feature.Achievement.GetAll
{
    public class GetAllAchievementsHandler : IRequestHandler<GetAllAchievementsQuery, List<AchievementListDto>>
    {
        private readonly IGenericRepository<Domain.Entity.Achievement> _repository;

        public GetAllAchievementsHandler(IGenericRepository<Domain.Entity.Achievement> repository)
        {
            _repository = repository;
        }

        public Task<List<AchievementListDto>> Handle(GetAllAchievementsQuery request, CancellationToken cancellationToken)
        {
            var items = _repository.GetAll()
                .Select(a => new AchievementListDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    Description = a.Description,
                    Icon = a.IconUrl,
                    Category = "General"
                })
                .OrderBy(a => a.Name)
                .ToList();

            return Task.FromResult(items);
        }
    }
}

