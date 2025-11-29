 using MediatR;
using Microsoft.AspNetCore.Http;
using ProgressTrackingService.Domain.Entity;
using ProgressTrackingService.Domain.Interfaces;
using ProgressTrackingService.Feature.UserStatisticsfiles.GetWeightStatisticsQueryByUserId;
using ProgressTrackingService.Feature.Waight.UpdateCurrentWeight.DTOs;
using ProgressTrackingService.Helper;
using ProgressTrackingService.Infrastructure;
using ProgressTrackingService.Shared;

namespace ProgressTrackingService.Feature.Waight.UpdateCurrentWeight
{
    public class CreateWeightHestoryCommandHandler : IRequestHandler<CreateWeightHistoryCommand, UpdateWeightHestoryResponseDto>
    {
        private readonly IGenericRepository<WeightHistory> _repository;
        private readonly IUniteOfWork _uOW;
        private readonly IMediator _mediator;
        private readonly BmiServiceClient _bmiService;

        public CreateWeightHestoryCommandHandler(IGenericRepository<WeightHistory> repository,
            IUniteOfWork UOW,
            IMediator mediator,
            BmiServiceClient bmi)
        {
            _repository = repository;
            _uOW = UOW;
            this._mediator = mediator;
            this._bmiService = bmi;
        }

        public async Task<UpdateWeightHestoryResponseDto> Handle(CreateWeightHistoryCommand request, CancellationToken cancellationToken)
        {
            
            var bmi = await _bmiService.GetBmiAsync(request.WeightEntryRequestDto.Weight, request.WeightEntryRequestDto.height);

            var weigthHistory = new WeightHistory 
            {
                UserId = request.WeightEntryRequestDto.UserId,
                Weight = request.WeightEntryRequestDto.Weight,
                CreatedAt = DateTime.Now,
                LoggedAt = request.WeightEntryRequestDto.Date
            };
          
            var AddedWeight = await _repository.AddAsync(weigthHistory);

            await _uOW.SaveChangesAsync();


            var weightStatistics = await _mediator.Send(new GetWeightStatisticsQuery(request.WeightEntryRequestDto.UserId));

            var difference = CalculationMethods.CalculateDifference(request.WeightEntryRequestDto.Weight, weightStatistics.LatestWeight);

            var Updatedweightresponse = new UpdateWeightHestoryResponseDto
            {
                EntryId = AddedWeight.Id,
                Weight = AddedWeight.Weight,
                Date = AddedWeight.LoggedAt,
                TotalWeightLost = CalculationMethods.CalculateTotalWeightLost(weightStatistics.StartingWeight, request.WeightEntryRequestDto.Weight),
                PreviousWeight = weightStatistics.LatestWeight,
                Difference = difference,
                EstimatedDaysToGoal = CalculationMethods.CalculateEstimatedDaysToGoal(
                    weightStatistics.StartingWeight,
                    weightStatistics.LatestWeight,
                    weightStatistics.GoalWeight,
                    weightStatistics.startDate.GetValueOrDefault(),
                    weightStatistics.lastUpdate.GetValueOrDefault()
                ),
                GoalRemaining = request.WeightEntryRequestDto.Weight - weightStatistics.GoalWeight,
                ProgressToGoal = CalculationMethods.CalculateProgressToGoal(weightStatistics.StartingWeight,
                request.WeightEntryRequestDto.Weight, weightStatistics.GoalWeight),
                Trend = CalculationMethods.CalculateTrend(difference),
                
                Bmi=bmi
            };
            return Updatedweightresponse;



        }
    }
}
