using MediatR;
using FitnessCalculationService.Features.Shared;

namespace FitnessCalculationService.Features.Calculations.CalculateOneRepMax
{
    public record CalculateOneRepMaxQuery(
        double WeightLifted,
        int RepsCompleted,
        string Formula = "Epley" // "Epley", "Brzycki", "Lander", "Lombardi", "Mayhew", "OConner", "Wathan"
    ) : IRequest<RequestResponse<OneRepMaxResult>>;
}

