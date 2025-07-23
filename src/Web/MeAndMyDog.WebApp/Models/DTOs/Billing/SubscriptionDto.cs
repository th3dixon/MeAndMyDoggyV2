namespace MeAndMyDog.WebApp.Models.DTOs.Billing
{
    /// <summary>
    /// DTO for subscription information
    /// </summary>
    public class SubscriptionDto
    {
        public string PlanName { get; set; }
        public string PlanId { get; set; }
        public string Status { get; set; }
        public decimal PricePerMonth { get; set; }
        public string Currency { get; set; }
        public DateTime? NextBillingDate { get; set; }
        public bool CancelAtPeriodEnd { get; set; }
        public List<string> Features { get; set; }
    }
}