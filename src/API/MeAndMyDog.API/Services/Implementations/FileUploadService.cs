using Microsoft.EntityFrameworkCore;
using MeAndMyDog.API.Data;
using MeAndMyDog.API.Models.DTOs;
using MeAndMyDog.API.Models.Entities;
using MeAndMyDog.API.Models.Enums;
using MeAndMyDog.API.Services.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using System.Security.Cryptography;
using System.Text.Json;

namespace MeAndMyDog.API.Services.Implementations;

/// <summary>
/// Service implementation for file upload operations
/// </summary>
public class FileUploadService : IFileUploadService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<FileUploadService> _logger;
    private readonly IConfiguration _configuration;
    
    // File size limits in bytes
    private const long FREE_USER_FILE_LIMIT = 5 * 1024 * 1024; // 5MB
    private const long PREMIUM_USER_FILE_LIMIT = 50 * 1024 * 1024; // 50MB
    
    // Daily file limits
    private const int FREE_USER_DAILY_LIMIT = 10;
    private const int PREMIUM_USER_DAILY_LIMIT = 100;
    
    // Allowed file types
    private static readonly Dictionary<string, AttachmentType> AllowedMimeTypes = new()
    {
        // Images
        { "image/jpeg", AttachmentType.Image },
        { "image/jpg", AttachmentType.Image },
        { "image/png", AttachmentType.Image },
        { "image/gif", AttachmentType.Image },
        { "image/webp", AttachmentType.Image },
        
        // Videos
        { "video/mp4", AttachmentType.Video },
        { "video/mpeg", AttachmentType.Video },
        { "video/quicktime", AttachmentType.Video },
        { "video/webm", AttachmentType.Video },
        
        // Audio
        { "audio/mpeg", AttachmentType.Audio },
        { "audio/mp3", AttachmentType.Audio },
        { "audio/wav", AttachmentType.Audio },
        { "audio/ogg", AttachmentType.Audio },
        { "audio/webm", AttachmentType.Audio },
        
        // Documents
        { "application/pdf", AttachmentType.Document },
        { "application/msword", AttachmentType.Document },
        { "application/vnd.openxmlformats-officedocument.wordprocessingml.document", AttachmentType.Document },
        { "text/plain", AttachmentType.Document }
    };
    
    /// <summary>
    /// Initialize the file upload service
    /// </summary>
    public FileUploadService(ApplicationDbContext context, ILogger<FileUploadService> logger, IConfiguration configuration)
    {
        _context = context;
        _logger = logger;
        _configuration = configuration;
    }
    
    /// <inheritdoc />
    public async Task<FileUploadResult> UploadFileAsync(IFormFile file, string userId, string? messageId = null)
    {
        try
        {
            // Validate file
            var validation = await ValidateFileAsync(file, userId);
            if (!validation.IsValid)
            {
                return new FileUploadResult
                {
                    Success = false,
                    ErrorMessage = string.Join(", ", validation.Errors)
                };
            }
            
            // Check user limits
            var stats = await GetFileUsageStatsAsync(userId);
            if (stats.HasExceededLimits)
            {
                return new FileUploadResult
                {
                    Success = false,
                    ErrorMessage = "File upload limits exceeded. Please upgrade to premium or try again tomorrow."
                };
            }
            
            // Generate unique filename
            var fileExtension = Path.GetExtension(file.FileName);
            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
            
            // For now, save to local storage (in production this should be cloud storage)
            var uploadPath = Path.Combine("wwwroot", "uploads", DateTime.UtcNow.ToString("yyyy-MM"));
            Directory.CreateDirectory(uploadPath);
            
            var fullPath = Path.Combine(uploadPath, uniqueFileName);
            
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            
            // Generate file URL
            var fileUrl = $"/uploads/{DateTime.UtcNow:yyyy-MM}/{uniqueFileName}";
            
            // Generate thumbnail for images
            string? thumbnailUrl = null;
            int? width = null, height = null;
            
            if (validation.AttachmentType == AttachmentType.Image)
            {
                try
                {
                    using var image = Image.Load(fullPath);
                    width = image.Width;
                    height = image.Height;
                    
                    // Generate thumbnail
                    thumbnailUrl = await GenerateThumbnailAsync(fileUrl, userId);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to process image metadata for file {FileName}", file.FileName);
                }
            }
            
            // Create upload record in database
            var uploadRecord = new FileUploadRecord
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                MessageId = messageId,
                FileName = file.FileName,
                UniqueFileName = uniqueFileName,
                FileUrl = fileUrl,
                ThumbnailUrl = thumbnailUrl,
                FileSize = file.Length,
                MimeType = file.ContentType ?? "application/octet-stream",
                AttachmentType = validation.AttachmentType.ToString(),
                Width = width,
                Height = height,
                UploadedAt = DateTimeOffset.UtcNow
            };
            
            _context.FileUploadRecords.Add(uploadRecord);
            await _context.SaveChangesAsync();
            
            var result = new FileUploadResult
            {
                Success = true,
                FileUrl = fileUrl,
                ThumbnailUrl = thumbnailUrl,
                FileName = file.FileName,
                FileSize = file.Length,
                MimeType = file.ContentType,
                AttachmentType = validation.AttachmentType,
                Width = width,
                Height = height
            };
            
            _logger.LogInformation("File {FileName} uploaded successfully by user {UserId}", file.FileName, userId);
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file {FileName} for user {UserId}", file.FileName, userId);
            return new FileUploadResult
            {
                Success = false,
                ErrorMessage = "An error occurred while uploading the file. Please try again."
            };
        }
    }
    
    /// <inheritdoc />
    public async Task<List<FileUploadResult>> UploadMultipleFilesAsync(IFormFile[] files, string userId, string? messageId = null)
    {
        var results = new List<FileUploadResult>();
        
        foreach (var file in files)
        {
            var result = await UploadFileAsync(file, userId, messageId);
            results.Add(result);
        }
        
        return results;
    }
    
    /// <inheritdoc />
    public async Task<FileValidationResult> ValidateFileAsync(IFormFile file, string userId)
    {
        var result = new FileValidationResult();
        
        // Check if file is provided
        if (file == null || file.Length == 0)
        {
            result.IsValid = false;
            result.Errors.Add("No file provided or file is empty");
            return result;
        }
        
        // Check MIME type
        if (string.IsNullOrEmpty(file.ContentType) || !AllowedMimeTypes.ContainsKey(file.ContentType))
        {
            result.IsValid = false;
            result.Errors.Add($"File type '{file.ContentType}' is not supported");
            return result;
        }
        
        result.AttachmentType = AllowedMimeTypes[file.ContentType];
        
        // Check user subscription for file size limits
        var user = await _context.Users.FindAsync(userId);
        var isPremium = await IsUserPremium(userId);
        
        var fileSizeLimit = isPremium ? PREMIUM_USER_FILE_LIMIT : FREE_USER_FILE_LIMIT;
        
        if (file.Length > fileSizeLimit)
        {
            result.IsValid = false;
            result.Errors.Add($"File size {file.Length / (1024 * 1024)}MB exceeds limit of {fileSizeLimit / (1024 * 1024)}MB");
            result.RequiresPremium = !isPremium;
            return result;
        }
        
        // Check daily upload limits
        var stats = await GetFileUsageStatsAsync(userId);
        var dailyLimit = isPremium ? PREMIUM_USER_DAILY_LIMIT : FREE_USER_DAILY_LIMIT;
        
        if (stats.FilesToday >= dailyLimit)
        {
            result.IsValid = false;
            result.Errors.Add($"Daily file upload limit of {dailyLimit} files exceeded");
            result.RequiresPremium = !isPremium;
            return result;
        }
        
        // Basic security checks
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        var dangerousExtensions = new[] { ".exe", ".bat", ".cmd", ".com", ".scr", ".vbs", ".js" };
        
        if (dangerousExtensions.Contains(extension))
        {
            result.IsValid = false;
            result.Errors.Add("File type is not allowed for security reasons");
            return result;
        }
        
        result.IsValid = true;
        return result;
    }
    
    /// <inheritdoc />
    public async Task<bool> DeleteFileAsync(string fileUrl, string userId)
    {
        try
        {
            // Find upload record
            var uploadRecord = await _context.FileUploadRecords
                .FirstOrDefaultAsync(r => r.FileUrl == fileUrl && r.UserId == userId);
                
            if (uploadRecord == null)
            {
                return false;
            }
            
            // Delete physical file
            var physicalPath = Path.Combine("wwwroot", fileUrl.TrimStart('/'));
            if (File.Exists(physicalPath))
            {
                File.Delete(physicalPath);
            }
            
            // Delete thumbnail if exists
            if (!string.IsNullOrEmpty(uploadRecord.ThumbnailUrl))
            {
                var thumbnailPath = Path.Combine("wwwroot", uploadRecord.ThumbnailUrl.TrimStart('/'));
                if (File.Exists(thumbnailPath))
                {
                    File.Delete(thumbnailPath);
                }
            }
            
            // Remove database record
            _context.FileUploadRecords.Remove(uploadRecord);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("File {FileUrl} deleted successfully by user {UserId}", fileUrl, userId);
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file {FileUrl} for user {UserId}", fileUrl, userId);
            return false;
        }
    }
    
    /// <inheritdoc />
    public async Task<string?> GenerateThumbnailAsync(string fileUrl, string userId)
    {
        try
        {
            var physicalPath = Path.Combine("wwwroot", fileUrl.TrimStart('/'));
            
            if (!File.Exists(physicalPath))
            {
                return null;
            }
            
            using var originalImage = Image.Load(physicalPath);
            
            // Calculate thumbnail dimensions (max 200x200, maintaining aspect ratio)
            var maxThumbnailSize = 200;
            var ratioX = (double)maxThumbnailSize / originalImage.Width;
            var ratioY = (double)maxThumbnailSize / originalImage.Height;
            var ratio = Math.Min(ratioX, ratioY);
            
            var newWidth = (int)(originalImage.Width * ratio);
            var newHeight = (int)(originalImage.Height * ratio);
            
            // Create thumbnail using ImageSharp
            originalImage.Mutate(x => x.Resize(newWidth, newHeight));
            
            // Save thumbnail
            var thumbnailFileName = $"thumb_{Path.GetFileName(physicalPath)}";
            var thumbnailDirectory = Path.GetDirectoryName(physicalPath)!;
            var thumbnailPath = Path.Combine(thumbnailDirectory, thumbnailFileName);
            
            await originalImage.SaveAsJpegAsync(thumbnailPath);
            
            // Return thumbnail URL
            var thumbnailUrl = fileUrl.Replace(Path.GetFileName(fileUrl), thumbnailFileName);
            return thumbnailUrl;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating thumbnail for file {FileUrl}", fileUrl);
            return null;
        }
    }
    
    /// <inheritdoc />
    public async Task<FileUsageStats> GetFileUsageStatsAsync(string userId)
    {
        try
        {
            var today = DateTimeOffset.UtcNow.Date;
            var isPremium = await IsUserPremium(userId);
            
            var uploadRecords = await _context.FileUploadRecords
                .Where(r => r.UserId == userId)
                .ToListAsync();
            
            var todayRecords = uploadRecords.Where(r => r.UploadedAt.Date == today).ToList();
            
            var stats = new FileUsageStats
            {
                UserId = userId,
                TotalFiles = uploadRecords.Count,
                TotalStorageUsed = uploadRecords.Sum(r => r.FileSize),
                FilesToday = todayRecords.Count,
                StorageTodayUsed = todayRecords.Sum(r => r.FileSize),
                StorageLimit = isPremium ? PREMIUM_USER_FILE_LIMIT * 100 : FREE_USER_FILE_LIMIT * 20, // Total storage limit
                DailyFileLimit = isPremium ? PREMIUM_USER_DAILY_LIMIT : FREE_USER_DAILY_LIMIT
            };
            
            stats.HasExceededLimits = stats.FilesToday >= stats.DailyFileLimit || 
                                     stats.TotalStorageUsed >= stats.StorageLimit;
            
            return stats;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting file usage stats for user {UserId}", userId);
            return new FileUsageStats { UserId = userId };
        }
    }
    
    /// <summary>
    /// Check if user has premium subscription
    /// </summary>
    private async Task<bool> IsUserPremium(string userId)
    {
        // Implement proper premium subscription check
        // For now, check if user has any subscription
        return await _context.UserSubscriptions
            .AnyAsync(s => s.UserId == userId && s.IsActive);
    }
}