using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.API.Validation;

/// <summary>
/// Validates that a gender value is from the allowed list
/// </summary>
public class ValidPetGenderAttribute : ValidationAttribute
{
    private readonly string[] _validGenders = { "Male", "Female", "Unknown" };

    public ValidPetGenderAttribute()
    {
        ErrorMessage = "Gender must be Male, Female, or Unknown";
    }

    public override bool IsValid(object? value)
    {
        if (value is not string gender || string.IsNullOrWhiteSpace(gender))
        {
            return true; // Allow null/empty values
        }

        return _validGenders.Contains(gender, StringComparer.OrdinalIgnoreCase);
    }
}