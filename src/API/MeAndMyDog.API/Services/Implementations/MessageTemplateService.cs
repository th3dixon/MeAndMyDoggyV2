using Microsoft.EntityFrameworkCore;
using MeAndMyDog.API.Data;
using MeAndMyDog.API.Models.DTOs;
using MeAndMyDog.API.Models.Entities;
using MeAndMyDog.API.Models.Enums;
using MeAndMyDog.API.Services.Interfaces;
using MeAndMyDog.API.Services.Helpers;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace MeAndMyDog.API.Services.Implementations;

/// <summary>
/// Service implementation for message template operations
/// </summary>
public class MessageTemplateService : IMessageTemplateService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<MessageTemplateService> _logger;

    // Regex pattern to find template variables like {{variable}}
    private static readonly Regex VariablePattern = new(@"\{\{(\w+)\}\}", RegexOptions.Compiled);

    /// <summary>
    /// Initialize the message template service
    /// </summary>
    public MessageTemplateService(ApplicationDbContext context, ILogger<MessageTemplateService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<TemplateResponse> CreateTemplateAsync(string userId, CreateMessageTemplateRequest request)
    {
        try
        {
            // Validate request
            var validationResult = await ValidateTemplateAsync(request.Content, request.Variables);
            if (!validationResult.IsValid)
            {
                return new TemplateResponse
                {
                    Success = false,
                    Message = $"Template validation failed: {string.Join(", ", validationResult.Errors)}"
                };
            }

            // Check for duplicate name
            var existingTemplate = await _context.MessageTemplates
                .FirstOrDefaultAsync(t => t.UserId == userId && t.Name == request.Name && t.IsActive);

            if (existingTemplate != null)
            {
                return new TemplateResponse
                {
                    Success = false,
                    Message = "A template with this name already exists"
                };
            }

            // Create template
            var template = new MessageTemplate
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                Name = request.Name,
                Description = request.Description,
                Category = EnumConverter.ToString(
                    Enum.TryParse<TemplateCategory>(request.Category, true, out var category) 
                    ? category : TemplateCategory.General),
                Content = request.Content,
                Variables = request.Variables?.Any() == true ? JsonSerializer.Serialize(request.Variables) : null,
                IsActive = true,
                IsShared = request.IsShared,
                IsSystemTemplate = false,
                Language = request.Language ?? "en",
                UsageCount = 0,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            };

            _context.MessageTemplates.Add(template);
            await _context.SaveChangesAsync();

            var templateDto = MapToTemplateDto(template);

            _logger.LogInformation("Message template {TemplateId} created successfully for user {UserId}", 
                template.Id, userId);

            return new TemplateResponse
            {
                Success = true,
                Message = "Template created successfully",
                TemplateId = template.Id,
                Template = templateDto
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating template for user {UserId}", userId);
            return new TemplateResponse
            {
                Success = false,
                Message = "An error occurred while creating the template"
            };
        }
    }

    /// <inheritdoc />
    public async Task<TemplateResponse> UpdateTemplateAsync(string userId, string templateId, UpdateMessageTemplateRequest request)
    {
        try
        {
            var template = await _context.MessageTemplates
                .FirstOrDefaultAsync(t => t.Id == templateId && t.UserId == userId);

            if (template == null)
            {
                return new TemplateResponse
                {
                    Success = false,
                    Message = "Template not found"
                };
            }

            if (template.IsSystemTemplate)
            {
                return new TemplateResponse
                {
                    Success = false,
                    Message = "System templates cannot be modified"
                };
            }

            // Validate updated content
            var validationResult = await ValidateTemplateAsync(request.Content, request.Variables);
            if (!validationResult.IsValid)
            {
                return new TemplateResponse
                {
                    Success = false,
                    Message = $"Template validation failed: {string.Join(", ", validationResult.Errors)}"
                };
            }

            // Check for duplicate name (excluding current template)
            if (request.Name != template.Name)
            {
                var existingTemplate = await _context.MessageTemplates
                    .FirstOrDefaultAsync(t => t.UserId == userId && t.Name == request.Name && 
                                            t.Id != templateId && t.IsActive);

                if (existingTemplate != null)
                {
                    return new TemplateResponse
                    {
                        Success = false,
                        Message = "A template with this name already exists"
                    };
                }
            }

            // Update template
            template.Name = request.Name;
            template.Description = request.Description;
            template.Category = EnumConverter.ToString(
                Enum.TryParse<TemplateCategory>(request.Category, true, out var category) 
                ? category : TemplateCategory.General);
            template.Content = request.Content;
            template.Variables = request.Variables?.Any() == true ? JsonSerializer.Serialize(request.Variables) : null;
            template.IsActive = request.IsActive;
            template.IsShared = request.IsShared;
            template.Language = request.Language ?? "en";
            template.UpdatedAt = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();

            var templateDto = MapToTemplateDto(template);

            _logger.LogInformation("Message template {TemplateId} updated successfully", templateId);

            return new TemplateResponse
            {
                Success = true,
                Message = "Template updated successfully",
                TemplateId = templateId,
                Template = templateDto
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating template {TemplateId}", templateId);
            return new TemplateResponse
            {
                Success = false,
                Message = "An error occurred while updating the template"
            };
        }
    }

    /// <inheritdoc />
    public async Task<bool> DeleteTemplateAsync(string userId, string templateId)
    {
        try
        {
            var template = await _context.MessageTemplates
                .FirstOrDefaultAsync(t => t.Id == templateId && t.UserId == userId);

            if (template == null)
            {
                return false;
            }

            if (template.IsSystemTemplate)
            {
                _logger.LogWarning("Attempt to delete system template {TemplateId}", templateId);
                return false;
            }

            // Soft delete by marking as inactive
            template.IsActive = false;
            template.UpdatedAt = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Message template {TemplateId} deleted successfully", templateId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting template {TemplateId}", templateId);
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<MessageTemplateDto?> GetTemplateAsync(string userId, string templateId)
    {
        try
        {
            var template = await _context.MessageTemplates
                .Where(t => t.Id == templateId && t.IsActive)
                .Where(t => t.UserId == userId || t.IsShared || t.IsSystemTemplate)
                .FirstOrDefaultAsync();

            return template != null ? MapToTemplateDto(template) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting template {TemplateId}", templateId);
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<List<MessageTemplateDto>> GetUserTemplatesAsync(string userId, TemplateCategory? category = null, 
        bool includeShared = true, bool includeSystem = true, int skip = 0, int take = 50)
    {
        try
        {
            var query = _context.MessageTemplates
                .Where(t => t.IsActive)
                .Where(t => t.UserId == userId || 
                           (includeShared && t.IsShared) || 
                           (includeSystem && t.IsSystemTemplate));

            if (category.HasValue)
            {
                var categoryString = EnumConverter.ToString(category.Value);
                query = query.Where(t => t.Category == categoryString);
            }

            var templates = await query
                .OrderByDescending(t => t.UsageCount)
                .ThenByDescending(t => t.UpdatedAt)
                .Skip(skip)
                .Take(take)
                .ToListAsync();

            return templates.Select(MapToTemplateDto).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting templates for user {UserId}", userId);
            return new List<MessageTemplateDto>();
        }
    }

    /// <inheritdoc />
    public async Task<List<MessageTemplateDto>> SearchTemplatesAsync(string userId, string query, 
        TemplateCategory? category = null, int skip = 0, int take = 20)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            {
                return new List<MessageTemplateDto>();
            }

            var searchQuery = _context.MessageTemplates
                .Where(t => t.IsActive)
                .Where(t => t.UserId == userId || t.IsShared || t.IsSystemTemplate)
                .Where(t => t.Name.Contains(query) || 
                           t.Description!.Contains(query) || 
                           t.Content.Contains(query));

            if (category.HasValue)
            {
                var categoryString = EnumConverter.ToString(category.Value);
                searchQuery = searchQuery.Where(t => t.Category == categoryString);
            }

            var templates = await searchQuery
                .OrderByDescending(t => t.UsageCount)
                .ThenBy(t => t.Name)
                .Skip(skip)
                .Take(take)
                .ToListAsync();

            return templates.Select(MapToTemplateDto).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching templates for user {UserId}", userId);
            return new List<MessageTemplateDto>();
        }
    }

    /// <inheritdoc />
    public async Task<string> ProcessTemplateAsync(string templateContent, Dictionary<string, string> variables)
    {
        return await Task.FromResult(ProcessTemplateVariables(templateContent, variables));
    }

    /// <inheritdoc />
    public async Task<TemplateValidationResult> ValidateTemplateAsync(string templateContent, List<TemplateVariableDto>? variables)
    {
        var result = new TemplateValidationResult();

        try
        {
            if (string.IsNullOrWhiteSpace(templateContent))
            {
                result.Errors.Add("Template content cannot be empty");
                return result;
            }

            // Detect variables in template
            var matches = VariablePattern.Matches(templateContent);
            var detectedVariables = matches.Select(m => m.Groups[1].Value).Distinct().ToList();
            result.DetectedVariables = detectedVariables;

            // Validate against defined variables
            if (variables?.Any() == true)
            {
                var definedVariables = variables.Select(v => v.Name).ToHashSet();
                
                // Check for undefined variables
                var undefinedVars = detectedVariables.Where(v => !definedVariables.Contains(v)).ToList();
                if (undefinedVars.Any())
                {
                    result.Warnings.Add($"Undefined variables found: {string.Join(", ", undefinedVars)}");
                }

                // Check for unused defined variables
                var unusedVars = definedVariables.Where(v => !detectedVariables.Contains(v)).ToList();
                if (unusedVars.Any())
                {
                    result.Warnings.Add($"Defined but unused variables: {string.Join(", ", unusedVars)}");
                }

                // Validate variable definitions
                foreach (var variable in variables)
                {
                    if (string.IsNullOrWhiteSpace(variable.Name))
                    {
                        result.Errors.Add("Variable name cannot be empty");
                    }
                    else if (!Regex.IsMatch(variable.Name, @"^[a-zA-Z_][a-zA-Z0-9_]*$"))
                    {
                        result.Errors.Add($"Variable '{variable.Name}' has invalid format. Use only letters, numbers, and underscores.");
                    }

                    if (variable.IsRequired && string.IsNullOrWhiteSpace(variable.DefaultValue))
                    {
                        // This is just a warning as the value will be provided at runtime
                        result.Warnings.Add($"Required variable '{variable.Name}' has no default value");
                    }
                }
            }
            else if (detectedVariables.Any())
            {
                result.Warnings.Add($"Variables found in template but no variable definitions provided: {string.Join(", ", detectedVariables)}");
            }

            // Check template length
            if (templateContent.Length > 10000)
            {
                result.Warnings.Add("Template is very long. Consider breaking it into smaller templates.");
            }

            result.IsValid = !result.Errors.Any();
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating template");
            result.Errors.Add("An error occurred during template validation");
            result.IsValid = false;
            return result;
        }
    }

    /// <inheritdoc />
    public async Task<TemplateUsageStatistics> GetTemplateUsageAsync(string userId, string templateId)
    {
        try
        {
            var template = await _context.MessageTemplates
                .FirstOrDefaultAsync(t => t.Id == templateId && t.UserId == userId);

            if (template == null)
            {
                return new TemplateUsageStatistics { TemplateId = templateId };
            }

            var now = DateTimeOffset.UtcNow;
            var thisMonth = now.AddDays(-30);
            var thisWeek = now.AddDays(-7);

            // Get scheduled messages using this template
            var scheduledMessages = await _context.ScheduledMessages
                .Where(sm => sm.TemplateId == templateId)
                .ToListAsync();

            var usageThisMonth = scheduledMessages.Count(sm => sm.CreatedAt >= thisMonth);
            var usageThisWeek = scheduledMessages.Count(sm => sm.CreatedAt >= thisWeek);

            var recentConversations = scheduledMessages
                .Where(sm => sm.SentAt >= thisWeek)
                .Select(sm => sm.ConversationId)
                .Distinct()
                .Take(10)
                .ToList();

            return new TemplateUsageStatistics
            {
                TemplateId = templateId,
                TotalUsage = template.UsageCount,
                UsageThisMonth = usageThisMonth,
                UsageThisWeek = usageThisWeek,
                LastUsed = template.LastUsedAt,
                RecentConversations = recentConversations
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting template usage for {TemplateId}", templateId);
            return new TemplateUsageStatistics { TemplateId = templateId };
        }
    }

    /// <inheritdoc />
    public async Task<TemplateResponse> DuplicateTemplateAsync(string userId, string templateId, string newName)
    {
        try
        {
            var template = await _context.MessageTemplates
                .FirstOrDefaultAsync(t => t.Id == templateId && 
                                        (t.UserId == userId || t.IsShared || t.IsSystemTemplate) && 
                                        t.IsActive);

            if (template == null)
            {
                return new TemplateResponse
                {
                    Success = false,
                    Message = "Template not found"
                };
            }

            // Check for duplicate name
            var existingTemplate = await _context.MessageTemplates
                .FirstOrDefaultAsync(t => t.UserId == userId && t.Name == newName && t.IsActive);

            if (existingTemplate != null)
            {
                return new TemplateResponse
                {
                    Success = false,
                    Message = "A template with this name already exists"
                };
            }

            // Create duplicate template
            var duplicateTemplate = new MessageTemplate
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                Name = newName,
                Description = template.Description,
                Category = template.Category,
                Content = template.Content,
                Variables = template.Variables,
                IsActive = true,
                IsShared = false, // New template is not shared by default
                IsSystemTemplate = false, // Duplicated templates are never system templates
                Language = template.Language,
                UsageCount = 0,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            };

            _context.MessageTemplates.Add(duplicateTemplate);
            await _context.SaveChangesAsync();

            var templateDto = MapToTemplateDto(duplicateTemplate);

            _logger.LogInformation("Template {TemplateId} duplicated as {NewTemplateId} for user {UserId}", 
                templateId, duplicateTemplate.Id, userId);

            return new TemplateResponse
            {
                Success = true,
                Message = "Template duplicated successfully",
                TemplateId = duplicateTemplate.Id,
                Template = templateDto
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error duplicating template {TemplateId}", templateId);
            return new TemplateResponse
            {
                Success = false,
                Message = "An error occurred while duplicating the template"
            };
        }
    }

    /// <inheritdoc />
    public async Task<List<MessageTemplateDto>> GetSystemTemplatesAsync(TemplateCategory? category = null, string language = "en")
    {
        try
        {
            var query = _context.MessageTemplates
                .Where(t => t.IsSystemTemplate && t.IsActive && t.Language == language);

            if (category.HasValue)
            {
                var categoryString = EnumConverter.ToString(category.Value);
                query = query.Where(t => t.Category == categoryString);
            }

            var templates = await query
                .OrderBy(t => t.Category)
                .ThenBy(t => t.Name)
                .ToListAsync();

            return templates.Select(MapToTemplateDto).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting system templates");
            return new List<MessageTemplateDto>();
        }
    }

    /// <summary>
    /// Process template variables in content
    /// </summary>
    private static string ProcessTemplateVariables(string templateContent, Dictionary<string, string> variables)
    {
        if (variables == null || !variables.Any())
        {
            return templateContent;
        }

        return VariablePattern.Replace(templateContent, match =>
        {
            var variableName = match.Groups[1].Value;
            return variables.TryGetValue(variableName, out var value) ? value : match.Value;
        });
    }

    /// <summary>
    /// Map entity to DTO
    /// </summary>
    private static MessageTemplateDto MapToTemplateDto(MessageTemplate template)
    {
        List<TemplateVariableDto>? variables = null;
        if (!string.IsNullOrEmpty(template.Variables))
        {
            try
            {
                variables = JsonSerializer.Deserialize<List<TemplateVariableDto>>(template.Variables);
            }
            catch
            {
                // Ignore deserialization errors
            }
        }

        return new MessageTemplateDto
        {
            Id = template.Id,
            UserId = template.UserId,
            Name = template.Name,
            Description = template.Description,
            Category = template.Category,
            Content = template.Content,
            Variables = variables,
            IsActive = template.IsActive,
            IsShared = template.IsShared,
            IsSystemTemplate = template.IsSystemTemplate,
            Language = template.Language,
            UsageCount = template.UsageCount,
            LastUsedAt = template.LastUsedAt,
            CreatedAt = template.CreatedAt,
            UpdatedAt = template.UpdatedAt
        };
    }

    /// <summary>
    /// Update template usage statistics
    /// </summary>
    public async Task UpdateTemplateUsageAsync(string templateId)
    {
        try
        {
            var template = await _context.MessageTemplates
                .FirstOrDefaultAsync(t => t.Id == templateId);

            if (template != null)
            {
                template.UsageCount++;
                template.LastUsedAt = DateTimeOffset.UtcNow;
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating template usage for {TemplateId}", templateId);
            // Don't throw as this is non-critical
        }
    }
}