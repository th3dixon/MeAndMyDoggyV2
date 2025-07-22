namespace MeAndMyDog.API.Attributes;

/// <summary>
/// Rate limit attribute for file upload endpoints
/// </summary>
public class FileUploadRateLimitAttribute : RateLimitAttribute
{
    public FileUploadRateLimitAttribute()
    {
        RequestsPerMinute = 10;
        RequestsPerHour = 100;
        ErrorMessage = "File upload rate limit exceeded. Please wait before uploading more files.";
        PerUser = true;
    }
}