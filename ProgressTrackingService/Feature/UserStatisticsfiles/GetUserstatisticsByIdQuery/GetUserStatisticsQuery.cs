using MediatR;

namespace ProgressTrackingService.Feature.UserStatisticsfiles.GetUserstatisticsByIdQuery
{
    public record GetUserWeightStatisticsQuery(int UserStatisticId) : IRequest<GetWeightstatisticsrelatedToUser>;

}
