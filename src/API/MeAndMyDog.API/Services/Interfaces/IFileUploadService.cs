using MeAndMyDog.API.Models.DTOs;
using MeAndMyDog.API.Models.Enums;

namespace MeAndMyDog.API.Services.Interfaces;

/// <summary>
/// Service interface for file upload operations
/// </summary>
public interface IFileUploadService
{
    /// <summary>
    /// Upload a file and return upload information
    /// </summary>
    /// <param name="file">File to upload</param>
    /// <param name="userId">User uploading the file</param>
    /// <param name="messageId">Optional message ID if attaching to message</param>
    /// <returns>Upload result with URLs and metadata</returns>
    Task<FileUploadResult> UploadFileAsync(IFormFile file, string userId, string? messageId = null);
    
    /// <summary>
    /// Upload multiple files
    /// </summary>
    /// <param name="files">Files to upload</param>
    /// <param name="userId">User uploading the files</param>
    /// <param name="messageId">Optional message ID if attaching to message</param>
    /// <returns>List of upload results</returns>
    Task<List<FileUploadResult>> UploadMultipleFilesAsync(IFormFile[] files, string userId, string? messageId = null);
    
    /// <summary>
    /// Validate file before upload
    /// </summary>
    /// <param name="file">File to validate</param>
    /// <param name="userId">User uploading the file</param>
    /// <returns>Validation result</returns>
    Task<FileValidationResult> ValidateFileAsync(IFormFile file, string userId);
    
    /// <summary>
    /// Delete uploaded file
    /// </summary>
    /// <param name="fileUrl">URL of file to delete</param>
    /// <param name="userId">User requesting deletion</param>
    /// <returns>True if successfully deleted</returns>
    Task<bool> DeleteFileAsync(string fileUrl, string userId);
    
    /// <summary>
    /// Generate thumbnail for image or video
    /// </summary>
    /// <param name="fileUrl">Original file URL</param>
    /// <param name="userId">User requesting thumbnail</param>
    /// <returns>Thumbnail URL or null if not applicable</returns>
    Task<string?> GenerateThumbnailAsync(string fileUrl, string userId);
    
    /// <summary>
    /// Get file usage statistics for user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Usage statistics</returns>
    Task<FileUsageStats> GetFileUsageStatsAsync(string userId);
}