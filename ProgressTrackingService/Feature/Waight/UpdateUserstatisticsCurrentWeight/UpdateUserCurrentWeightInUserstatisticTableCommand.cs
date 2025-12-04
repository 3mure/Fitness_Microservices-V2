using MediatR;

namespace ProgressTrackingService.Feature.Waight.UpdateUserstatisticsCurrentWeight
{
    public record UpdateUserCurrentWeightInUserstatisticTableCommand(int userId, double currentWeight) : IRequest<int>;

    
}
