namespace MeAndMyDog.API.Models.Enums;

/// <summary>
/// Supported translation providers
/// </summary>
public enum TranslationProvider
{
    /// <summary>
    /// Azure Translator service
    /// </summary>
    Azure = 0,
    
    /// <summary>
    /// Google Translate API
    /// </summary>
    Google = 1,
    
    /// <summary>
    /// Amazon Translate
    /// </summary>
    AWS = 2,
    
    /// <summary>
    /// DeepL API
    /// </summary>
    DeepL = 3,
    
    /// <summary>
    /// OpenAI translation (ChatGPT)
    /// </summary>
    OpenAI = 4,
    
    /// <summary>
    /// LibreTranslate (open source)
    /// </summary>
    LibreTranslate = 5,
    
    /// <summary>
    /// Manual human translation
    /// </summary>
    Manual = 6
}