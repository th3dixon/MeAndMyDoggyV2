namespace MeAndMyDog.API.DTOs.Address
{
    public class AddressSearchResultDto
    {
        public int? CacheId { get; set; }
        public string DisplayText { get; set; } = string.Empty;
        public string AddressLine1 { get; set; } = string.Empty;
        public string? AddressLine2 { get; set; }
        public string City { get; set; } = string.Empty;
        public string County { get; set; } = string.Empty;
        public string PostcodeFormatted { get; set; } = string.Empty;
        public int PostcodeId { get; set; }
        public int? AddressId { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public int SearchRank { get; set; }
        public string Source { get; set; } = "cache";
    }
}