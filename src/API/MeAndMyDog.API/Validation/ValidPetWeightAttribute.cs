using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.API.Validation;

/// <summary>
/// Validates that a pet weight is reasonable
/// </summary>
public class ValidPetWeightAttribute : ValidationAttribute
{
    private readonly decimal _minWeight;
    private readonly decimal _maxWeight;

    public ValidPetWeightAttribute(double minWeight = 0.1, double maxWeight = 150.0)
    {
        _minWeight = (decimal)minWeight;
        _maxWeight = (decimal)maxWeight;
        ErrorMessage = $"Pet weight must be between {_minWeight} and {_maxWeight} kg";
    }

    public override bool IsValid(object? value)
    {
        if (value is not decimal weight)
        {
            return true; // Let other validation handle null/type issues
        }

        return weight >= _minWeight && weight <= _maxWeight;
    }
}