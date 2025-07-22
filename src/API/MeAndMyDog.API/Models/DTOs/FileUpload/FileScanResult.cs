namespace MeAndMyDog.API.Models.DTOs;

/// <summary>
/// File security scan result
/// </summary>
public class FileScanResult
{
    /// <summary>
    /// Whether the file passed security scan
    /// </summary>
    public bool IsClean { get; set; } = true;

    /// <summary>
    /// Scan result summary
    /// </summary>
    public string ScanSummary { get; set; } = string.Empty;

    /// <summary>
    /// Detected threats (if any)
    /// </summary>
    public List<string> Threats { get; set; } = new();

    /// <summary>
    /// Scanner used
    /// </summary>
    public string Scanner { get; set; } = string.Empty;

    /// <summary>
    /// When the scan was performed
    /// </summary>
    public DateTimeOffset ScannedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Scan confidence score (0-100)
    /// </summary>
    public int ConfidenceScore { get; set; } = 100;

    /// <summary>
    /// Whether file quarantine is recommended
    /// </summary>
    public bool RecommendQuarantine { get; set; } = false;
    
    /// <summary>
    /// Whether scan was successful
    /// </summary>
    public bool Success { get; set; } = true;
    
    /// <summary>
    /// Whether file is safe (alias for IsClean)
    /// </summary>
    public bool IsSafe
    {
        get => IsClean;
        set => IsClean = value;
    }
    
    /// <summary>
    /// Scan status (alias for ScanDetails)
    /// </summary>
    public string Status
    {
        get => IsClean ? "clean" : "infected";
        set { /* Read-only computed property */ }
    }
    
    /// <summary>
    /// Threats detected (alias for Threats)
    /// </summary>
    public List<string> ThreatsDetected
    {
        get => Threats;
        set => Threats = value;
    }
    
    /// <summary>
    /// Scan engine (alias for Scanner)
    /// </summary>
    public string ScanEngine
    {
        get => Scanner;
        set => Scanner = value;
    }
    
    /// <summary>
    /// Risk score (0-100, computed from confidence)
    /// </summary>
    public int RiskScore
    {
        get => IsClean ? 0 : (100 - ConfidenceScore);
        set { /* Read-only computed property */ }
    }
    
    /// <summary>
    /// File hash for verification
    /// </summary>
    public string FileHash { get; set; } = string.Empty;
    
    /// <summary>
    /// Additional scan details
    /// </summary>
    public Dictionary<string, object> ScanDetails { get; set; } = new();
}