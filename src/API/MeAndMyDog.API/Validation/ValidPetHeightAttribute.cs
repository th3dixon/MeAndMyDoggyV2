using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.API.Validation;

/// <summary>
/// Validates that a pet height is reasonable
/// </summary>
public class ValidPetHeightAttribute : ValidationAttribute
{
    private readonly decimal _minHeight;
    private readonly decimal _maxHeight;

    public ValidPetHeightAttribute(double minHeight = 5.0, double maxHeight = 120.0)
    {
        _minHeight = (decimal)minHeight;
        _maxHeight = (decimal)maxHeight;
        ErrorMessage = $"Pet height must be between {_minHeight} and {_maxHeight} cm";
    }

    public override bool IsValid(object? value)
    {
        if (value is not decimal height)
        {
            return true; // Let other validation handle null/type issues
        }

        return height >= _minHeight && height <= _maxHeight;
    }
}