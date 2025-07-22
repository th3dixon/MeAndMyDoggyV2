using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MeAndMyDog.API.Services.Interfaces;
using System.Security.Claims;

namespace MeAndMyDog.API.Controllers;

/// <summary>
/// Controller for file upload operations
/// </summary>
[ApiController]
[Route("api/v1/files")]
[Authorize]
public class FileUploadController : ControllerBase
{
    private readonly IFileUploadService _fileUploadService;
    private readonly ILogger<FileUploadController> _logger;
    
    /// <summary>
    /// Initialize the file upload controller
    /// </summary>
    public FileUploadController(IFileUploadService fileUploadService, ILogger<FileUploadController> logger)
    {
        _fileUploadService = fileUploadService;
        _logger = logger;
    }
    
    /// <summary>
    /// Upload a single file
    /// </summary>
    /// <param name="file">File to upload</param>
    /// <param name="messageId">Optional message ID to attach file to</param>
    /// <returns>Upload result</returns>
    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile(IFormFile file, [FromQuery] string? messageId = null)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not found");
            }
            
            var result = await _fileUploadService.UploadFileAsync(file, userId, messageId);
            
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(new { error = result.ErrorMessage });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in file upload endpoint");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }
    
    /// <summary>
    /// Upload multiple files
    /// </summary>
    /// <param name="files">Files to upload</param>
    /// <param name="messageId">Optional message ID to attach files to</param>
    /// <returns>List of upload results</returns>
    [HttpPost("upload-multiple")]
    public async Task<IActionResult> UploadMultipleFiles(IFormFile[] files, [FromQuery] string? messageId = null)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not found");
            }
            
            if (files == null || files.Length == 0)
            {
                return BadRequest(new { error = "No files provided" });
            }
            
            if (files.Length > 10)
            {
                return BadRequest(new { error = "Maximum 10 files allowed per upload" });
            }
            
            var results = await _fileUploadService.UploadMultipleFilesAsync(files, userId, messageId);
            
            return Ok(new { results });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in multiple file upload endpoint");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }
    
    /// <summary>
    /// Validate file before upload
    /// </summary>
    /// <param name="file">File to validate</param>
    /// <returns>Validation result</returns>
    [HttpPost("validate")]
    public async Task<IActionResult> ValidateFile(IFormFile file)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not found");
            }
            
            var result = await _fileUploadService.ValidateFileAsync(file, userId);
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in file validation endpoint");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }
    
    /// <summary>
    /// Delete uploaded file
    /// </summary>
    /// <param name="fileUrl">URL of file to delete</param>
    /// <returns>Success result</returns>
    [HttpDelete]
    public async Task<IActionResult> DeleteFile([FromQuery] string fileUrl)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not found");
            }
            
            if (string.IsNullOrEmpty(fileUrl))
            {
                return BadRequest(new { error = "File URL is required" });
            }
            
            var success = await _fileUploadService.DeleteFileAsync(fileUrl, userId);
            
            if (success)
            {
                return Ok(new { message = "File deleted successfully" });
            }
            else
            {
                return NotFound(new { error = "File not found or access denied" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in file deletion endpoint");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }
    
    /// <summary>
    /// Get file usage statistics for current user
    /// </summary>
    /// <returns>Usage statistics</returns>
    [HttpGet("usage")]
    public async Task<IActionResult> GetUsageStats()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not found");
            }
            
            var stats = await _fileUploadService.GetFileUsageStatsAsync(userId);
            
            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in usage stats endpoint");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }
}