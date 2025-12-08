using MediatR;
using ProgressTrackingService.Domain.Interfaces;
using ProgressTrackingService.Feature.LogWorkout.CreateWorkoutLogCommand;
using ProgressTrackingService.Feature.LogWorkout.CreateWorkoutLogCommand.DTOs;
using ProgressTrackingService.Feature.LogWorkout.PlaceWorkoutOrchestrator.DTos;
using ProgressTrackingService.Feature.UserStatisticsfiles.GetByUserIdQuery;
using ProgressTrackingService.Feature.UserStatisticsfiles.GetUserstatisticsByIdQuery;
using ProgressTrackingService.Feature.UserStatisticsfiles.UpdateUserStatistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ProgressTrackingService.Feature.LogWorkout.PlaceWorkoutOrchestrator
{
    public record WorkoutOrchestrator(WorkoutLogDto WorkoutLog) : IRequest<WorkoutLogResponseDto>;

    public class WorkoutOrchestratorHandler : IRequestHandler<WorkoutOrchestrator, WorkoutLogResponseDto>
    {
        private readonly IMediator _mediator;
        private readonly IGenericRepository<Domain.Entity.UserStatistics> _statisticsRepository;
        private readonly IGenericRepository<Domain.Entity.Achievement> _achievementRepository;
        private readonly IGenericRepository<Domain.Entity.UserAchievement> _userAchievementRepository;
        private readonly IUniteOfWork _unitOfWork;

        public WorkoutOrchestratorHandler(
            IMediator mediator,
            IGenericRepository<Domain.Entity.UserStatistics> statisticsRepository,
            IGenericRepository<Domain.Entity.Achievement> achievementRepository,
            IGenericRepository<Domain.Entity.UserAchievement> userAchievementRepository,
            IUniteOfWork unitOfWork)
        {
            _mediator = mediator;
            _statisticsRepository = statisticsRepository;
            _achievementRepository = achievementRepository;
            _userAchievementRepository = userAchievementRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<WorkoutLogResponseDto> Handle(WorkoutOrchestrator request, CancellationToken cancellationToken)
        {
            var createWorkoutLogCommand = new CreateWorkOutLogCommand (request.WorkoutLog);
            var workoutLogResponse = await _mediator.Send(createWorkoutLogCommand, cancellationToken);
            // Additional orchestration logic can be added here in the future
            var UpdateUserStatsCommand = new UpdateUserstatisticsCommand(request.WorkoutLog.CaloriesBurned,request.WorkoutLog.UserId);
           await _mediator.Send(UpdateUserStatsCommand, cancellationToken);
            
            
            var userStatisticsId = await _mediator.Send(new GetUserStatisticsId_ByUserIdQuery(request.WorkoutLog.UserId), cancellationToken);

            var userstatistics = await _mediator.Send(new GetUserWeightStatisticsQuery(userStatisticsId), cancellationToken)
                ?? new GetWeightstatisticsrelatedToUser
                {
                    TotalWorkouts = 0,
                    TotalCaloriesBurned = 0,
                    CurrentStreak = 0,
                    LongestStreak = 0,
                    LatestWeight = 0,
                    StartingWeight = 0,
                    GoalWeight = 0
                };
            var achievementsEarned = await EvaluateAndAwardAchievements(request.WorkoutLog.UserId, request.WorkoutLog.CaloriesBurned, cancellationToken);

            var response = new WorkoutLogResponseDto 
            { 
             Id = workoutLogResponse.Id,
                WorkoutId = workoutLogResponse.WorkoutId,
                WorkoutName = workoutLogResponse.WorkoutName,
                CompletedAt = workoutLogResponse.CompletedAt,
                Duration = workoutLogResponse.Duration,
                CaloriesBurned = workoutLogResponse.CaloriesBurned,
                CurrentStreak = userstatistics.CurrentStreak,
                TotalWorkouts = userstatistics.TotalWorkouts,
                TotalCaloriesBurned = userstatistics.TotalCaloriesBurned,
                NewAchievements = achievementsEarned
                

            };
            return response;

        }

        private record AchievementRule(
            string Name,
            string Description,
            string IconUrl,
            Func<Domain.Entity.UserStatistics, WeightStatsSnapshot, bool> Predicate,
            string Category);

        private class WeightStatsSnapshot
        {
            public double LatestWeight { get; init; }
            public double GoalWeight { get; init; }
            public double StartingWeight { get; init; }
        }

        private IEnumerable<AchievementRule> GetRules()
        {
            // Icons are simple placeholders; expected to be replaced by CDN links later
            return new List<AchievementRule>
            {
                new("First Workout", "Complete your first logged workout", "/icons/first-workout.png",
                    (stats, weight) => stats.TotalWorkouts >= 1, "Milestone"),
                new("7-Day Streak", "Work out 7 days in a row", "/icons/streak-7.png",
                    (stats, weight) => stats.CurrentStreak >= 7, "Streak"),
                new("1000 Calories Burned", "Burn a total of 1,000 calories", "/icons/calories-1k.png",
                    (stats, weight) => stats.TotalCaloriesBurned >= 1000, "Calories"),
                new("10000 Calories Burned", "Burn a total of 10,000 calories", "/icons/calories-10k.png",
                    (stats, weight) => stats.TotalCaloriesBurned >= 10000, "Calories"),
                new("Goal Weight Reached", "Reach or beat your goal weight", "/icons/goal-weight.png",
                    (stats, weight) => weight.GoalWeight > 0 && weight.LatestWeight > 0 && weight.LatestWeight <= weight.GoalWeight, "Weight")
            };
        }

        private async Task<List<AchievementDto>> EvaluateAndAwardAchievements(int userId, int caloriesBurnedThisWorkout, CancellationToken cancellationToken)
        {
            var stats = _statisticsRepository.GetAll().FirstOrDefault(s => s.UserId == userId);
            if (stats == null)
            {
                return new List<AchievementDto>();
            }

            // Grab weight stats if available
            var weightSnapshot = new WeightStatsSnapshot
            {
                LatestWeight = stats.LatestWeight,
                GoalWeight = stats.GoalWeight,
                StartingWeight = stats.StartingWeight
            };

            // Ensure base achievement definitions exist
            var rules = GetRules().ToList();
            var existingDefinitions = _achievementRepository.GetAll().ToList();
            var missing = rules.Where(r => !existingDefinitions.Any(a => a.Name == r.Name)).ToList();
            if (missing.Any())
            {
                var newAchievements = missing.Select(r => new Domain.Entity.Achievement
                {
                    Name = r.Name,
                    Description = r.Description,
                    IconUrl = r.IconUrl,
                    CreatedAt = DateTime.UtcNow
                }).ToList();
                await _achievementRepository.AddRangeAsync(newAchievements);
                await _unitOfWork.SaveChangesAsync();
                existingDefinitions = existingDefinitions.Concat(newAchievements).ToList();
            }

            var alreadyEarned = _userAchievementRepository.GetAll()
                .Where(ua => ua.UserId == userId)
                .Select(ua => ua.AchievementId)
                .ToHashSet();

            var earnedThisRun = new List<Domain.Entity.UserAchievement>();
            foreach (var rule in rules)
            {
                var definition = existingDefinitions.First(a => a.Name == rule.Name);
                if (alreadyEarned.Contains(definition.Id))
                    continue;

                if (rule.Predicate(stats, weightSnapshot))
                {
                    earnedThisRun.Add(new Domain.Entity.UserAchievement
                    {
                        UserId = userId,
                        AchievementId = definition.Id,
                        EarnedAt = DateTime.UtcNow
                    });
                }
            }

            if (earnedThisRun.Count == 0)
            {
                return new List<AchievementDto>();
            }

            await _userAchievementRepository.AddRangeAsync(earnedThisRun);
            await _unitOfWork.SaveChangesAsync();

            return earnedThisRun.Select(ua =>
            {
                var def = existingDefinitions.First(a => a.Id == ua.AchievementId);
                return new AchievementDto
                {
                    Id = ua.AchievementId,
                    Name = def.Name,
                    Description = def.Description,
                    Icon = def.IconUrl
                };
            }).ToList();
        }
    } 

}
