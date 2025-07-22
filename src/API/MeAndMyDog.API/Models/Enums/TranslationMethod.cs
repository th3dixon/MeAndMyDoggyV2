namespace MeAndMyDog.API.Models.Enums;

/// <summary>
/// Methods of translation
/// </summary>
public enum TranslationMethod
{
    /// <summary>
    /// Automatic machine translation
    /// </summary>
    Automatic = 0,
    
    /// <summary>
    /// Retrieved from translation cache
    /// </summary>
    Cached = 1,
    
    /// <summary>
    /// Manual human translation
    /// </summary>
    Manual = 2,
    
    /// <summary>
    /// AI-assisted translation with human review
    /// </summary>
    Assisted = 3,
    
    /// <summary>
    /// Real-time translation
    /// </summary>
    RealTime = 4,
    
    /// <summary>
    /// Batch translation processing
    /// </summary>
    Batch = 5
}