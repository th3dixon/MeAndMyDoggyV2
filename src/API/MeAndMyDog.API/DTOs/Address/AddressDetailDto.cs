namespace MeAndMyDog.API.DTOs.Address
{
    public class AddressDetailDto
    {
        public int AddressId { get; set; }
        public string? BuildingNumber { get; set; }
        public string? BuildingName { get; set; }
        public string? SubBuilding { get; set; }
        public int StreetId { get; set; }
        public string StreetName { get; set; } = string.Empty;
        public string? StreetType { get; set; }
        public int CityId { get; set; }
        public string CityName { get; set; } = string.Empty;
        public string? CityType { get; set; }
        public int CountyId { get; set; }
        public string CountyName { get; set; } = string.Empty;
        public int CountryId { get; set; }
        public string CountryName { get; set; } = string.Empty;
        public string CountryCode { get; set; } = string.Empty;
        public int PostcodeId { get; set; }
        public string PostcodeFormatted { get; set; } = string.Empty;
        public string Postcode { get; set; } = string.Empty;
        public string OutwardCode { get; set; } = string.Empty;
        public string InwardCode { get; set; } = string.Empty;
        public string PostcodeArea { get; set; } = string.Empty;
        public string PostcodeDistrict { get; set; } = string.Empty;
        public string PostcodeSector { get; set; } = string.Empty;
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public long? UPRN { get; set; }
        public bool IsResidential { get; set; }
        public bool IsActive { get; set; }
        public string FullAddress { get; set; } = string.Empty;
    }
}