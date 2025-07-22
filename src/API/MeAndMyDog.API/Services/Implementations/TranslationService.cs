using Microsoft.EntityFrameworkCore;
using MeAndMyDog.API.Data;
using MeAndMyDog.API.Models.DTOs;
using MeAndMyDog.API.Models.Entities;
using MeAndMyDog.API.Models.Enums;
using MeAndMyDog.API.Services.Interfaces;
using MeAndMyDog.API.Services.Helpers;
using System.Security.Cryptography;
using System.Text;

namespace MeAndMyDog.API.Services.Implementations;

/// <summary>
/// Service implementation for message translation functionality
/// </summary>
public class TranslationService : ITranslationService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<TranslationService> _logger;

    // Static language mappings for common languages
    private static readonly Dictionary<string, SupportedLanguageDto> SupportedLanguages = new()
    {
        ["en"] = new SupportedLanguageDto { Code = "en", Name = "English", NativeName = "English", IsCommon = true },
        ["es"] = new SupportedLanguageDto { Code = "es", Name = "Spanish", NativeName = "Español", IsCommon = true },
        ["fr"] = new SupportedLanguageDto { Code = "fr", Name = "French", NativeName = "Français", IsCommon = true },
        ["de"] = new SupportedLanguageDto { Code = "de", Name = "German", NativeName = "Deutsch", IsCommon = true },
        ["it"] = new SupportedLanguageDto { Code = "it", Name = "Italian", NativeName = "Italiano", IsCommon = true },
        ["pt"] = new SupportedLanguageDto { Code = "pt", Name = "Portuguese", NativeName = "Português", IsCommon = true },
        ["ru"] = new SupportedLanguageDto { Code = "ru", Name = "Russian", NativeName = "Русский", IsCommon = true },
        ["ja"] = new SupportedLanguageDto { Code = "ja", Name = "Japanese", NativeName = "日本語", IsCommon = true },
        ["ko"] = new SupportedLanguageDto { Code = "ko", Name = "Korean", NativeName = "한국어", IsCommon = true },
        ["zh"] = new SupportedLanguageDto { Code = "zh", Name = "Chinese", NativeName = "中文", IsCommon = true },
        ["ar"] = new SupportedLanguageDto { Code = "ar", Name = "Arabic", NativeName = "العربية", Direction = "rtl", IsCommon = true },
        ["hi"] = new SupportedLanguageDto { Code = "hi", Name = "Hindi", NativeName = "हिन्दी", IsCommon = true },
        ["nl"] = new SupportedLanguageDto { Code = "nl", Name = "Dutch", NativeName = "Nederlands" },
        ["sv"] = new SupportedLanguageDto { Code = "sv", Name = "Swedish", NativeName = "Svenska" },
        ["no"] = new SupportedLanguageDto { Code = "no", Name = "Norwegian", NativeName = "Norsk" },
        ["da"] = new SupportedLanguageDto { Code = "da", Name = "Danish", NativeName = "Dansk" },
        ["fi"] = new SupportedLanguageDto { Code = "fi", Name = "Finnish", NativeName = "Suomi" },
        ["pl"] = new SupportedLanguageDto { Code = "pl", Name = "Polish", NativeName = "Polski" },
        ["tr"] = new SupportedLanguageDto { Code = "tr", Name = "Turkish", NativeName = "Türkçe" },
        ["he"] = new SupportedLanguageDto { Code = "he", Name = "Hebrew", NativeName = "עברית", Direction = "rtl" },
        ["th"] = new SupportedLanguageDto { Code = "th", Name = "Thai", NativeName = "ไทย" },
        ["vi"] = new SupportedLanguageDto { Code = "vi", Name = "Vietnamese", NativeName = "Tiếng Việt" }
    };

    /// <summary>
    /// Initialize the translation service
    /// </summary>
    public TranslationService(ApplicationDbContext context, ILogger<TranslationService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<TranslationResponse> TranslateMessageAsync(string userId, TranslateMessageRequest request)
    {
        try
        {
            // Get the message
            var message = await _context.Messages
                .FirstOrDefaultAsync(m => m.Id == request.MessageId);

            if (message == null)
            {
                return new TranslationResponse
                {
                    Success = false,
                    Error = "Message not found"
                };
            }

            // Check if user has access to this message
            var hasAccess = await _context.Conversations
                .Where(c => c.Id == message.ConversationId)
                .SelectMany(c => c.Participants)
                .AnyAsync(p => p.UserId == userId);

            if (!hasAccess)
            {
                return new TranslationResponse
                {
                    Success = false,
                    Error = "Access denied to this message"
                };
            }

            // Check if translation already exists
            if (request.UseCache)
            {
                var existingTranslation = await _context.MessageTranslations
                    .FirstOrDefaultAsync(t => t.MessageId == request.MessageId &&
                                            t.TargetLanguage == request.TargetLanguage &&
                                            (request.SourceLanguage == null || t.SourceLanguage == request.SourceLanguage));

                if (existingTranslation != null)
                {
                    // Update access statistics
                    existingTranslation.AccessCount++;
                    existingTranslation.LastAccessedAt = DateTimeOffset.UtcNow;
                    await _context.SaveChangesAsync();

                    return new TranslationResponse
                    {
                        Success = true,
                        Translation = await MapToTranslationDto(existingTranslation)
                    };
                }
            }

            // Perform the actual translation
            var translationResult = await TranslateTextInternalAsync(message.Content, request.TargetLanguage, 
                request.SourceLanguage, request.PreferredProvider, request.UseCache);

            if (!translationResult.Success)
            {
                return new TranslationResponse
                {
                    Success = false,
                    Error = translationResult.Error
                };
            }

            // Save the translation
            var messageTranslation = new MessageTranslation
            {
                Id = Guid.NewGuid().ToString(),
                MessageId = request.MessageId,
                UserId = userId,
                SourceLanguage = translationResult.SourceLanguage!,
                TargetLanguage = request.TargetLanguage,
                SourceText = message.Content,
                TranslatedText = translationResult.TranslatedText!,
                ConfidenceScore = translationResult.ConfidenceScore,
                TranslationProvider = EnumConverter.ToString(translationResult.Provider),
                TranslationMethod = translationResult.IsCached ? "cached" : "automatic",
                IsCached = translationResult.IsCached,
                CharacterCount = translationResult.TranslatedText!.Length,
                TranslationCost = translationResult.Cost,
                CreatedAt = DateTimeOffset.UtcNow,
                AccessCount = 1
            };

            _context.MessageTranslations.Add(messageTranslation);
            await _context.SaveChangesAsync();

            var translationDto = await MapToTranslationDto(messageTranslation);

            _logger.LogInformation("Message {MessageId} translated to {TargetLanguage} for user {UserId}", 
                request.MessageId, request.TargetLanguage, userId);

            return new TranslationResponse
            {
                Success = true,
                Translation = translationDto
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error translating message {MessageId} for user {UserId}", request.MessageId, userId);
            return new TranslationResponse
            {
                Success = false,
                Error = "An error occurred while translating the message"
            };
        }
    }

    /// <inheritdoc />
    public async Task<TranslationResponse> TranslateTextAsync(string userId, TranslateTextRequest request)
    {
        try
        {
            // Perform the translation
            var translationResult = await TranslateTextInternalAsync(request.Text, request.TargetLanguage,
                request.SourceLanguage, request.PreferredProvider, request.UseCache);

            if (!translationResult.Success)
            {
                return new TranslationResponse
                {
                    Success = false,
                    Error = translationResult.Error
                };
            }

            // Create a temporary translation DTO (not saved to database)
            var translationDto = new MessageTranslationDto
            {
                Id = Guid.NewGuid().ToString(),
                MessageId = "", // No associated message
                UserId = userId,
                SourceLanguage = translationResult.SourceLanguage!,
                TargetLanguage = request.TargetLanguage,
                SourceText = request.Text,
                TranslatedText = translationResult.TranslatedText!,
                ConfidenceScore = translationResult.ConfidenceScore,
                TranslationProvider = translationResult.Provider,
                TranslationMethod = translationResult.IsCached ? TranslationMethod.Cached : TranslationMethod.Automatic,
                IsCached = translationResult.IsCached,
                CharacterCount = translationResult.TranslatedText!.Length,
                TranslationCost = translationResult.Cost,
                CreatedAt = DateTimeOffset.UtcNow,
                AccessCount = 1
            };

            // Add language names
            var sourceLanguage = await GetLanguageInfoAsync(translationResult.SourceLanguage!);
            var targetLanguage = await GetLanguageInfoAsync(request.TargetLanguage);
            translationDto.SourceLanguageName = sourceLanguage?.Name ?? translationResult.SourceLanguage!;
            translationDto.TargetLanguageName = targetLanguage?.Name ?? request.TargetLanguage;

            _logger.LogInformation("Text translated from {SourceLanguage} to {TargetLanguage} for user {UserId}", 
                translationResult.SourceLanguage, request.TargetLanguage, userId);

            return new TranslationResponse
            {
                Success = true,
                Translation = translationDto
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error translating text for user {UserId}", userId);
            return new TranslationResponse
            {
                Success = false,
                Error = "An error occurred while translating the text"
            };
        }
    }

    /// <inheritdoc />
    public async Task<LanguageDetectionResult> DetectLanguageAsync(string text, TranslationProvider? provider = null)
    {
        try
        {
            // This would typically integrate with translation service APIs
            // For now, we'll implement a basic heuristic approach
            await Task.Delay(50); // Simulate API call

            // Simple heuristic based on character patterns
            var detectedLanguage = DetectLanguageHeuristic(text);

            return new LanguageDetectionResult
            {
                Success = true,
                LanguageCode = detectedLanguage.Code,
                LanguageName = detectedLanguage.Name,
                ConfidenceScore = detectedLanguage.Confidence,
                Alternatives = detectedLanguage.Alternatives
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error detecting language for text");
            return new LanguageDetectionResult
            {
                Success = false,
                Error = "An error occurred while detecting the language"
            };
        }
    }

    /// <inheritdoc />
    public async Task<MessageTranslationDto?> GetTranslationAsync(string userId, string translationId)
    {
        try
        {
            var translation = await _context.MessageTranslations
                .FirstOrDefaultAsync(t => t.Id == translationId && t.UserId == userId);

            return translation != null ? await MapToTranslationDto(translation) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting translation {TranslationId} for user {UserId}", translationId, userId);
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<List<MessageTranslationDto>> GetMessageTranslationsAsync(string userId, string messageId)
    {
        try
        {
            var translations = await _context.MessageTranslations
                .Where(t => t.MessageId == messageId && t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            var translationDtos = new List<MessageTranslationDto>();
            foreach (var translation in translations)
            {
                translationDtos.Add(await MapToTranslationDto(translation));
            }

            return translationDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting translations for message {MessageId}", messageId);
            return new List<MessageTranslationDto>();
        }
    }

    /// <inheritdoc />
    public async Task<bool> RateTranslationAsync(string userId, RateTranslationRequest request)
    {
        try
        {
            var translation = await _context.MessageTranslations
                .FirstOrDefaultAsync(t => t.Id == request.TranslationId && t.UserId == userId);

            if (translation == null)
            {
                return false;
            }

            translation.QualityRating = request.Rating;
            translation.QualityFeedback = request.Feedback;

            // Update cache entry if this translation was cached
            if (translation.IsCached)
            {
                await UpdateCacheQualityRatingAsync(translation.SourceText, translation.SourceLanguage, 
                    translation.TargetLanguage, request.Rating);
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Translation {TranslationId} rated {Rating}/5 by user {UserId}", 
                request.TranslationId, request.Rating, userId);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rating translation {TranslationId}", request.TranslationId);
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<bool> DeleteTranslationAsync(string userId, string translationId)
    {
        try
        {
            var translation = await _context.MessageTranslations
                .FirstOrDefaultAsync(t => t.Id == translationId && t.UserId == userId);

            if (translation == null)
            {
                return false;
            }

            _context.MessageTranslations.Remove(translation);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Translation {TranslationId} deleted by user {UserId}", translationId, userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting translation {TranslationId}", translationId);
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<LanguagePreferenceDto?> GetLanguagePreferenceAsync(string userId)
    {
        try
        {
            var preference = await _context.UserLanguagePreferences
                .FirstOrDefaultAsync(p => p.UserId == userId && p.IsActive);

            return preference != null ? await MapToLanguagePreferenceDto(preference) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting language preference for user {UserId}", userId);
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<LanguagePreferenceResponse> UpdateLanguagePreferenceAsync(string userId, UpdateLanguagePreferenceRequest request)
    {
        try
        {
            // Validate languages
            if (!SupportedLanguages.ContainsKey(request.PrimaryLanguage))
            {
                return new LanguagePreferenceResponse
                {
                    Success = false,
                    Error = "Primary language is not supported"
                };
            }

            var existingPreference = await _context.UserLanguagePreferences
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (existingPreference != null)
            {
                // Update existing preference
                existingPreference.PrimaryLanguage = request.PrimaryLanguage;
                existingPreference.SecondaryLanguages = string.Join(",", request.SecondaryLanguages);
                existingPreference.AutoTranslateIncoming = request.AutoTranslateIncoming;
                existingPreference.AutoDetectOutgoing = request.AutoDetectOutgoing;
                existingPreference.PreferredProvider = EnumConverter.ToString(request.PreferredProvider);
                existingPreference.MinConfidenceThreshold = request.MinConfidenceThreshold;
                existingPreference.ShowConfidenceScores = request.ShowConfidenceScores;
                existingPreference.EnableTranslationCache = request.EnableTranslationCache;
                existingPreference.ExcludeLanguages = string.Join(",", request.ExcludeLanguages);
                existingPreference.EnableSuggestions = request.EnableSuggestions;
                existingPreference.UpdatedAt = DateTimeOffset.UtcNow;

                await _context.SaveChangesAsync();

                var preferenceDto = await MapToLanguagePreferenceDto(existingPreference);

                return new LanguagePreferenceResponse
                {
                    Success = true,
                    Preference = preferenceDto
                };
            }
            else
            {
                // Create new preference
                var newPreference = new UserLanguagePreference
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = userId,
                    PrimaryLanguage = request.PrimaryLanguage,
                    SecondaryLanguages = string.Join(",", request.SecondaryLanguages),
                    AutoTranslateIncoming = request.AutoTranslateIncoming,
                    AutoDetectOutgoing = request.AutoDetectOutgoing,
                    PreferredProvider = EnumConverter.ToString(request.PreferredProvider),
                    MinConfidenceThreshold = request.MinConfidenceThreshold,
                    ShowConfidenceScores = request.ShowConfidenceScores,
                    EnableTranslationCache = request.EnableTranslationCache,
                    ExcludeLanguages = string.Join(",", request.ExcludeLanguages),
                    EnableSuggestions = request.EnableSuggestions,
                    IsActive = true,
                    CreatedAt = DateTimeOffset.UtcNow,
                    UpdatedAt = DateTimeOffset.UtcNow
                };

                _context.UserLanguagePreferences.Add(newPreference);
                await _context.SaveChangesAsync();

                var preferenceDto = await MapToLanguagePreferenceDto(newPreference);

                _logger.LogInformation("Language preference created for user {UserId}", userId);

                return new LanguagePreferenceResponse
                {
                    Success = true,
                    Preference = preferenceDto
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating language preference for user {UserId}", userId);
            return new LanguagePreferenceResponse
            {
                Success = false,
                Error = "An error occurred while updating language preferences"
            };
        }
    }

    /// <inheritdoc />
    public async Task<List<SupportedLanguageDto>> GetSupportedLanguagesAsync(TranslationProvider? provider = null)
    {
        // In a real implementation, this would query the translation provider APIs
        // For now, return our static list with provider support information
        var languages = SupportedLanguages.Values.ToList();

        // Add provider support information
        foreach (var language in languages)
        {
            language.SupportedProviders = new List<TranslationProvider>
            {
                TranslationProvider.Azure,
                TranslationProvider.Google,
                TranslationProvider.AWS
            };

            if (language.IsCommon)
            {
                language.SupportedProviders.Add(TranslationProvider.DeepL);
                language.SupportedProviders.Add(TranslationProvider.OpenAI);
            }
        }

        if (provider.HasValue)
        {
            languages = languages.Where(l => l.SupportedProviders.Contains(provider.Value)).ToList();
        }

        await Task.CompletedTask; // For async signature
        return languages.OrderBy(l => l.Name).ToList();
    }

    /// <inheritdoc />
    public async Task<SupportedLanguageDto?> GetLanguageInfoAsync(string languageCode)
    {
        await Task.CompletedTask; // For async signature
        return SupportedLanguages.TryGetValue(languageCode.ToLowerInvariant(), out var language) ? language : null;
    }

    /// <inheritdoc />
    public async Task<TranslationStatsDto> GetTranslationStatsAsync(string userId, DateTimeOffset? fromDate = null, DateTimeOffset? toDate = null)
    {
        try
        {
            fromDate ??= DateTimeOffset.UtcNow.AddDays(-30);
            toDate ??= DateTimeOffset.UtcNow;

            var translations = await _context.MessageTranslations
                .Where(t => t.UserId == userId && t.CreatedAt >= fromDate && t.CreatedAt <= toDate)
                .ToListAsync();

            if (!translations.Any())
            {
                return new TranslationStatsDto
                {
                    UserId = userId,
                    FromDate = fromDate.Value,
                    ToDate = toDate.Value
                };
            }

            var stats = new TranslationStatsDto
            {
                UserId = userId,
                TotalTranslations = translations.Count,
                CachedTranslations = translations.Count(t => t.IsCached),
                AutomaticTranslations = translations.Count(t => t.TranslationMethod == "automatic"),
                ManualTranslations = translations.Count(t => t.TranslationMethod == "manual"),
                TotalCharactersTranslated = translations.Sum(t => t.CharacterCount),
                TotalTranslationCost = translations.Sum(t => t.TranslationCost),
                AverageConfidenceScore = translations.Where(t => t.ConfidenceScore.HasValue).Average(t => t.ConfidenceScore!.Value),
                AverageQualityRating = translations.Where(t => t.QualityRating.HasValue).DefaultIfEmpty().Average(t => t?.QualityRating ?? 0),
                CacheHitRatio = (double)translations.Count(t => t.IsCached) / translations.Count,
                FromDate = fromDate.Value,
                ToDate = toDate.Value
            };

            // Most used languages
            var sourceLanguages = translations.GroupBy(t => t.SourceLanguage).OrderByDescending(g => g.Count()).FirstOrDefault();
            var targetLanguages = translations.GroupBy(t => t.TargetLanguage).OrderByDescending(g => g.Count()).FirstOrDefault();

            stats.MostUsedSourceLanguage = sourceLanguages?.Key ?? "";
            stats.MostUsedTargetLanguage = targetLanguages?.Key ?? "";

            // Most used provider
            var providerGroups = translations.GroupBy(t => t.TranslationProvider).OrderByDescending(g => g.Count()).FirstOrDefault();
            if (providerGroups != null)
            {
                stats.MostUsedProvider = EnumConverter.ToTranslationProvider(providerGroups.Key);
            }

            // Daily stats
            stats.DailyStats = translations
                .GroupBy(t => DateOnly.FromDateTime(t.CreatedAt.Date))
                .Select(g => new TranslationDayStatsDto
                {
                    Date = g.Key,
                    TranslationCount = g.Count(),
                    CharactersTranslated = g.Sum(t => t.CharacterCount),
                    TranslationCost = g.Sum(t => t.TranslationCost),
                    CacheHits = g.Count(t => t.IsCached)
                })
                .OrderBy(ds => ds.Date)
                .ToList();

            // Language pairs
            stats.TopLanguagePairs = translations
                .GroupBy(t => new { t.SourceLanguage, t.TargetLanguage })
                .Select(g => new LanguagePairStatsDto
                {
                    SourceLanguage = g.Key.SourceLanguage,
                    TargetLanguage = g.Key.TargetLanguage,
                    TranslationCount = g.Count(),
                    AverageConfidence = g.Where(t => t.ConfidenceScore.HasValue).Average(t => t.ConfidenceScore!.Value),
                    AverageQuality = g.Where(t => t.QualityRating.HasValue).DefaultIfEmpty().Average(t => t?.QualityRating ?? 0),
                    LastUsed = g.Max(t => t.CreatedAt)
                })
                .OrderByDescending(lp => lp.TranslationCount)
                .Take(10)
                .ToList();

            // Add language names
            foreach (var pair in stats.TopLanguagePairs)
            {
                var sourceInfo = await GetLanguageInfoAsync(pair.SourceLanguage);
                var targetInfo = await GetLanguageInfoAsync(pair.TargetLanguage);
                pair.SourceLanguageName = sourceInfo?.Name ?? pair.SourceLanguage;
                pair.TargetLanguageName = targetInfo?.Name ?? pair.TargetLanguage;
            }

            return stats;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting translation stats for user {UserId}", userId);
            return new TranslationStatsDto
            {
                UserId = userId,
                FromDate = fromDate ?? DateTimeOffset.UtcNow.AddDays(-30),
                ToDate = toDate ?? DateTimeOffset.UtcNow
            };
        }
    }

    /// <inheritdoc />
    public async Task<List<MessageTranslationDto>> GetRecentTranslationsAsync(string userId, int limit = 20)
    {
        try
        {
            var translations = await _context.MessageTranslations
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .Take(limit)
                .ToListAsync();

            var translationDtos = new List<MessageTranslationDto>();
            foreach (var translation in translations)
            {
                translationDtos.Add(await MapToTranslationDto(translation));
            }

            return translationDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting recent translations for user {UserId}", userId);
            return new List<MessageTranslationDto>();
        }
    }

    /// <inheritdoc />
    public async Task<int> ClearTranslationCacheAsync(int olderThanDays = 30)
    {
        try
        {
            var cutoffDate = DateTimeOffset.UtcNow.AddDays(-olderThanDays);

            var expiredEntries = await _context.TranslationCache
                .Where(tc => tc.LastUsedAt < cutoffDate || (tc.ExpiresAt.HasValue && tc.ExpiresAt < DateTimeOffset.UtcNow))
                .ToListAsync();

            var count = expiredEntries.Count;

            _context.TranslationCache.RemoveRange(expiredEntries);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Cleared {Count} expired translation cache entries", count);
            return count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing translation cache");
            return 0;
        }
    }

    /// <inheritdoc />
    public async Task<List<TranslationSuggestionDto>> GetTranslationSuggestionsAsync(string userId, string conversationId, int limit = 5)
    {
        try
        {
            // Get user's language preferences
            var preferences = await GetLanguagePreferenceAsync(userId);
            
            // Get recent translations in this conversation
            var recentTranslations = await _context.MessageTranslations
                .Where(t => t.UserId == userId && t.Message.ConversationId == conversationId)
                .GroupBy(t => t.TargetLanguage)
                .Select(g => new TranslationSuggestionDto
                {
                    LanguageCode = g.Key,
                    MessageCount = g.Count(),
                    LastUsed = g.Max(t => t.CreatedAt),
                    Reason = "Previously used in this conversation",
                    ConfidenceScore = 0.8
                })
                .OrderByDescending(s => s.LastUsed)
                .Take(limit)
                .ToListAsync();

            // Add language names and user preference info
            foreach (var suggestion in recentTranslations)
            {
                var languageInfo = await GetLanguageInfoAsync(suggestion.LanguageCode);
                suggestion.LanguageName = languageInfo?.Name ?? suggestion.LanguageCode;
                
                if (preferences != null)
                {
                    suggestion.IsUserPreferred = preferences.PrimaryLanguage == suggestion.LanguageCode ||
                                               preferences.SecondaryLanguages.Contains(suggestion.LanguageCode);
                }
            }

            // Add user's preferred languages if not already included
            if (preferences != null)
            {
                var preferredLanguages = new[] { preferences.PrimaryLanguage }
                    .Concat(preferences.SecondaryLanguages)
                    .Where(lang => !recentTranslations.Any(rt => rt.LanguageCode == lang))
                    .Take(limit - recentTranslations.Count)
                    .Select(lang => new TranslationSuggestionDto
                    {
                        LanguageCode = lang,
                        LanguageName = GetLanguageInfoAsync(lang).Result?.Name ?? lang,
                        Reason = "From your language preferences",
                        ConfidenceScore = 0.9,
                        IsUserPreferred = true,
                        MessageCount = 0
                    });

                recentTranslations.AddRange(preferredLanguages);
            }

            return recentTranslations.OrderByDescending(s => s.ConfidenceScore).Take(limit).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting translation suggestions for user {UserId}", userId);
            return new List<TranslationSuggestionDto>();
        }
    }

    /// <inheritdoc />
    public async Task<List<TranslationResponse>> BatchTranslateMessagesAsync(string userId, List<string> messageIds, string targetLanguage, string? sourceLanguage = null)
    {
        var responses = new List<TranslationResponse>();

        foreach (var messageId in messageIds)
        {
            var request = new TranslateMessageRequest
            {
                MessageId = messageId,
                TargetLanguage = targetLanguage,
                SourceLanguage = sourceLanguage,
                UseCache = true,
                CacheResult = true
            };

            var response = await TranslateMessageAsync(userId, request);
            responses.Add(response);

            // Add small delay to avoid overwhelming translation services
            await Task.Delay(100);
        }

        _logger.LogInformation("Batch translated {Count} messages for user {UserId}", messageIds.Count, userId);
        return responses;
    }

    /// <inheritdoc />
    public async Task<TranslationCacheStatsDto> GetCacheStatsAsync()
    {
        try
        {
            var cacheEntries = await _context.TranslationCache.ToListAsync();

            if (!cacheEntries.Any())
            {
                return new TranslationCacheStatsDto
                {
                    CalculatedAt = DateTimeOffset.UtcNow
                };
            }

            var stats = new TranslationCacheStatsDto
            {
                TotalCacheEntries = cacheEntries.Count,
                ActiveCacheEntries = cacheEntries.Count(c => c.IsActive),
                TotalCacheHits = cacheEntries.Sum(c => c.UsageCount),
                TotalCharactersCached = cacheEntries.Sum(c => c.CharacterCount),
                AverageCacheQuality = cacheEntries.Where(c => c.AverageQualityRating.HasValue)
                                                 .DefaultIfEmpty()
                                                 .Average(c => c?.AverageQualityRating ?? 0),
                CalculatedAt = DateTimeOffset.UtcNow
            };

            stats.CacheHitRatio = stats.TotalCacheEntries > 0 ? (double)stats.TotalCacheHits / stats.TotalCacheEntries : 0;

            // Top language pairs in cache
            stats.TopLanguagePairs = cacheEntries
                .GroupBy(c => new { c.SourceLanguage, c.TargetLanguage })
                .Select(g => new CacheLanguagePairDto
                {
                    SourceLanguage = g.Key.SourceLanguage,
                    TargetLanguage = g.Key.TargetLanguage,
                    CacheEntries = g.Count(),
                    UsageCount = g.Sum(c => c.UsageCount),
                    AverageQuality = g.Where(c => c.AverageQualityRating.HasValue).DefaultIfEmpty().Average(c => c?.AverageQualityRating ?? 0)
                })
                .OrderByDescending(lp => lp.UsageCount)
                .Take(10)
                .ToList();

            // Provider stats
            stats.ProviderStats = cacheEntries
                .GroupBy(c => c.TranslationProvider)
                .Select(g => new CacheProviderStatsDto
                {
                    Provider = g.Key,
                    CacheEntries = g.Count(),
                    UsageCount = g.Sum(c => c.UsageCount),
                    AverageQuality = g.Where(c => c.AverageQualityRating.HasValue).DefaultIfEmpty().Average(c => c?.AverageQualityRating ?? 0)
                })
                .OrderByDescending(ps => ps.UsageCount)
                .ToList();

            return stats;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cache statistics");
            return new TranslationCacheStatsDto
            {
                CalculatedAt = DateTimeOffset.UtcNow
            };
        }
    }

    #region Private Helper Methods

    /// <summary>
    /// Internal method to translate text using available providers
    /// </summary>
    private async Task<InternalTranslationResult> TranslateTextInternalAsync(string text, string targetLanguage, 
        string? sourceLanguage, TranslationProvider? preferredProvider, bool useCache)
    {
        try
        {
            // Check cache first if enabled
            if (useCache)
            {
                var cachedTranslation = await GetFromCacheAsync(text, sourceLanguage, targetLanguage);
                if (cachedTranslation != null)
                {
                    return cachedTranslation;
                }
            }

            // Auto-detect source language if not provided
            if (string.IsNullOrEmpty(sourceLanguage))
            {
                var detection = await DetectLanguageAsync(text);
                if (detection.Success && !string.IsNullOrEmpty(detection.LanguageCode))
                {
                    sourceLanguage = detection.LanguageCode;
                }
                else
                {
                    sourceLanguage = "auto"; // Fallback
                }
            }

            // Select translation provider
            var provider = preferredProvider ?? TranslationProvider.Azure;

            // Perform actual translation (this would integrate with real translation APIs)
            var translatedText = await CallTranslationProviderAsync(text, sourceLanguage, targetLanguage, provider);

            var result = new InternalTranslationResult
            {
                Success = true,
                SourceLanguage = sourceLanguage,
                TranslatedText = translatedText,
                ConfidenceScore = 0.85, // Mock confidence score
                Provider = provider,
                Cost = CalculateTranslationCost(text.Length, provider),
                IsCached = false
            };

            // Cache the result if enabled
            if (useCache)
            {
                await AddToCacheAsync(text, sourceLanguage, targetLanguage, translatedText, provider, 0.85);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in internal translation");
            return new InternalTranslationResult
            {
                Success = false,
                Error = "Translation failed"
            };
        }
    }

    /// <summary>
    /// Mock translation provider call (would integrate with real APIs)
    /// </summary>
    private async Task<string> CallTranslationProviderAsync(string text, string sourceLanguage, string targetLanguage, TranslationProvider provider)
    {
        // Simulate API call delay
        await Task.Delay(200);

        // Mock translation - in reality this would call Azure Translator, Google Translate, etc.
        return $"[{provider} Translation from {sourceLanguage} to {targetLanguage}]: {text}";
    }

    /// <summary>
    /// Calculate mock translation cost
    /// </summary>
    private decimal CalculateTranslationCost(int characterCount, TranslationProvider provider)
    {
        // Mock pricing per provider (per 1M characters)
        var pricePerMillion = provider switch
        {
            TranslationProvider.Azure => 10.00m,
            TranslationProvider.Google => 20.00m,
            TranslationProvider.AWS => 15.00m,
            TranslationProvider.DeepL => 25.00m,
            TranslationProvider.OpenAI => 30.00m,
            _ => 10.00m
        };

        return (decimal)characterCount / 1_000_000 * pricePerMillion;
    }

    /// <summary>
    /// Get translation from cache
    /// </summary>
    private async Task<InternalTranslationResult?> GetFromCacheAsync(string text, string? sourceLanguage, string targetLanguage)
    {
        try
        {
            var textHash = ComputeTextHash(text);

            var query = _context.TranslationCache
                .Where(tc => tc.TextHash == textHash && tc.TargetLanguage == targetLanguage && tc.IsActive);

            if (!string.IsNullOrEmpty(sourceLanguage))
            {
                query = query.Where(tc => tc.SourceLanguage == sourceLanguage);
            }

            var cacheEntry = await query.FirstOrDefaultAsync();

            if (cacheEntry != null)
            {
                // Update usage statistics
                cacheEntry.UsageCount++;
                cacheEntry.LastUsedAt = DateTimeOffset.UtcNow;
                await _context.SaveChangesAsync();

                return new InternalTranslationResult
                {
                    Success = true,
                    SourceLanguage = cacheEntry.SourceLanguage,
                    TranslatedText = cacheEntry.TranslatedText,
                    ConfidenceScore = cacheEntry.ConfidenceScore,
                    Provider = EnumConverter.ToTranslationProvider(cacheEntry.TranslationProvider),
                    Cost = 0, // No cost for cached translations
                    IsCached = true
                };
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting translation from cache");
            return null;
        }
    }

    /// <summary>
    /// Add translation to cache
    /// </summary>
    private async Task AddToCacheAsync(string text, string sourceLanguage, string targetLanguage, 
        string translatedText, TranslationProvider provider, double confidence)
    {
        try
        {
            var textHash = ComputeTextHash(text);

            var cacheEntry = new TranslationCache
            {
                Id = Guid.NewGuid().ToString(),
                TextHash = textHash,
                SourceLanguage = sourceLanguage,
                TargetLanguage = targetLanguage,
                SourceText = text,
                TranslatedText = translatedText,
                ConfidenceScore = confidence,
                TranslationProvider = EnumConverter.ToString(provider),
                CharacterCount = text.Length,
                UsageCount = 1,
                CreatedAt = DateTimeOffset.UtcNow,
                LastUsedAt = DateTimeOffset.UtcNow,
                IsActive = true
            };

            _context.TranslationCache.Add(cacheEntry);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding translation to cache");
            // Non-critical, don't throw
        }
    }

    /// <summary>
    /// Update cache entry quality rating
    /// </summary>
    private async Task UpdateCacheQualityRatingAsync(string text, string sourceLanguage, string targetLanguage, int rating)
    {
        try
        {
            var textHash = ComputeTextHash(text);

            var cacheEntry = await _context.TranslationCache
                .FirstOrDefaultAsync(tc => tc.TextHash == textHash && 
                                         tc.SourceLanguage == sourceLanguage && 
                                         tc.TargetLanguage == targetLanguage);

            if (cacheEntry != null)
            {
                var currentTotal = (cacheEntry.AverageQualityRating ?? 0) * cacheEntry.QualityRatingCount;
                cacheEntry.QualityRatingCount++;
                cacheEntry.AverageQualityRating = (currentTotal + rating) / cacheEntry.QualityRatingCount;

                await _context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating cache quality rating");
            // Non-critical, don't throw
        }
    }

    /// <summary>
    /// Compute hash for text caching
    /// </summary>
    private static string ComputeTextHash(string text)
    {
        using (var sha256 = SHA256.Create())
        {
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(text));
            return Convert.ToBase64String(bytes);
        }
    }

    /// <summary>
    /// Simple language detection heuristic (would use real APIs in production)
    /// </summary>
    private (string Code, string Name, double Confidence, List<LanguageDetectionOption> Alternatives) DetectLanguageHeuristic(string text)
    {
        // Very basic heuristic - in reality would use proper language detection APIs
        var alternatives = new List<LanguageDetectionOption>();

        if (text.Any(c => c >= 0x4E00 && c <= 0x9FFF)) // Chinese characters
        {
            alternatives.Add(new LanguageDetectionOption { LanguageCode = "ja", LanguageName = "Japanese", ConfidenceScore = 0.3 });
            return ("zh", "Chinese", 0.8, alternatives);
        }

        if (text.Any(c => c >= 0x3040 && c <= 0x309F) || text.Any(c => c >= 0x30A0 && c <= 0x30FF)) // Japanese characters
        {
            alternatives.Add(new LanguageDetectionOption { LanguageCode = "zh", LanguageName = "Chinese", ConfidenceScore = 0.2 });
            return ("ja", "Japanese", 0.9, alternatives);
        }

        if (text.Any(c => c >= 0xAC00 && c <= 0xD7AF)) // Korean characters
        {
            return ("ko", "Korean", 0.95, alternatives);
        }

        if (text.Any(c => c >= 0x0600 && c <= 0x06FF)) // Arabic characters
        {
            alternatives.Add(new LanguageDetectionOption { LanguageCode = "fa", LanguageName = "Persian", ConfidenceScore = 0.1 });
            return ("ar", "Arabic", 0.85, alternatives);
        }

        if (text.Any(c => c >= 0x0400 && c <= 0x04FF)) // Cyrillic characters
        {
            alternatives.Add(new LanguageDetectionOption { LanguageCode = "uk", LanguageName = "Ukrainian", ConfidenceScore = 0.2 });
            return ("ru", "Russian", 0.8, alternatives);
        }

        // Default to English for basic Latin text
        alternatives.Add(new LanguageDetectionOption { LanguageCode = "es", LanguageName = "Spanish", ConfidenceScore = 0.4 });
        alternatives.Add(new LanguageDetectionOption { LanguageCode = "fr", LanguageName = "French", ConfidenceScore = 0.3 });
        return ("en", "English", 0.7, alternatives);
    }

    /// <summary>
    /// Map entity to DTO
    /// </summary>
    private async Task<MessageTranslationDto> MapToTranslationDto(MessageTranslation translation)
    {
        var sourceLanguage = await GetLanguageInfoAsync(translation.SourceLanguage);
        var targetLanguage = await GetLanguageInfoAsync(translation.TargetLanguage);

        return new MessageTranslationDto
        {
            Id = translation.Id,
            MessageId = translation.MessageId,
            UserId = translation.UserId,
            SourceLanguage = translation.SourceLanguage,
            SourceLanguageName = sourceLanguage?.Name ?? translation.SourceLanguage,
            TargetLanguage = translation.TargetLanguage,
            TargetLanguageName = targetLanguage?.Name ?? translation.TargetLanguage,
            SourceText = translation.SourceText,
            TranslatedText = translation.TranslatedText,
            ConfidenceScore = translation.ConfidenceScore,
            TranslationProvider = EnumConverter.ToTranslationProvider(translation.TranslationProvider),
            TranslationMethod = EnumConverter.ToTranslationMethod(translation.TranslationMethod),
            IsCached = translation.IsCached,
            CharacterCount = translation.CharacterCount,
            TranslationCost = translation.TranslationCost,
            QualityRating = translation.QualityRating,
            QualityFeedback = translation.QualityFeedback,
            CreatedAt = translation.CreatedAt,
            LastAccessedAt = translation.LastAccessedAt,
            AccessCount = translation.AccessCount
        };
    }

    /// <summary>
    /// Map entity to DTO
    /// </summary>
    private async Task<LanguagePreferenceDto> MapToLanguagePreferenceDto(UserLanguagePreference preference)
    {
        var primaryLanguage = await GetLanguageInfoAsync(preference.PrimaryLanguage);
        var secondaryLanguages = !string.IsNullOrEmpty(preference.SecondaryLanguages) 
            ? preference.SecondaryLanguages.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
            : new List<string>();

        var secondaryLanguageNames = new List<string>();
        foreach (var lang in secondaryLanguages)
        {
            var langInfo = await GetLanguageInfoAsync(lang);
            secondaryLanguageNames.Add(langInfo?.Name ?? lang);
        }

        var excludeLanguages = !string.IsNullOrEmpty(preference.ExcludeLanguages)
            ? preference.ExcludeLanguages.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
            : new List<string>();

        return new LanguagePreferenceDto
        {
            Id = preference.Id,
            UserId = preference.UserId,
            PrimaryLanguage = preference.PrimaryLanguage,
            PrimaryLanguageName = primaryLanguage?.Name ?? preference.PrimaryLanguage,
            SecondaryLanguages = secondaryLanguages,
            SecondaryLanguageNames = secondaryLanguageNames,
            AutoTranslateIncoming = preference.AutoTranslateIncoming,
            AutoDetectOutgoing = preference.AutoDetectOutgoing,
            PreferredProvider = EnumConverter.ToTranslationProvider(preference.PreferredProvider),
            MinConfidenceThreshold = preference.MinConfidenceThreshold,
            ShowConfidenceScores = preference.ShowConfidenceScores,
            EnableTranslationCache = preference.EnableTranslationCache,
            ExcludeLanguages = excludeLanguages,
            EnableSuggestions = preference.EnableSuggestions,
            IsActive = preference.IsActive,
            CreatedAt = preference.CreatedAt,
            UpdatedAt = preference.UpdatedAt
        };
    }

    #endregion
}

/// <summary>
/// Internal translation result for processing
/// </summary>
internal class InternalTranslationResult
{
    public bool Success { get; set; }
    public string? SourceLanguage { get; set; }
    public string? TranslatedText { get; set; }
    public double? ConfidenceScore { get; set; }
    public TranslationProvider Provider { get; set; }
    public decimal Cost { get; set; }
    public bool IsCached { get; set; }
    public string? Error { get; set; }
}