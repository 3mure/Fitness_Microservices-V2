using MediatR;

namespace ProgressTrackingService.Feature.UserStatisticsfiles.GetWeightStatisticsQueryByUserId
{
    public record GetWeightStatisticsQuery(int UserId) : IRequest<WeigtStatisticsDto>;
  
    
}
