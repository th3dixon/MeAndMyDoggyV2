using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MeAndMyDog.API.DTOs.Translation;
using MeAndMyDog.API.Models.DTOs;
using MeAndMyDog.API.Models.Enums;
using MeAndMyDog.API.Services.Interfaces;
using System.Security.Claims;

namespace MeAndMyDog.API.Controllers;

/// <summary>
/// Controller for message translation operations
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class TranslationController : ControllerBase
{
    private readonly ITranslationService _translationService;
    private readonly ILogger<TranslationController> _logger;

    public TranslationController(
        ITranslationService translationService,
        ILogger<TranslationController> logger)
    {
        _translationService = translationService;
        _logger = logger;
    }

    /// <summary>
    /// Translate a message
    /// </summary>
    /// <param name="request">Translation request</param>
    /// <returns>Translation response</returns>
    [HttpPost("translate/message")]
    public async Task<ActionResult<TranslationResponse>> TranslateMessage([FromBody] TranslateMessageRequest request)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _translationService.TranslateMessageAsync(userId, request);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Invalid translation request: {Message}", ex.Message);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error translating message");
            return StatusCode(500, "An error occurred while translating the message");
        }
    }

    /// <summary>
    /// Translate arbitrary text
    /// </summary>
    /// <param name="request">Text translation request</param>
    /// <returns>Translation response</returns>
    [HttpPost("translate/text")]
    public async Task<ActionResult<TranslationResponse>> TranslateText([FromBody] TranslateTextRequest request)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _translationService.TranslateTextAsync(userId, request);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Invalid text translation request: {Message}", ex.Message);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error translating text");
            return StatusCode(500, "An error occurred while translating the text");
        }
    }

    /// <summary>
    /// Detect language of text
    /// </summary>
    /// <param name="text">Text to analyze</param>
    /// <param name="provider">Optional provider to use for detection</param>
    /// <returns>Language detection result</returns>
    [HttpPost("detect-language")]
    public async Task<ActionResult<LanguageDetectionResult>> DetectLanguage(
        [FromBody] string text,
        [FromQuery] TranslationProvider? provider = null)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(text))
                return BadRequest("Text cannot be empty");

            var result = await _translationService.DetectLanguageAsync(text, provider);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error detecting language");
            return StatusCode(500, "An error occurred while detecting language");
        }
    }

    /// <summary>
    /// Get translation by ID
    /// </summary>
    /// <param name="translationId">Translation ID</param>
    /// <returns>Translation details</returns>
    [HttpGet("{translationId}")]
    public async Task<ActionResult<MessageTranslationDto>> GetTranslation(string translationId)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token");

            var translation = await _translationService.GetTranslationAsync(userId, translationId);
            if (translation == null)
                return NotFound("Translation not found");

            return Ok(translation);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting translation {TranslationId}", translationId);
            return StatusCode(500, "An error occurred while retrieving the translation");
        }
    }

    /// <summary>
    /// Get all translations for a message
    /// </summary>
    /// <param name="messageId">Message ID</param>
    /// <returns>List of translations</returns>
    [HttpGet("message/{messageId}")]
    public async Task<ActionResult<List<MessageTranslationDto>>> GetMessageTranslations(string messageId)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token");

            var translations = await _translationService.GetMessageTranslationsAsync(userId, messageId);
            return Ok(translations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting translations for message {MessageId}", messageId);
            return StatusCode(500, "An error occurred while retrieving translations");
        }
    }

    /// <summary>
    /// Rate a translation quality
    /// </summary>
    /// <param name="request">Rating request</param>
    /// <returns>Success status</returns>
    [HttpPost("rate")]
    public async Task<ActionResult<bool>> RateTranslation([FromBody] RateTranslationRequest request)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _translationService.RateTranslationAsync(userId, request);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Invalid rating request: {Message}", ex.Message);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rating translation");
            return StatusCode(500, "An error occurred while rating the translation");
        }
    }

    /// <summary>
    /// Delete a translation
    /// </summary>
    /// <param name="translationId">Translation ID to delete</param>
    /// <returns>Success status</returns>
    [HttpDelete("{translationId}")]
    public async Task<ActionResult<bool>> DeleteTranslation(string translationId)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token");

            var result = await _translationService.DeleteTranslationAsync(userId, translationId);
            if (!result)
                return NotFound("Translation not found or cannot be deleted");

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting translation {TranslationId}", translationId);
            return StatusCode(500, "An error occurred while deleting the translation");
        }
    }

    /// <summary>
    /// Get user's language preferences
    /// </summary>
    /// <returns>Language preferences</returns>
    [HttpGet("preferences")]
    public async Task<ActionResult<LanguagePreferenceDto>> GetLanguagePreferences()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token");

            var preferences = await _translationService.GetLanguagePreferenceAsync(userId);
            if (preferences == null)
                return NotFound("Language preferences not found");

            return Ok(preferences);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting language preferences for user {UserId}", User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return StatusCode(500, "An error occurred while retrieving language preferences");
        }
    }

    /// <summary>
    /// Update user's language preferences
    /// </summary>
    /// <param name="request">Updated preferences</param>
    /// <returns>Language preference response</returns>
    [HttpPut("preferences")]
    public async Task<ActionResult<LanguagePreferenceResponse>> UpdateLanguagePreferences([FromBody] UpdateLanguagePreferenceRequest request)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _translationService.UpdateLanguagePreferenceAsync(userId, request);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Invalid language preference request: {Message}", ex.Message);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating language preferences");
            return StatusCode(500, "An error occurred while updating language preferences");
        }
    }

    /// <summary>
    /// Get supported languages
    /// </summary>
    /// <param name="provider">Optional provider filter</param>
    /// <returns>List of supported languages</returns>
    [HttpGet("languages")]
    [AllowAnonymous]
    public async Task<ActionResult<List<SupportedLanguageDto>>> GetSupportedLanguages([FromQuery] TranslationProvider? provider = null)
    {
        try
        {
            var languages = await _translationService.GetSupportedLanguagesAsync(provider);
            return Ok(languages);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting supported languages");
            return StatusCode(500, "An error occurred while retrieving supported languages");
        }
    }

    /// <summary>
    /// Get language information by code
    /// </summary>
    /// <param name="languageCode">Language code (ISO 639-1)</param>
    /// <returns>Language information</returns>
    [HttpGet("languages/{languageCode}")]
    [AllowAnonymous]
    public async Task<ActionResult<SupportedLanguageDto>> GetLanguageInfo(string languageCode)
    {
        try
        {
            var language = await _translationService.GetLanguageInfoAsync(languageCode);
            if (language == null)
                return NotFound("Language not supported");

            return Ok(language);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting language info for {LanguageCode}", languageCode);
            return StatusCode(500, "An error occurred while retrieving language information");
        }
    }

    /// <summary>
    /// Get translation statistics
    /// </summary>
    /// <param name="fromDate">Statistics from date</param>
    /// <param name="toDate">Statistics to date</param>
    /// <returns>Translation statistics</returns>
    [HttpGet("stats")]
    public async Task<ActionResult<TranslationStatsDto>> GetTranslationStats(
        [FromQuery] DateTimeOffset? fromDate = null,
        [FromQuery] DateTimeOffset? toDate = null)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token");

            var stats = await _translationService.GetTranslationStatsAsync(userId, fromDate, toDate);
            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting translation statistics");
            return StatusCode(500, "An error occurred while retrieving translation statistics");
        }
    }

    /// <summary>
    /// Get recent translations
    /// </summary>
    /// <param name="limit">Maximum translations to return</param>
    /// <returns>List of recent translations</returns>
    [HttpGet("recent")]
    public async Task<ActionResult<List<MessageTranslationDto>>> GetRecentTranslations([FromQuery] int limit = 20)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token");

            if (limit <= 0 || limit > 100)
                return BadRequest("Limit must be between 1 and 100");

            var translations = await _translationService.GetRecentTranslationsAsync(userId, limit);
            return Ok(translations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting recent translations");
            return StatusCode(500, "An error occurred while retrieving recent translations");
        }
    }

    /// <summary>
    /// Get translation suggestions
    /// </summary>
    /// <param name="conversationId">Conversation ID</param>
    /// <param name="limit">Maximum suggestions to return</param>
    /// <returns>List of translation suggestions</returns>
    [HttpGet("suggestions")]
    public async Task<ActionResult<List<TranslationSuggestionDto>>> GetTranslationSuggestions(
        [FromQuery] string conversationId,
        [FromQuery] int limit = 5)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token");

            if (string.IsNullOrWhiteSpace(conversationId))
                return BadRequest("Conversation ID is required");

            if (limit <= 0 || limit > 20)
                return BadRequest("Limit must be between 1 and 20");

            var suggestions = await _translationService.GetTranslationSuggestionsAsync(userId, conversationId, limit);
            return Ok(suggestions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting translation suggestions");
            return StatusCode(500, "An error occurred while retrieving translation suggestions");
        }
    }

    /// <summary>
    /// Batch translate multiple messages
    /// </summary>
    /// <param name="request">Batch translation request</param>
    /// <returns>List of translation responses</returns>
    [HttpPost("batch")]
    public async Task<ActionResult<List<TranslationResponse>>> BatchTranslateMessages([FromBody] BatchTranslateRequest request)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (request.MessageIds == null || !request.MessageIds.Any())
                return BadRequest("Message IDs are required");

            if (request.MessageIds.Count > 50)
                return BadRequest("Cannot translate more than 50 messages at once");

            var results = await _translationService.BatchTranslateMessagesAsync(
                userId, request.MessageIds, request.TargetLanguage, request.SourceLanguage);

            return Ok(results);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Invalid batch translation request: {Message}", ex.Message);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error batch translating messages");
            return StatusCode(500, "An error occurred while batch translating messages");
        }
    }

    /// <summary>
    /// Get translation cache statistics (admin only)
    /// </summary>
    /// <returns>Cache statistics</returns>
    [HttpGet("cache/stats")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<TranslationCacheStatsDto>> GetCacheStats()
    {
        try
        {
            var stats = await _translationService.GetCacheStatsAsync();
            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cache statistics");
            return StatusCode(500, "An error occurred while retrieving cache statistics");
        }
    }

    /// <summary>
    /// Clear translation cache (admin only)
    /// </summary>
    /// <param name="olderThanDays">Clear cache entries older than this many days</param>
    /// <returns>Number of cache entries cleared</returns>
    [HttpPost("cache/clear")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<int>> ClearTranslationCache([FromQuery] int olderThanDays = 30)
    {
        try
        {
            if (olderThanDays < 1)
                return BadRequest("Days must be at least 1");

            var cleared = await _translationService.ClearTranslationCacheAsync(olderThanDays);
            _logger.LogInformation("Cleared {Count} translation cache entries older than {Days} days", cleared, olderThanDays);
            return Ok(cleared);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing translation cache");
            return StatusCode(500, "An error occurred while clearing translation cache");
        }
    }
}