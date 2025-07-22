namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// File usage statistics for a user
/// </summary>
public class FileUsageStats
{
    /// <summary>
    /// User ID
    /// </summary>
    public string UserId { get; set; } = string.Empty;
    
    /// <summary>
    /// Total files uploaded
    /// </summary>
    public int TotalFiles { get; set; }
    
    /// <summary>
    /// Total storage used in bytes
    /// </summary>
    public long TotalStorageUsed { get; set; }
    
    /// <summary>
    /// Files uploaded today
    /// </summary>
    public int FilesToday { get; set; }
    
    /// <summary>
    /// Storage used today in bytes
    /// </summary>
    public long StorageTodayUsed { get; set; }
    
    /// <summary>
    /// Storage limit in bytes (based on subscription)
    /// </summary>
    public long StorageLimit { get; set; }
    
    /// <summary>
    /// Daily file upload limit (based on subscription)
    /// </summary>
    public int DailyFileLimit { get; set; }
    
    /// <summary>
    /// Whether user has exceeded limits
    /// </summary>
    public bool HasExceededLimits { get; set; }
    
    /// <summary>
    /// Quota usage percentage (0-100)
    /// </summary>
    public double QuotaUsagePercentage => StorageLimit > 0 ? (double)TotalStorageUsed / StorageLimit * 100 : 0;
    
    /// <summary>
    /// Storage quota in bytes (alias for StorageLimit)
    /// </summary>
    public long StorageQuota
    {
        get => StorageLimit;
        set => StorageLimit = value;
    }
    
    /// <summary>
    /// Files by type breakdown
    /// </summary>
    public Dictionary<string, int> FilesByType { get; set; } = new();
    
    /// <summary>
    /// Storage by type breakdown
    /// </summary>
    public Dictionary<string, long> StorageByType { get; set; } = new();
    
    /// <summary>
    /// Recent file uploads
    /// </summary>
    public List<FileInfoDto> RecentUploads { get; set; } = new();
    
    /// <summary>
    /// Uploads by month breakdown
    /// </summary>
    public Dictionary<string, int> UploadsByMonth { get; set; } = new();
}