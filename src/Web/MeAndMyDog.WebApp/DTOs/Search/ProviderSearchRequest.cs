/// <summary>
/// Request model for provider search
/// </summary>
public class ProviderSearchRequest
{
    public string? Location { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public int? RadiusMiles { get; set; }
    public List<string>? ServiceCategoryIds { get; set; }
    public List<string>? SubServiceIds { get; set; }
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
    public int? PetCount { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public decimal? MinRating { get; set; }
    public bool? VerifiedOnly { get; set; }
    public bool? EmergencyServiceOnly { get; set; }
    public bool? WeekendAvailable { get; set; }
    public bool? EveningAvailable { get; set; }
    public string? SortBy { get; set; }
    public string? SortDirection { get; set; }
    public int? Page { get; set; }
    public int? PageSize { get; set; }
    public bool? IncludeAvailability { get; set; }
}