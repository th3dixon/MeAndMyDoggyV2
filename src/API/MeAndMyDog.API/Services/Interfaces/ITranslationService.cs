using MeAndMyDog.API.Models.DTOs;
using MeAndMyDog.API.Models.Enums;

namespace MeAndMyDog.API.Services.Interfaces;

/// <summary>
/// Service interface for message translation functionality
/// </summary>
public interface ITranslationService
{
    /// <summary>
    /// Translate a message
    /// </summary>
    /// <param name="userId">User requesting the translation</param>
    /// <param name="request">Translation request details</param>
    /// <returns>Translation response</returns>
    Task<TranslationResponse> TranslateMessageAsync(string userId, TranslateMessageRequest request);

    /// <summary>
    /// Translate arbitrary text
    /// </summary>
    /// <param name="userId">User requesting the translation</param>
    /// <param name="request">Text translation request</param>
    /// <returns>Translation response</returns>
    Task<TranslationResponse> TranslateTextAsync(string userId, TranslateTextRequest request);

    /// <summary>
    /// Auto-detect the language of text
    /// </summary>
    /// <param name="text">Text to analyze</param>
    /// <param name="provider">Translation provider to use for detection</param>
    /// <returns>Detected language code and confidence</returns>
    Task<LanguageDetectionResult> DetectLanguageAsync(string text, TranslationProvider? provider = null);

    /// <summary>
    /// Get translation for a message by ID
    /// </summary>
    /// <param name="userId">User requesting the translation</param>
    /// <param name="translationId">Translation ID</param>
    /// <returns>Translation details or null if not found</returns>
    Task<MessageTranslationDto?> GetTranslationAsync(string userId, string translationId);

    /// <summary>
    /// Get all translations for a message
    /// </summary>
    /// <param name="userId">User requesting the translations</param>
    /// <param name="messageId">Message ID</param>
    /// <returns>List of translations for the message</returns>
    Task<List<MessageTranslationDto>> GetMessageTranslationsAsync(string userId, string messageId);

    /// <summary>
    /// Rate the quality of a translation
    /// </summary>
    /// <param name="userId">User providing the rating</param>
    /// <param name="request">Rating request</param>
    /// <returns>True if rating was saved successfully</returns>
    Task<bool> RateTranslationAsync(string userId, RateTranslationRequest request);

    /// <summary>
    /// Delete a translation
    /// </summary>
    /// <param name="userId">User requesting deletion</param>
    /// <param name="translationId">Translation ID to delete</param>
    /// <returns>True if deleted successfully</returns>
    Task<bool> DeleteTranslationAsync(string userId, string translationId);

    /// <summary>
    /// Get user's language preferences
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Language preference or null if not set</returns>
    Task<LanguagePreferenceDto?> GetLanguagePreferenceAsync(string userId);

    /// <summary>
    /// Update user's language preferences
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="request">Updated preferences</param>
    /// <returns>Language preference response</returns>
    Task<LanguagePreferenceResponse> UpdateLanguagePreferenceAsync(string userId, UpdateLanguagePreferenceRequest request);

    /// <summary>
    /// Get list of supported languages
    /// </summary>
    /// <param name="provider">Optional provider filter</param>
    /// <returns>List of supported languages</returns>
    Task<List<SupportedLanguageDto>> GetSupportedLanguagesAsync(TranslationProvider? provider = null);

    /// <summary>
    /// Get language information by code
    /// </summary>
    /// <param name="languageCode">Language code (ISO 639-1)</param>
    /// <returns>Language information or null if not supported</returns>
    Task<SupportedLanguageDto?> GetLanguageInfoAsync(string languageCode);

    /// <summary>
    /// Get translation statistics for a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="fromDate">Statistics from date</param>
    /// <param name="toDate">Statistics to date</param>
    /// <returns>Translation statistics</returns>
    Task<TranslationStatsDto> GetTranslationStatsAsync(string userId, DateTimeOffset? fromDate = null, DateTimeOffset? toDate = null);

    /// <summary>
    /// Get recent translations for a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="limit">Maximum translations to return</param>
    /// <returns>List of recent translations</returns>
    Task<List<MessageTranslationDto>> GetRecentTranslationsAsync(string userId, int limit = 20);

    /// <summary>
    /// Clear translation cache entries older than specified days
    /// </summary>
    /// <param name="olderThanDays">Clear cache entries older than this many days</param>
    /// <returns>Number of cache entries cleared</returns>
    Task<int> ClearTranslationCacheAsync(int olderThanDays = 30);

    /// <summary>
    /// Get translation suggestions based on user's conversation history
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="conversationId">Conversation ID</param>
    /// <param name="limit">Maximum suggestions to return</param>
    /// <returns>List of suggested language translations</returns>
    Task<List<TranslationSuggestionDto>> GetTranslationSuggestionsAsync(string userId, string conversationId, int limit = 5);

    /// <summary>
    /// Batch translate multiple messages
    /// </summary>
    /// <param name="userId">User requesting the translations</param>
    /// <param name="messageIds">List of message IDs to translate</param>
    /// <param name="targetLanguage">Target language for all translations</param>
    /// <param name="sourceLanguage">Source language (optional, will auto-detect if not provided)</param>
    /// <returns>List of translation responses</returns>
    Task<List<TranslationResponse>> BatchTranslateMessagesAsync(string userId, List<string> messageIds, string targetLanguage, string? sourceLanguage = null);

    /// <summary>
    /// Get translation cache statistics
    /// </summary>
    /// <returns>Cache statistics</returns>
    Task<TranslationCacheStatsDto> GetCacheStatsAsync();
}