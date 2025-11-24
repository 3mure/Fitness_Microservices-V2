using MediatR;
using FitnessCalculationService.Features.Shared;

namespace FitnessCalculationService.Features.Calculations.CalculateHeartRateZones
{
    public class CalculateHeartRateZonesHandler : IRequestHandler<CalculateHeartRateZonesQuery, RequestResponse<HeartRateZonesResult>>
    {
        public Task<RequestResponse<HeartRateZonesResult>> Handle(CalculateHeartRateZonesQuery request, CancellationToken cancellationToken)
        {
            if (request.Age <= 0 || request.Age > 120)
            {
                return Task.FromResult(RequestResponse<HeartRateZonesResult>.Fail("Age must be between 1 and 120"));
            }

            // Calculate Max Heart Rate: 220 - age
            var maxHeartRate = 220 - request.Age;

            HeartRateZone zone1, zone2, zone3, zone4, zone5;
            string method;

            if (request.RestingHeartRate.HasValue && request.RestingHeartRate.Value > 0)
            {
                // Karvonen Method (more accurate)
                method = "Karvonen Method";
                var heartRateReserve = maxHeartRate - request.RestingHeartRate.Value;

                zone1 = new HeartRateZone(
                    "Recovery",
                    (int)(request.RestingHeartRate.Value + heartRateReserve * 0.5),
                    (int)(request.RestingHeartRate.Value + heartRateReserve * 0.6),
                    50, 60,
                    "Light activity, recovery, warm-up"
                );

                zone2 = new HeartRateZone(
                    "Fat Burn",
                    (int)(request.RestingHeartRate.Value + heartRateReserve * 0.6),
                    (int)(request.RestingHeartRate.Value + heartRateReserve * 0.7),
                    60, 70,
                    "Aerobic zone, fat burning, endurance"
                );

                zone3 = new HeartRateZone(
                    "Aerobic",
                    (int)(request.RestingHeartRate.Value + heartRateReserve * 0.7),
                    (int)(request.RestingHeartRate.Value + heartRateReserve * 0.8),
                    70, 80,
                    "Aerobic capacity, improved cardiovascular fitness"
                );

                zone4 = new HeartRateZone(
                    "Anaerobic",
                    (int)(request.RestingHeartRate.Value + heartRateReserve * 0.8),
                    (int)(request.RestingHeartRate.Value + heartRateReserve * 0.9),
                    80, 90,
                    "Anaerobic threshold, improved speed and power"
                );

                zone5 = new HeartRateZone(
                    "Maximum",
                    (int)(request.RestingHeartRate.Value + heartRateReserve * 0.9),
                    maxHeartRate,
                    90, 100,
                    "Maximum effort, short bursts, elite athletes"
                );
            }
            else
            {
                // Simple Percentage Method
                method = "Percentage Method";

                zone1 = new HeartRateZone(
                    "Recovery",
                    (int)(maxHeartRate * 0.5),
                    (int)(maxHeartRate * 0.6),
                    50, 60,
                    "Light activity, recovery, warm-up"
                );

                zone2 = new HeartRateZone(
                    "Fat Burn",
                    (int)(maxHeartRate * 0.6),
                    (int)(maxHeartRate * 0.7),
                    60, 70,
                    "Aerobic zone, fat burning, endurance"
                );

                zone3 = new HeartRateZone(
                    "Aerobic",
                    (int)(maxHeartRate * 0.7),
                    (int)(maxHeartRate * 0.8),
                    70, 80,
                    "Aerobic capacity, improved cardiovascular fitness"
                );

                zone4 = new HeartRateZone(
                    "Anaerobic",
                    (int)(maxHeartRate * 0.8),
                    (int)(maxHeartRate * 0.9),
                    80, 90,
                    "Anaerobic threshold, improved speed and power"
                );

                zone5 = new HeartRateZone(
                    "Maximum",
                    (int)(maxHeartRate * 0.9),
                    maxHeartRate,
                    90, 100,
                    "Maximum effort, short bursts, elite athletes"
                );
            }

            var result = new HeartRateZonesResult(
                maxHeartRate,
                request.RestingHeartRate,
                zone1,
                zone2,
                zone3,
                zone4,
                zone5,
                method
            );

            return Task.FromResult(RequestResponse<HeartRateZonesResult>.Success(result, "Heart rate zones calculated successfully"));
        }
    }
}

