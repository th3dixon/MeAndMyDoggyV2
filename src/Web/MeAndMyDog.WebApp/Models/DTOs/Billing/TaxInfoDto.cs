namespace MeAndMyDog.WebApp.Models.DTOs.Billing
{
    /// <summary>
    /// DTO for tax information
    /// </summary>
    public class TaxInfoDto
    {
        public string BusinessType { get; set; }
        public string VatNumber { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
    }
}