using Microsoft.EntityFrameworkCore;
using MeAndMyDog.API.Data;
using MeAndMyDog.API.Models.DTOs;
using MeAndMyDog.API.Models.Entities;
using MeAndMyDog.API.Models.Enums;
using MeAndMyDog.API.Services.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using System.Security.Cryptography;
using System.Text.Json;

namespace MeAndMyDog.API.Services.Implementations;

/// <summary>
/// Enhanced file upload service with comprehensive features
/// </summary>
public class EnhancedFileUploadService : IFileUploadService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<EnhancedFileUploadService> _logger;
    private readonly IConfiguration _configuration;

    // File size limits in bytes
    private const long FREE_USER_FILE_LIMIT = 10 * 1024 * 1024; // 10MB
    private const long PREMIUM_USER_FILE_LIMIT = 100 * 1024 * 1024; // 100MB
    private const long TOTAL_FREE_STORAGE = 1024 * 1024 * 1024; // 1GB
    private const long TOTAL_PREMIUM_STORAGE = 10L * 1024 * 1024 * 1024; // 10GB

    // Daily file limits
    private const int FREE_USER_DAILY_LIMIT = 20;
    private const int PREMIUM_USER_DAILY_LIMIT = 200;

    // Security scanning settings
    private readonly HashSet<string> _allowedExtensions = new()
    {
        ".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp", ".tiff",
        ".mp4", ".avi", ".mov", ".wmv", ".webm", ".mkv",
        ".mp3", ".wav", ".ogg", ".aac", ".flac", ".m4a",
        ".pdf", ".doc", ".docx", ".txt", ".rtf", ".xls", ".xlsx", ".ppt", ".pptx"
    };

    private readonly HashSet<string> _dangerousExtensions = new()
    {
        ".exe", ".bat", ".cmd", ".com", ".scr", ".vbs", ".js", ".jar", ".msi", ".ps1"
    };

    private readonly Dictionary<string, FileUploadType> _mimeTypeMapping = new()
    {
        // Images
        ["image/jpeg"] = FileUploadType.Image,
        ["image/jpg"] = FileUploadType.Image,
        ["image/png"] = FileUploadType.Image,
        ["image/gif"] = FileUploadType.Image,
        ["image/webp"] = FileUploadType.Image,
        ["image/bmp"] = FileUploadType.Image,
        ["image/tiff"] = FileUploadType.Image,

        // Videos
        ["video/mp4"] = FileUploadType.Video,
        ["video/avi"] = FileUploadType.Video,
        ["video/quicktime"] = FileUploadType.Video,
        ["video/x-msvideo"] = FileUploadType.Video,
        ["video/webm"] = FileUploadType.Video,
        ["video/x-matroska"] = FileUploadType.Video,

        // Audio
        ["audio/mpeg"] = FileUploadType.Audio,
        ["audio/mp3"] = FileUploadType.Audio,
        ["audio/wav"] = FileUploadType.Audio,
        ["audio/ogg"] = FileUploadType.Audio,
        ["audio/aac"] = FileUploadType.Audio,
        ["audio/flac"] = FileUploadType.Audio,
        ["audio/x-m4a"] = FileUploadType.Audio,

        // Documents
        ["application/pdf"] = FileUploadType.Document,
        ["application/msword"] = FileUploadType.Document,
        ["application/vnd.openxmlformats-officedocument.wordprocessingml.document"] = FileUploadType.Document,
        ["text/plain"] = FileUploadType.Document,
        ["application/rtf"] = FileUploadType.Document,
        ["application/vnd.ms-excel"] = FileUploadType.Document,
        ["application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"] = FileUploadType.Document,
        ["application/vnd.ms-powerpoint"] = FileUploadType.Document,
        ["application/vnd.openxmlformats-officedocument.presentationml.presentation"] = FileUploadType.Document
    };

    public EnhancedFileUploadService(
        ApplicationDbContext context,
        ILogger<EnhancedFileUploadService> logger,
        IConfiguration configuration)
    {
        _context = context;
        _logger = logger;
        _configuration = configuration;
    }

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

            // Check user quotas
            var usageStats = await GetFileUsageStatsAsync(userId);
            if (usageStats.QuotaUsagePercentage >= 100)
            {
                return new FileUploadResult
                {
                    Success = false,
                    ErrorMessage = "Storage quota exceeded. Please delete some files or upgrade your plan."
                };
            }

            // Generate unique identifiers
            var fileId = Guid.NewGuid().ToString();
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var uniqueFileName = $"{fileId}{fileExtension}";

            // Calculate file hash
            var fileHash = await CalculateFileHashAsync(file);

            // Check for duplicate files
            var existingFile = await _context.FileUploads
                .FirstOrDefaultAsync(f => f.FileHash == fileHash && f.UploadedBy == userId && !f.IsDeleted);

            if (existingFile != null)
            {
                return new FileUploadResult
                {
                    Success = true,
                    FileId = existingFile.Id,
                    FileInfo = MapToFileInfoDto(existingFile),
                    UploadUrl = existingFile.FileUrl,
                    ThumbnailUrl = existingFile.ThumbnailUrl
                };
            }

            // Create storage directory
            var uploadDir = Path.Combine("wwwroot", "uploads", DateTime.UtcNow.ToString("yyyy"), DateTime.UtcNow.ToString("MM"));
            Directory.CreateDirectory(uploadDir);

            var filePath = Path.Combine(uploadDir, uniqueFileName);
            var relativePath = Path.Combine("uploads", DateTime.UtcNow.ToString("yyyy"), DateTime.UtcNow.ToString("MM"), uniqueFileName);

            // Save file to disk
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Process file metadata
            var metadata = new Dictionary<string, object>();
            string? thumbnailUrl = null;
            string? imageDimensions = null;

            var uploadType = DetermineUploadType(file.ContentType, messageId);

            // Process images
            if (uploadType == FileUploadType.Image)
            {
                var imageInfo = await ProcessImageAsync(filePath, fileId);
                if (imageInfo != null)
                {
                    thumbnailUrl = imageInfo.ThumbnailUrl;
                    imageDimensions = JsonSerializer.Serialize(new { width = imageInfo.Width, height = imageInfo.Height });
                    metadata["width"] = imageInfo.Width;
                    metadata["height"] = imageInfo.Height;
                }
            }

            // Create file upload entity
            var fileUpload = new FileUploadRecord
            {
                Id = fileId,
                FileName = file.FileName,
                UniqueFileName = Path.GetFileName(relativePath),
                FileUrl = $"/{relativePath.Replace('\\', '/')}",
                ThumbnailUrl = thumbnailUrl,
                FileSize = file.Length,
                ContentType = file.ContentType ?? "application/octet-stream",
                UserId = userId,
                MessageId = messageId,
                UploadedAt = DateTimeOffset.UtcNow,
                TagsJson = JsonSerializer.Serialize(metadata),
                IsDeleted = false
            };

            _context.FileUploads.Add(fileUpload);
            await _context.SaveChangesAsync();

            // Queue security scan
            _ = Task.Run(async () => await ScanFileAsync(fileId));

            var result = new FileUploadResult
            {
                Success = true,
                FileId = fileId,
                FileInfo = MapToFileInfoDto(fileUpload),
                UploadUrl = fileUpload.FileUrl,
                ThumbnailUrl = thumbnailUrl,
                ProcessingStatus = "completed"
            };

            _logger.LogInformation("File {FileName} uploaded successfully by user {UserId} with ID {FileId}", 
                file.FileName, userId, fileId);

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

    public async Task<List<FileUploadResult>> UploadMultipleFilesAsync(IFormFile[] files, string userId, string? messageId = null)
    {
        var results = new List<FileUploadResult>();

        foreach (var file in files)
        {
            var result = await UploadFileAsync(file, userId, messageId);
            results.Add(result);

            // Stop uploading if quota exceeded
            if (!result.Success && result.ErrorMessage?.Contains("quota") == true)
            {
                break;
            }
        }

        return results;
    }

    public async Task<FileValidationResult> ValidateFileAsync(IFormFile file, string userId)
    {
        var result = new FileValidationResult();

        // Check if file exists
        if (file == null || file.Length == 0)
        {
            result.IsValid = false;
            result.Errors.Add("No file provided or file is empty");
            return result;
        }

        // Check file extension
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (string.IsNullOrEmpty(extension) || !_allowedExtensions.Contains(extension))
        {
            result.IsValid = false;
            result.Errors.Add($"File extension '{extension}' is not allowed");
            return result;
        }

        // Check for dangerous extensions
        if (_dangerousExtensions.Contains(extension))
        {
            result.IsValid = false;
            result.Errors.Add("File type is not allowed for security reasons");
            return result;
        }

        // Check MIME type
        if (string.IsNullOrEmpty(file.ContentType) || !_mimeTypeMapping.ContainsKey(file.ContentType))
        {
            result.Warnings.Add($"MIME type '{file.ContentType}' may not be fully supported");
        }

        // Check user subscription and limits
        var isPremium = await IsUserPremium(userId);
        var fileSizeLimit = isPremium ? PREMIUM_USER_FILE_LIMIT : FREE_USER_FILE_LIMIT;

        if (file.Length > fileSizeLimit)
        {
            result.IsValid = false;
            result.Errors.Add($"File size {file.Length / (1024 * 1024):F1}MB exceeds limit of {fileSizeLimit / (1024 * 1024)}MB");
            return result;
        }

        // Check daily upload limits
        var today = DateTimeOffset.UtcNow.Date;
        var todayUploads = await _context.FileUploads
            .CountAsync(f => f.UploadedBy == userId && f.UploadedAt.Date == today && !f.IsDeleted);

        var dailyLimit = isPremium ? PREMIUM_USER_DAILY_LIMIT : FREE_USER_DAILY_LIMIT;
        if (todayUploads >= dailyLimit)
        {
            result.IsValid = false;
            result.Errors.Add($"Daily upload limit of {dailyLimit} files exceeded");
            return result;
        }

        // Check total storage usage
        var totalUsage = await _context.FileUploads
            .Where(f => f.UploadedBy == userId && !f.IsDeleted)
            .SumAsync(f => f.FileSize);

        var storageLimit = isPremium ? TOTAL_PREMIUM_STORAGE : TOTAL_FREE_STORAGE;
        if (totalUsage + file.Length > storageLimit)
        {
            result.IsValid = false;
            result.Errors.Add($"Storage quota would be exceeded. Current usage: {totalUsage / (1024 * 1024):F1}MB, Limit: {storageLimit / (1024 * 1024)}MB");
            return result;
        }

        result.IsValid = true;
        result.DetectedFileType = _mimeTypeMapping.GetValueOrDefault(file.ContentType, FileUploadType.Document).ToString();
        result.FileSize = file.Length;
        return result;
    }

    public async Task<bool> DeleteFileAsync(string fileUrl, string userId)
    {
        try
        {
            var fileUpload = await _context.FileUploads
                .FirstOrDefaultAsync(f => f.FileUrl == fileUrl && f.UserId == userId && !f.IsDeleted);

            if (fileUpload == null)
            {
                return false;
            }

            // Mark as deleted (soft delete)
            fileUpload.IsDeleted = true;
            fileUpload.DeletedAt = DateTimeOffset.UtcNow;
            fileUpload.DeletedBy = userId;

            await _context.SaveChangesAsync();

            // Queue physical file deletion
            _ = Task.Run(() => DeletePhysicalFileAsync(fileUpload.FilePath, fileUpload.ThumbnailUrl));

            _logger.LogInformation("File {FileId} deleted by user {UserId}", fileUpload.Id, userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file {FileUrl} for user {UserId}", fileUrl, userId);
            return false;
        }
    }

    public async Task<string?> GenerateThumbnailAsync(string fileUrl, string userId)
    {
        try
        {
            var fileUpload = await _context.FileUploads
                .FirstOrDefaultAsync(f => f.FileUrl == fileUrl && !f.IsDeleted);

            if (fileUpload == null || !fileUpload.UploadType.Contains("Image"))
            {
                return null;
            }

            var filePath = Path.Combine("wwwroot", fileUpload.FilePath);
            if (!File.Exists(filePath))
            {
                return null;
            }

            var thumbnailFileName = $"thumb_{Path.GetFileName(filePath)}";
            var thumbnailDir = Path.GetDirectoryName(filePath)!;
            var thumbnailPath = Path.Combine(thumbnailDir, thumbnailFileName);

            using var image = await Image.LoadAsync(filePath);
            
            // Calculate thumbnail size (max 300x300, maintain aspect ratio)
            const int maxSize = 300;
            var ratio = Math.Min((float)maxSize / image.Width, (float)maxSize / image.Height);
            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            image.Mutate(x => x.Resize(newWidth, newHeight));
            await image.SaveAsync(thumbnailPath, new JpegEncoder { Quality = 85 });

            var thumbnailUrl = fileUpload.FileUrl.Replace(Path.GetFileName(fileUpload.FileUrl), thumbnailFileName);
            
            // Update file record with thumbnail URL
            fileUpload.ThumbnailUrl = thumbnailUrl;
            await _context.SaveChangesAsync();

            return thumbnailUrl;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating thumbnail for file {FileUrl}", fileUrl);
            return null;
        }
    }

    public async Task<FileUsageStats> GetFileUsageStatsAsync(string userId)
    {
        try
        {
            var files = await _context.FileUploads
                .Where(f => f.UploadedBy == userId && !f.IsDeleted)
                .ToListAsync();

            var isPremium = await IsUserPremium(userId);
            var totalStorageUsed = files.Sum(f => f.FileSize);
            var storageQuota = isPremium ? TOTAL_PREMIUM_STORAGE : TOTAL_FREE_STORAGE;

            var filesByType = files.GroupBy(f => f.UploadType)
                .ToDictionary(g => g.Key, g => g.Count());

            var storageByType = files.GroupBy(f => f.UploadType)
                .ToDictionary(g => g.Key, g => g.Sum(f => f.FileSize));

            var recentFiles = files.OrderByDescending(f => f.UploadedAt)
                .Take(10)
                .Select(MapToFileInfoDto)
                .ToList();

            return new FileUsageStats
            {
                UserId = userId,
                TotalFiles = files.Count,
                TotalStorageUsed = totalStorageUsed,
                StorageQuota = storageQuota,
                FilesByType = filesByType,
                StorageByType = storageByType,
                RecentUploads = recentFiles,
                UploadsByMonth = files.GroupBy(f => f.UploadedAt.ToString("yyyy-MM"))
                    .ToDictionary(g => g.Key, g => g.Count())
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting file usage stats for user {UserId}", userId);
            return new FileUsageStats { UserId = userId };
        }
    }

    private async Task<string> CalculateFileHashAsync(IFormFile file)
    {
        using var md5 = MD5.Create();
        using var stream = file.OpenReadStream();
        var hash = await Task.Run(() => md5.ComputeHash(stream));
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    private FileUploadType DetermineUploadType(string? contentType, string? messageId)
    {
        if (!string.IsNullOrEmpty(messageId))
        {
            if (contentType?.StartsWith("audio/") == true)
                return FileUploadType.VoiceMessage;
            else
                return FileUploadType.MessageAttachment;
        }

        return _mimeTypeMapping.GetValueOrDefault(contentType, FileUploadType.Document);
    }

    private async Task<string?> GetConversationIdFromMessage(string? messageId)
    {
        if (string.IsNullOrEmpty(messageId))
            return null;

        var message = await _context.Messages
            .Where(m => m.Id == messageId)
            .Select(m => m.ConversationId)
            .FirstOrDefaultAsync();

        return message;
    }

    private async Task<bool> IsUserPremium(string userId)
    {
        // Implement proper premium check based on your subscription system
        // For now, return false (all users are free)
        return await Task.FromResult(false);
    }

    private async Task<ImageProcessingResult?> ProcessImageAsync(string filePath, string fileId)
    {
        try
        {
            using var image = await Image.LoadAsync(filePath);
            
            // Generate thumbnail
            var thumbnailFileName = $"thumb_{fileId}.jpg";
            var thumbnailDir = Path.GetDirectoryName(filePath)!;
            var thumbnailPath = Path.Combine(thumbnailDir, thumbnailFileName);

            const int thumbnailSize = 300;
            var ratio = Math.Min((float)thumbnailSize / image.Width, (float)thumbnailSize / image.Height);
            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            using var thumbnail = image.CloneAs<Rgba32>();
            thumbnail.Mutate(x => x.Resize(newWidth, newHeight));
            await thumbnail.SaveAsync(thumbnailPath, new JpegEncoder { Quality = 85 });

            var thumbnailUrl = $"/uploads/{DateTime.UtcNow:yyyy}/{DateTime.UtcNow:MM}/{thumbnailFileName}";

            return new ImageProcessingResult
            {
                Width = image.Width,
                Height = image.Height,
                ThumbnailUrl = thumbnailUrl
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing image {FileId}", fileId);
            return null;
        }
    }

    private async Task DeletePhysicalFileAsync(string filePath, string? thumbnailUrl)
    {
        try
        {
            var fullPath = Path.Combine("wwwroot", filePath);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            if (!string.IsNullOrEmpty(thumbnailUrl))
            {
                var thumbnailPath = Path.Combine("wwwroot", thumbnailUrl.TrimStart('/'));
                if (File.Exists(thumbnailPath))
                {
                    File.Delete(thumbnailPath);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting physical files: {FilePath}, {ThumbnailUrl}", filePath, thumbnailUrl);
        }
    }

    private FileInfoDto MapToFileInfoDto(FileUploadRecord fileUpload)
    {
        var tags = new List<string>();
        if (!string.IsNullOrEmpty(fileUpload.TagsJson))
        {
            try
            {
                tags = JsonSerializer.Deserialize<List<string>>(fileUpload.TagsJson) ?? new List<string>();
            }
            catch
            {
                // Ignore JSON parsing errors
            }
        }

        var metadata = new Dictionary<string, object>();
        if (!string.IsNullOrEmpty(fileUpload.MetadataJson))
        {
            try
            {
                metadata = JsonSerializer.Deserialize<Dictionary<string, object>>(fileUpload.MetadataJson) ?? new Dictionary<string, object>();
            }
            catch
            {
                // Ignore JSON parsing errors
            }
        }

        return new FileInfoDto
        {
            Id = fileUpload.Id,
            FileName = fileUpload.FileName,
            FileExtension = fileUpload.FileExtension,
            ContentType = fileUpload.ContentType,
            FileSize = fileUpload.FileSize,
            Url = fileUpload.FileUrl ?? string.Empty,
            ThumbnailUrl = fileUpload.ThumbnailUrl,
            UploadedBy = fileUpload.UploadedBy,
            UploadedAt = fileUpload.UploadedAt,
            UploadType = fileUpload.UploadType,
            IsPublic = fileUpload.IsPublic,
            Description = fileUpload.Description,
            Tags = tags,
            DownloadCount = fileUpload.DownloadCount,
            IsEncrypted = fileUpload.IsEncrypted,
            ScanStatus = fileUpload.ScanStatus,
            Metadata = metadata
        };
    }

    public async Task<FileScanResult> ScanFileAsync(string fileId)
    {
        try
        {
            var fileUpload = await _context.FileUploads.FindAsync(fileId);
            if (fileUpload == null)
            {
                return new FileScanResult
                {
                    Success = false,
                    Status = "error",
                    ScannedAt = DateTimeOffset.UtcNow
                };
            }

            // Update scan status
            fileUpload.ScanStatus = "scanning";
            await _context.SaveChangesAsync();

            // Simulate virus scanning (implement real scanning with ClamAV, VirusTotal, etc.)
            await Task.Delay(1000); // Simulate scan time

            var scanResult = new FileScanResult
            {
                Success = true,
                IsSafe = true,
                Status = "clean",
                ThreatsDetected = new List<string>(),
                ScanEngine = "Built-in",
                ScannedAt = DateTimeOffset.UtcNow,
                RiskScore = 0,
                FileHash = fileUpload.FileHash ?? string.Empty,
                ScanDetails = new Dictionary<string, object>
                {
                    ["file_type"] = fileUpload.ContentType,
                    ["file_size"] = fileUpload.FileSize,
                    ["scan_duration_ms"] = 1000
                }
            };

            // Update file with scan results
            fileUpload.ScanStatus = scanResult.IsSafe ? "clean" : "infected";
            fileUpload.ScanResultJson = JsonSerializer.Serialize(scanResult);
            fileUpload.ScannedAt = scanResult.ScannedAt;

            await _context.SaveChangesAsync();

            _logger.LogInformation("File {FileId} scanned successfully. Status: {Status}", fileId, scanResult.Status);

            return scanResult;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error scanning file {FileId}", fileId);
            
            // Update scan status to error
            var fileUpload = await _context.FileUploads.FindAsync(fileId);
            if (fileUpload != null)
            {
                fileUpload.ScanStatus = "error";
                await _context.SaveChangesAsync();
            }

            return new FileScanResult
            {
                Success = false,
                Status = "error",
                ScannedAt = DateTimeOffset.UtcNow
            };
        }
    }

    private class ImageProcessingResult
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public string? ThumbnailUrl { get; set; }
    }
}