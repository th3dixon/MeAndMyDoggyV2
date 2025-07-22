namespace MeAndMyDog.API.DTOs.Address
{
    /// <summary>
    /// Data transfer object for UK postcode information
    /// </summary>
    public class PostcodeInfoDto
    {
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
        public string? AreaName { get; set; }
        public string? Region { get; set; }
        public List<string> Cities { get; set; } = new List<string>();
        public int AddressCount { get; set; }
    }
}