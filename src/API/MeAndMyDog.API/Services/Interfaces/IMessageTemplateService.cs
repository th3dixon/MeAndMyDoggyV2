using MeAndMyDog.API.Models.DTOs;
using MeAndMyDog.API.Models.Enums;

namespace MeAndMyDog.API.Services.Interfaces;

/// <summary>
/// Service interface for message template operations
/// </summary>
public interface IMessageTemplateService
{
    /// <summary>
    /// Create a new message template
    /// </summary>
    /// <param name="userId">User ID creating the template</param>
    /// <param name="request">Template creation details</param>
    /// <returns>Created template response</returns>
    Task<TemplateResponse> CreateTemplateAsync(string userId, CreateMessageTemplateRequest request);

    /// <summary>
    /// Update an existing message template
    /// </summary>
    /// <param name="userId">User ID updating the template</param>
    /// <param name="templateId">Template ID to update</param>
    /// <param name="request">Template update details</param>
    /// <returns>Updated template response</returns>
    Task<TemplateResponse> UpdateTemplateAsync(string userId, string templateId, UpdateMessageTemplateRequest request);

    /// <summary>
    /// Delete a message template
    /// </summary>
    /// <param name="userId">User ID deleting the template</param>
    /// <param name="templateId">Template ID to delete</param>
    /// <returns>True if successfully deleted</returns>
    Task<bool> DeleteTemplateAsync(string userId, string templateId);

    /// <summary>
    /// Get a template by ID
    /// </summary>
    /// <param name="userId">User ID requesting the template</param>
    /// <param name="templateId">Template ID</param>
    /// <returns>Template details or null if not found</returns>
    Task<MessageTemplateDto?> GetTemplateAsync(string userId, string templateId);

    /// <summary>
    /// Get templates for a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="category">Optional category filter</param>
    /// <param name="includeShared">Whether to include shared templates</param>
    /// <param name="includeSystem">Whether to include system templates</param>
    /// <param name="skip">Number to skip for pagination</param>
    /// <param name="take">Number to take for pagination</param>
    /// <returns>List of templates</returns>
    Task<List<MessageTemplateDto>> GetUserTemplatesAsync(string userId, TemplateCategory? category = null, 
        bool includeShared = true, bool includeSystem = true, int skip = 0, int take = 50);

    /// <summary>
    /// Search templates by content or name
    /// </summary>
    /// <param name="userId">User ID searching</param>
    /// <param name="query">Search query</param>
    /// <param name="category">Optional category filter</param>
    /// <param name="skip">Number to skip for pagination</param>
    /// <param name="take">Number to take for pagination</param>
    /// <returns>List of matching templates</returns>
    Task<List<MessageTemplateDto>> SearchTemplatesAsync(string userId, string query, 
        TemplateCategory? category = null, int skip = 0, int take = 20);

    /// <summary>
    /// Process template content with variables
    /// </summary>
    /// <param name="templateContent">Template content with placeholders</param>
    /// <param name="variables">Variable values</param>
    /// <returns>Processed content with variables replaced</returns>
    Task<string> ProcessTemplateAsync(string templateContent, Dictionary<string, string> variables);

    /// <summary>
    /// Validate template content and variables
    /// </summary>
    /// <param name="templateContent">Template content to validate</param>
    /// <param name="variables">Template variables</param>
    /// <returns>Validation result with any errors</returns>
    Task<TemplateValidationResult> ValidateTemplateAsync(string templateContent, List<TemplateVariableDto>? variables);

    /// <summary>
    /// Get template usage statistics
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="templateId">Template ID</param>
    /// <returns>Usage statistics</returns>
    Task<TemplateUsageStatistics> GetTemplateUsageAsync(string userId, string templateId);

    /// <summary>
    /// Duplicate a template
    /// </summary>
    /// <param name="userId">User ID duplicating the template</param>
    /// <param name="templateId">Template ID to duplicate</param>
    /// <param name="newName">Name for the duplicated template</param>
    /// <returns>Duplicated template response</returns>
    Task<TemplateResponse> DuplicateTemplateAsync(string userId, string templateId, string newName);

    /// <summary>
    /// Get system default templates
    /// </summary>
    /// <param name="category">Optional category filter</param>
    /// <param name="language">Language filter</param>
    /// <returns>List of system templates</returns>
    Task<List<MessageTemplateDto>> GetSystemTemplatesAsync(TemplateCategory? category = null, string language = "en");
}