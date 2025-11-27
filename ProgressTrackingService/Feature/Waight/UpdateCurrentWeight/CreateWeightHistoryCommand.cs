using MediatR;
using ProgressTrackingService.Feature.Waight.UpdateCurrentWeight.DTOs;

namespace ProgressTrackingService.Feature.Waight.UpdateCurrentWeight
{
    public record CreateWeightHistoryCommand(WeightEntryRequestDto WeightEntryRequestDto) : IRequest<UpdateWeightHestoryResponseDto>;
    
}
