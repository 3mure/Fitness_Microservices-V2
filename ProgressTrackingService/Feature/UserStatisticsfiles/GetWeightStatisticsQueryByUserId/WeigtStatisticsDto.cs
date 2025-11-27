namespace ProgressTrackingService.Feature.UserStatisticsfiles.GetWeightStatisticsQueryByUserId
{
    public class WeigtStatisticsDto
    {
        
        public double LatestWeight { get; set; }
        public double StartingWeight { get; set; }
        public double GoalWeight { get; set; }
        public DateTime? startDate { get; set; }
        public DateTime? lastUpdate { get; set; }

    }
}
