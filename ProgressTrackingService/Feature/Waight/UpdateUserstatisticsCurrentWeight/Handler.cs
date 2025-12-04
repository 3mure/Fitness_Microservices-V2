using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using ProgressTrackingService.Domain.Interfaces;
using ProgressTrackingService.Feature.UserStatisticsfiles.GetByUserIdQuery;

namespace ProgressTrackingService.Feature.Waight.UpdateUserstatisticsCurrentWeight
{
    public class Handler : IRequestHandler<UpdateUserCurrentWeightInUserstatisticTableCommand, int>
    {
        private readonly IMediator _mediator;
        private readonly IGenericRepository<Domain.Entity.UserStatistics> _repository;
        private readonly IUniteOfWork _uOW;

        public Handler(IMediator mediator , 
            IGenericRepository<Domain.Entity.UserStatistics> repository,
            IUniteOfWork UOW
            )
        {
            this._mediator = mediator;
            this._repository = repository;
            _uOW = UOW;
        }
        public async Task<int> Handle(UpdateUserCurrentWeightInUserstatisticTableCommand request, CancellationToken cancellationToken)
        {
            var userStatisticId = await _mediator.Send(new  GetUserStatisticsId_ByUserIdQuery(request.userId), cancellationToken);
            var NewUserStatistic  = new Domain.Entity.UserStatistics
            {
                Id = userStatisticId,
                LatestWeight = request.currentWeight 
            };
            _repository.SaveInclude(NewUserStatistic, [nameof(NewUserStatistic.LatestWeight)]);
          var flag =  await _uOW.SaveChangesAsync();
            if (flag == 0)
                return StatusCodes.Status412PreconditionFailed;

            return StatusCodes.Status200OK;

        }
    }
}
