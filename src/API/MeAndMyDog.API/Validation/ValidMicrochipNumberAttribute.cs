using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.API.Validation;

/// <summary>
/// Validates that a microchip number follows standard formats
/// </summary>
public class ValidMicrochipNumberAttribute : ValidationAttribute
{
    public ValidMicrochipNumberAttribute()
    {
        ErrorMessage = "Microchip number must be 9, 10, or 15 digits";
    }

    public override bool IsValid(object? value)
    {
        if (value is not string microchipNumber || string.IsNullOrWhiteSpace(microchipNumber))
        {
            return true; // Allow null/empty values, let Required attribute handle if needed
        }

        // Remove any spaces or dashes
        var cleanNumber = microchipNumber.Replace(" ", "").Replace("-", "");

        // Check if it's all digits
        if (!cleanNumber.All(char.IsDigit))
        {
            return false;
        }

        // Check length (9, 10, or 15 digits are common standards)
        return cleanNumber.Length is 9 or 10 or 15;
    }
}