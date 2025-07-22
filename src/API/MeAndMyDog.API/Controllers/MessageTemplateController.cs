using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MeAndMyDog.API.Models.DTOs;
using MeAndMyDog.API.Models.Enums;
using MeAndMyDog.API.Services.Interfaces;
using System.Security.Claims;

namespace MeAndMyDog.API.Controllers;

/// <summary>
/// Controller for managing message templates
/// </summary>
[ApiController]
[Route("api/v1/templates")]
[Authorize]
public class MessageTemplateController : ControllerBase
{
    private readonly IMessageTemplateService _templateService;
    private readonly ILogger<MessageTemplateController> _logger;

    /// <summary>
    /// Initializes a new instance of MessageTemplateController
    /// </summary>
    public MessageTemplateController(
        IMessageTemplateService templateService,
        ILogger<MessageTemplateController> logger)
    {
        _templateService = templateService;
        _logger = logger;
    }

    /// <summary>
    /// Create a new message template
    /// </summary>
    /// <param name="request">Template creation details</param>
    /// <returns>Created template response</returns>
    [HttpPost]
    [ProducesResponseType(typeof(TemplateResponse), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> CreateTemplate([FromBody] CreateMessageTemplateRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var response = await _templateService.CreateTemplateAsync(userId, request);

            if (response.Success)
            {
                _logger.LogInformation("Template created successfully for user {UserId}", userId);
                return CreatedAtAction(nameof(GetTemplate), new { templateId = response.TemplateId }, response);
            }
            else
            {
                return BadRequest(response);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating template");
            return StatusCode(500, "An error occurred while creating the template");
        }
    }

    /// <summary>
    /// Update an existing message template
    /// </summary>
    /// <param name="templateId">Template ID to update</param>
    /// <param name="request">Template update details</param>
    /// <returns>Updated template response</returns>
    [HttpPut("{templateId}")]
    [ProducesResponseType(typeof(TemplateResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateTemplate(string templateId, [FromBody] UpdateMessageTemplateRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var response = await _templateService.UpdateTemplateAsync(userId, templateId, request);

            if (response.Success)
            {
                _logger.LogInformation("Template {TemplateId} updated successfully", templateId);
                return Ok(response);
            }
            else
            {
                if (response.Message.Contains("not found"))
                {
                    return NotFound(response);
                }
                return BadRequest(response);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating template {TemplateId}", templateId);
            return StatusCode(500, "An error occurred while updating the template");
        }
    }

    /// <summary>
    /// Delete a message template
    /// </summary>
    /// <param name="templateId">Template ID to delete</param>
    /// <returns>Success confirmation</returns>
    [HttpDelete("{templateId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteTemplate(string templateId)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var success = await _templateService.DeleteTemplateAsync(userId, templateId);
            if (success)
            {
                _logger.LogInformation("Template {TemplateId} deleted successfully", templateId);
                return NoContent();
            }
            else
            {
                return NotFound("Template not found");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting template {TemplateId}", templateId);
            return StatusCode(500, "An error occurred while deleting the template");
        }
    }

    /// <summary>
    /// Get a template by ID
    /// </summary>
    /// <param name="templateId">Template ID</param>
    /// <returns>Template details</returns>
    [HttpGet("{templateId}")]
    [ProducesResponseType(typeof(MessageTemplateDto), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetTemplate(string templateId)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var template = await _templateService.GetTemplateAsync(userId, templateId);
            if (template == null)
            {
                return NotFound("Template not found");
            }

            return Ok(template);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting template {TemplateId}", templateId);
            return StatusCode(500, "An error occurred while getting the template");
        }
    }

    /// <summary>
    /// Get templates for the current user
    /// </summary>
    /// <param name="category">Optional category filter</param>
    /// <param name="includeShared">Whether to include shared templates</param>
    /// <param name="includeSystem">Whether to include system templates</param>
    /// <param name="skip">Number to skip for pagination</param>
    /// <param name="take">Number to take for pagination</param>
    /// <returns>List of templates</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<MessageTemplateDto>), 200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> GetUserTemplates(
        [FromQuery] string? category = null,
        [FromQuery] bool includeShared = true,
        [FromQuery] bool includeSystem = true,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 50)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            // Validate pagination
            skip = Math.Max(0, skip);
            take = Math.Min(Math.Max(1, take), 100);

            TemplateCategory? categoryEnum = null;
            if (!string.IsNullOrEmpty(category) && Enum.TryParse<TemplateCategory>(category, true, out var parsed))
            {
                categoryEnum = parsed;
            }

            var templates = await _templateService.GetUserTemplatesAsync(userId, categoryEnum, includeShared, includeSystem, skip, take);
            return Ok(templates);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting templates for user");
            return StatusCode(500, "An error occurred while getting templates");
        }
    }

    /// <summary>
    /// Search templates by content or name
    /// </summary>
    /// <param name="query">Search query</param>
    /// <param name="category">Optional category filter</param>
    /// <param name="skip">Number to skip for pagination</param>
    /// <param name="take">Number to take for pagination</param>
    /// <returns>List of matching templates</returns>
    [HttpGet("search")]
    [ProducesResponseType(typeof(List<MessageTemplateDto>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> SearchTemplates(
        [FromQuery] string query,
        [FromQuery] string? category = null,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 20)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            {
                return BadRequest("Search query must be at least 2 characters long");
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            // Validate pagination
            skip = Math.Max(0, skip);
            take = Math.Min(Math.Max(1, take), 50);

            TemplateCategory? categoryEnum = null;
            if (!string.IsNullOrEmpty(category) && Enum.TryParse<TemplateCategory>(category, true, out var parsed))
            {
                categoryEnum = parsed;
            }

            var templates = await _templateService.SearchTemplatesAsync(userId, query, categoryEnum, skip, take);
            return Ok(templates);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching templates");
            return StatusCode(500, "An error occurred while searching templates");
        }
    }

    /// <summary>
    /// Process template content with variables
    /// </summary>
    /// <param name="request">Template processing request</param>
    /// <returns>Processed content</returns>
    [HttpPost("process")]
    [ProducesResponseType(typeof(ProcessTemplateResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> ProcessTemplate([FromBody] ProcessTemplateRequest request)
    {
        try
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(request.TemplateContent))
            {
                return BadRequest("Template content is required");
            }

            var processedContent = await _templateService.ProcessTemplateAsync(
                request.TemplateContent, request.Variables ?? new Dictionary<string, string>());

            var response = new ProcessTemplateResponse
            {
                Success = true,
                ProcessedContent = processedContent
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing template");
            return StatusCode(500, "An error occurred while processing the template");
        }
    }

    /// <summary>
    /// Validate template content and variables
    /// </summary>
    /// <param name="request">Template validation request</param>
    /// <returns>Validation result</returns>
    [HttpPost("validate")]
    [ProducesResponseType(typeof(TemplateValidationResult), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> ValidateTemplate([FromBody] ValidateTemplateRequest request)
    {
        try
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(request.TemplateContent))
            {
                return BadRequest("Template content is required");
            }

            var validationResult = await _templateService.ValidateTemplateAsync(
                request.TemplateContent, request.Variables);

            return Ok(validationResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating template");
            return StatusCode(500, "An error occurred while validating the template");
        }
    }

    /// <summary>
    /// Get template usage statistics
    /// </summary>
    /// <param name="templateId">Template ID</param>
    /// <returns>Usage statistics</returns>
    [HttpGet("{templateId}/usage")]
    [ProducesResponseType(typeof(TemplateUsageStatistics), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetTemplateUsage(string templateId)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var usage = await _templateService.GetTemplateUsageAsync(userId, templateId);
            return Ok(usage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting template usage for {TemplateId}", templateId);
            return StatusCode(500, "An error occurred while getting template usage");
        }
    }

    /// <summary>
    /// Duplicate a template
    /// </summary>
    /// <param name="templateId">Template ID to duplicate</param>
    /// <param name="request">Duplication request</param>
    /// <returns>Duplicated template response</returns>
    [HttpPost("{templateId}/duplicate")]
    [ProducesResponseType(typeof(TemplateResponse), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DuplicateTemplate(string templateId, [FromBody] DuplicateTemplateRequest request)
    {
        try
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(request.NewName))
            {
                return BadRequest("New template name is required");
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var response = await _templateService.DuplicateTemplateAsync(userId, templateId, request.NewName);

            if (response.Success)
            {
                _logger.LogInformation("Template {TemplateId} duplicated successfully", templateId);
                return CreatedAtAction(nameof(GetTemplate), new { templateId = response.TemplateId }, response);
            }
            else
            {
                if (response.Message.Contains("not found"))
                {
                    return NotFound(response);
                }
                return BadRequest(response);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error duplicating template {TemplateId}", templateId);
            return StatusCode(500, "An error occurred while duplicating the template");
        }
    }

    /// <summary>
    /// Get system default templates
    /// </summary>
    /// <param name="category">Optional category filter</param>
    /// <param name="language">Language filter</param>
    /// <returns>List of system templates</returns>
    [HttpGet("system")]
    [ProducesResponseType(typeof(List<MessageTemplateDto>), 200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> GetSystemTemplates(
        [FromQuery] string? category = null,
        [FromQuery] string language = "en")
    {
        try
        {
            TemplateCategory? categoryEnum = null;
            if (!string.IsNullOrEmpty(category) && Enum.TryParse<TemplateCategory>(category, true, out var parsed))
            {
                categoryEnum = parsed;
            }

            var templates = await _templateService.GetSystemTemplatesAsync(categoryEnum, language);
            return Ok(templates);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting system templates");
            return StatusCode(500, "An error occurred while getting system templates");
        }
    }
}