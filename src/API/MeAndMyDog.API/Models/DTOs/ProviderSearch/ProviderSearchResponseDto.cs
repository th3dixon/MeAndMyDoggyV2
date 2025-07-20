namespace MeAndMyDog.API.Models.DTOs.ProviderSearch;

/// <summary>
/// Response DTO for provider search operations with pagination
/// </summary>
public class ProviderSearchResponseDto
{
    /// <summary>
    /// List of provider search results
    /// </summary>
    public List<ProviderSearchResultDto> Results { get; set; } = new();
    
    /// <summary>
    /// Total number of providers found (before pagination)
    /// </summary>
    public int TotalCount { get; set; }
    
    /// <summary>
    /// Current page number (1-based)
    /// </summary>
    public int PageNumber { get; set; }
    
    /// <summary>
    /// Number of results per page
    /// </summary>
    public int PageSize { get; set; }
    
    /// <summary>
    /// Total number of pages available
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    
    /// <summary>
    /// Whether there are more pages available
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;
    
    /// <summary>
    /// Whether there is a previous page
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;
    
    /// <summary>
    /// Search location information
    /// </summary>
    public LocationDto? SearchLocation { get; set; }
    
    /// <summary>
    /// Applied search filters
    /// </summary>
    public ProviderSearchFilterDto? Filters { get; set; }
    
    /// <summary>
    /// Search execution time in milliseconds
    /// </summary>
    public long ExecutionTimeMs { get; set; }
    
    /// <summary>
    /// Additional message or information about the search
    /// </summary>
    public string? Message { get; set; }
}