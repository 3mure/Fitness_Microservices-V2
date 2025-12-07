using System.Collections.Generic;
using MediatR;
using ProgressTrackingService.Domain.Entity;
using ProgressTrackingService.Domain.Interfaces;
using ProgressTrackingService.Feature.GetUserProgress.DTOs;
using ProgressTrackingService.Helper;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ProgressTrackingService.Feature.GetUserProgress
{
    public class Handler : IRequestHandler<GetUserProgressQuery, UserProgressDto>
    {
        private readonly IGenericRepository<WorkoutLog> workoutRepository;
        private readonly IGenericRepository<Domain.Entity.UserStatistics> statisticRepository;
        private readonly IGenericRepository<WeightHistory> weightRepository;
        private readonly IGenericRepository<UserAchievement> achievementRepository;

        public Handler(IGenericRepository<WorkoutLog>_WorkoutRepository,
         IGenericRepository<Domain.Entity.UserStatistics> _StatisticRepository,
         IGenericRepository<WeightHistory> _WeightRepository,
            IGenericRepository<UserAchievement> _AchievementRepository
         )
        {
            workoutRepository = _WorkoutRepository;
            statisticRepository = _StatisticRepository;
            weightRepository = _WeightRepository;
            achievementRepository = _AchievementRepository;
        }
        public Task<UserProgressDto> Handle(GetUserProgressQuery request, CancellationToken cancellationToken)
        {
            // Calculate date range based on period or custom dates
            DateTime? startDateFilter = null;
            DateTime? endDateFilter = null;
            DateTime today = DateTime.Today;

            if (!string.IsNullOrEmpty(request.startDate) && !string.IsNullOrEmpty(request.endDate))
            {
                // Custom date range
                if (DateTime.TryParse(request.startDate, out DateTime parsedStart))
                    startDateFilter = parsedStart.Date;
                if (DateTime.TryParse(request.endDate, out DateTime parsedEnd))
                    endDateFilter = parsedEnd.Date.AddDays(1).AddTicks(-1); // End of day
            }
            else
            {
                // Use period
                var period = (request.period ?? "month").ToLower();
                switch (period)
                {
                    case "week":
                        startDateFilter = today.AddDays(-7);
                        endDateFilter = today.AddDays(1).AddTicks(-1);
                        break;
                    case "month":
                        startDateFilter = today.AddMonths(-1);
                        endDateFilter = today.AddDays(1).AddTicks(-1);
                        break;
                    case "year":
                        startDateFilter = today.AddYears(-1);
                        endDateFilter = today.AddDays(1).AddTicks(-1);
                        break;
                    case "all":
                    default:
                        // No filter - get all data
                        break;
                }
            }

            ////// Summary//////
            var statistics = statisticRepository.GetAll().FirstOrDefault(s => s.UserId == request.userId);
           
            var summary = new SummaryDto
            {
                TotalWorkouts = statistics?.TotalWorkouts ?? 0,
                WeightChange= CalculationMethods.CalculateTotalWeightLost(statistics?.StartingWeight ?? 0, statistics?.LatestWeight ?? 0),
                CurrentStreak = statistics?.CurrentStreak ?? 0,
                LongestStreak= statistics?.LongestStreak ?? 0,
                StartWeight= statistics?.StartingWeight ?? 0,
                CurrentWeight= statistics?.LatestWeight ?? 0,
                GoalWeight= statistics?.GoalWeight ?? 0,
                ProgressPercentage= CalculationMethods.CalculateProgressToGoal(statistics?.StartingWeight ?? 0, statistics?.LatestWeight ?? 0, statistics?.GoalWeight ?? 0),
                TotalCaloriesBurned= statistics?.TotalCaloriesBurned ?? 0,
               // AverageWorkoutDuration= statistics?.AverageWorkoutDuration ?? 0

            };
            ////// Weight History with date filtering//////
            var weightQuery = weightRepository.GetAll()
                .Where(w => w.UserId == request.userId);

            if (startDateFilter.HasValue)
                weightQuery = weightQuery.Where(w => w.LoggedAt >= startDateFilter.Value);
            if (endDateFilter.HasValue)
                weightQuery = weightQuery.Where(w => w.LoggedAt <= endDateFilter.Value);

            var weightEntries = weightQuery
                .OrderByDescending(w => w.CreatedAt)
                .Select(w => new WeightHistoryDto
                {
                    Weight = w.Weight,
                    Date = w.LoggedAt,
                    Bmi= w.Bmi

                })
                .ToList();

            ////Workout with date filtering////
            var workoutQuery = workoutRepository.GetAll()
                .Where(w => w.UserId == request.userId && w.CreatedAt.HasValue);

            if (startDateFilter.HasValue)
                workoutQuery = workoutQuery.Where(w => w.CreatedAt.Value >= startDateFilter.Value);
            if (endDateFilter.HasValue)
                workoutQuery = workoutQuery.Where(w => w.CreatedAt.Value <= endDateFilter.Value);
            
            var WorkoutEntries = workoutQuery
                .GroupBy(w=>w.CreatedAt.Value.Date)
                .OrderByDescending(g=>g.Key)

                .Select(g => new WorkoutHistoryDto
                {
                    Date = g.Key,                                
                    WorkoutsCompleted = g.Count(),              
                    TotalDuration = g.Sum(x => x.Duration),      
                    CaloriesBurned = g.Sum(x => x.CaloriesBurned) ,
                    Workouts= g.Select(x => new WorkoutDto
                    {
                        Id = x.Id,
                        Name = x.WorkoutName,
                        Duration = x.Duration,
                        CaloriesBurned = x.CaloriesBurned
                    }).ToList()

                }) .ToList();
            ///Weekly Stats (always current week vs previous week, regardless of period filter)////

            var today = DateTime.Today;
            var startOfCurrentWeek = today.AddDays(-7);
            var startOfPreviousWeek = today.AddDays(-14);
            
            var currentWeek = workoutRepository.GetAll()
                             .Where(w => w.UserId == request.userId && w.CreatedAt.HasValue)
                             .Where(w => w.CreatedAt.Value >= startOfCurrentWeek && w.CreatedAt.Value < today)
                             .ToList();

            var previousWeek = workoutRepository.GetAll()
                             .Where(w => w.UserId == request.userId && w.CreatedAt.HasValue)
                             .Where(w => w.CreatedAt.Value >= startOfPreviousWeek && w.CreatedAt.Value < startOfCurrentWeek)
                             .ToList();

            var currentStats = new WeeklyStatsDto
            {
                WorkoutsThisWeek = currentWeek.Count,
                TotalDuration = currentWeek.Sum(w => w.Duration),
                CaloriesBurned = currentWeek.Sum(w => w.CaloriesBurned),
                AverageDuration = currentWeek.Count > 0 ? currentWeek.Sum(w => w.Duration) / currentWeek.Count : 0
            };
             
            var previousStats = new WeeklyStatsDto
            {
                WorkoutsThisWeek = previousWeek.Count,
                TotalDuration = previousWeek.Sum(w => w.Duration),
                CaloriesBurned = previousWeek.Sum(w => w.CaloriesBurned),
                AverageDuration = previousWeek.Count > 0 ? previousWeek.Sum(w => w.Duration) / previousWeek.Count : 0
            };

            var comparison = new
            {
                Workouts = (currentStats.WorkoutsThisWeek - previousStats.WorkoutsThisWeek).ToString("+0;-0"),
                Duration = previousStats.TotalDuration == 0 ? "N/A" :
                $"{((double)(currentStats.TotalDuration - previousStats.TotalDuration) / previousStats.TotalDuration * 100):+0;-0}%",
                Calories = previousStats.CaloriesBurned == 0 ? "N/A" :
                $"{((double)(currentStats.CaloriesBurned - previousStats.CaloriesBurned) / previousStats.CaloriesBurned * 100):+0;-0}%"
            };

            var weeklyStats = new WeeklyStatsDto
            {
                WorkoutsThisWeek = currentStats.WorkoutsThisWeek,
                TotalDuration = currentStats.TotalDuration,
                CaloriesBurned = currentStats.CaloriesBurned,
                AverageDuration = currentStats.AverageDuration,
                ComparisonToPreviousWeek = new ComparisonDto
                {
                    Workouts = comparison.Workouts,
                    Duration = comparison.Duration,
                    Calories = comparison.Calories
                }
            };

            ////// Achievements//////

            var userProgress = new UserProgressDto
            {
                Summary = summary,
                WeightHistory = weightEntries,
                WorkoutHistory = WorkoutEntries,
                WeeklyStats = weeklyStats,
                Achievements = null
            };
            return Task.FromResult(userProgress); 

            
        }
    

    }
}
