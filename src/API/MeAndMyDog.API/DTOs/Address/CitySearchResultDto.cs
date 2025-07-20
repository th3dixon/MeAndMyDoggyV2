namespace MeAndMyDog.API.DTOs.Address
{
    public class CitySearchResultDto
    {
        public int CityId { get; set; }
        public string CityName { get; set; } = string.Empty;
        public string? CityType { get; set; }
        public int CountyId { get; set; }
        public string CountyName { get; set; } = string.Empty;
        public string CountryName { get; set; } = string.Empty;
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public int StreetCount { get; set; }
        public int AddressCount { get; set; }
    }
}