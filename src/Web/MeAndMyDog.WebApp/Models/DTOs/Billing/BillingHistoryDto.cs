namespace MeAndMyDog.WebApp.Models.DTOs.Billing
{
    /// <summary>
    /// DTO for billing history
    /// </summary>
    public class BillingHistoryDto
    {
        public string Id { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Status { get; set; }
        public string InvoiceUrl { get; set; }
    }
}