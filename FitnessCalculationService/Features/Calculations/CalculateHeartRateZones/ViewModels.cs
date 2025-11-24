namespace FitnessCalculationService.Features.Calculations.CalculateHeartRateZones
{
    public record HeartRateZonesResult(
        int MaxHeartRate,
        int? RestingHeartRate,
        HeartRateZone Zone1,
        HeartRateZone Zone2,
        HeartRateZone Zone3,
        HeartRateZone Zone4,
        HeartRateZone Zone5,
        string Method
    );

    public record HeartRateZone(
        string Name,
        int MinBPM,
        int MaxBPM,
        double PercentageMin,
        double PercentageMax,
        string Description
    );
}

