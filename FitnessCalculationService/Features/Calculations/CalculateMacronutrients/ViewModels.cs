namespace FitnessCalculationService.Features.Calculations.CalculateMacronutrients
{
    public record MacronutrientResult(
        double TotalCalories,
        ProteinMacro Protein,
        CarbohydrateMacro Carbohydrates,
        FatMacro Fat,
        string Goal,
        string Description
    );

    public record ProteinMacro(
        double Grams,
        double Calories,
        double Percentage
    );

    public record CarbohydrateMacro(
        double Grams,
        double Calories,
        double Percentage
    );

    public record FatMacro(
        double Grams,
        double Calories,
        double Percentage
    );
}

