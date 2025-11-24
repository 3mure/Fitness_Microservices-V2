namespace ProgressTrackingService.Helper
{
    public class CalculationMethods
    {
        public static double CalculateDifference(double lastWeight, double previousWeight)
        {
            return Math.Round(lastWeight - previousWeight, 2);
        }

        public static double CalculateTotalWeightLost(double startWeight, double lastWeight)
        {
            return Math.Round(startWeight - lastWeight, 2);
        }

        public static double CalculateProgressToGoal(double startWeight, double lastWeight, double goalWeight)
        {
            if (startWeight == goalWeight) return 100; // already at goal
            return Math.Round(((startWeight - lastWeight) / (startWeight - goalWeight)) * 100, 2);
        }

        public static string CalculateTrend(double difference)
        {
            if (difference < 0) return "decreasing";
            if (difference > 0) return "increasing";
            return "stable";
        }

        public static int CalculateEstimatedDaysToGoal(
            double startWeight,
            double lastWeight,
            double goalWeight,
            DateTime startDate,
            DateTime lastEntryDate)
        {
            var totalDaysElapsed = (lastEntryDate - startDate).TotalDays;
            if (totalDaysElapsed <= 0) return -1;

            var averageDailyLoss = (startWeight - lastWeight) / totalDaysElapsed;
            if (averageDailyLoss <= 0) return -1;

            var remainingWeight = lastWeight - goalWeight;
            return (int)Math.Round(remainingWeight / averageDailyLoss);
        } 
    }
}
