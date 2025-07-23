using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.API.Validation;

/// <summary>
/// Validates that a date of birth is reasonable for a pet
/// </summary>
public class ValidPetAgeAttribute : ValidationAttribute
{
    private readonly int _maxAgeYears;
    private readonly int _minAgeMonths;

    public ValidPetAgeAttribute(int maxAgeYears = 25, int minAgeMonths = 0)
    {
        _maxAgeYears = maxAgeYears;
        _minAgeMonths = minAgeMonths;
        ErrorMessage = $"Pet age must be between {_minAgeMonths} months and {_maxAgeYears} years";
    }

    public override bool IsValid(object? value)
    {
        if (value is not DateTimeOffset dateOfBirth)
        {
            return true; // Let other validation attributes handle null/type issues
        }

        var now = DateTimeOffset.UtcNow;
        var age = now - dateOfBirth;

        // Check if pet is too old
        if (age.TotalDays > (_maxAgeYears * 365))
        {
            return false;
        }

        // Check if pet is too young (if minimum age is specified)
        if (_minAgeMonths > 0 && age.TotalDays < (_minAgeMonths * 30.4))
        {
            return false;
        }

        // Check if date is in the future
        if (dateOfBirth > now)
        {
            ErrorMessage = "Date of birth cannot be in the future";
            return false;
        }

        return true;
    }
}