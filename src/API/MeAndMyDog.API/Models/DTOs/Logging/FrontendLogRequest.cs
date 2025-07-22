using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.API.Models.DTOs.Logging;

/// <summary>
/// Request model for frontend logging operations
/// </summary>
public class FrontendLogRequest
{
    /// <summary>
    /// List of log entries to process
    /// </summary>
    [Required]
    public List<FrontendLogEntry> Logs { get; set; } = new();
}