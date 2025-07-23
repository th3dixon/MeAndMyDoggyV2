using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.API.Validation;

/// <summary>
/// Validates that a pet name doesn't contain inappropriate content
/// </summary>
public class ValidPetNameAttribute : ValidationAttribute
{
    private readonly string[] _blockedWords = 
    {
        "admin", "administrator", "system", "null", "undefined", "test"
    };

    public ValidPetNameAttribute()
    {
        ErrorMessage = "Pet name contains inappropriate content";
    }

    public override bool IsValid(object? value)
    {
        if (value is not string name || string.IsNullOrWhiteSpace(name))
        {
            return true; // Let Required attribute handle if needed
        }

        var lowerName = name.ToLowerInvariant();
        
        // Check for blocked words
        if (_blockedWords.Any(word => lowerName.Contains(word)))
        {
            return false;
        }

        // Check for excessive special characters
        var specialCharCount = name.Count(c => !char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c));
        if (specialCharCount > 3)
        {
            ErrorMessage = "Pet name contains too many special characters";
            return false;
        }

        return true;
    }
}