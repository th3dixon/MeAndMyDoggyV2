namespace MeAndMyDog.API.Models.Enums;

/// <summary>
/// Security levels for messages
/// </summary>
public enum SecurityLevel
{
    /// <summary>
    /// Standard security level
    /// </summary>
    Standard = 0,
    
    /// <summary>
    /// Enhanced security with basic protections
    /// </summary>
    Enhanced = 1,
    
    /// <summary>
    /// High security with advanced protections
    /// </summary>
    High = 2,
    
    /// <summary>
    /// Maximum security with all protections enabled
    /// </summary>
    Maximum = 3,
    
    /// <summary>
    /// Enterprise security with custom policies
    /// </summary>
    Enterprise = 4,
    
    /// <summary>
    /// Government/military grade security
    /// </summary>
    Classified = 5,
    
    /// <summary>
    /// Custom security configuration
    /// </summary>
    Custom = 6
}