namespace MeAndMyDog.WebApp.Models.DTOs.Billing
{
    /// <summary>
    /// DTO for payment method
    /// </summary>
    public class PaymentMethodDto
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Brand { get; set; }
        public string Last4 { get; set; }
        public int ExpiryMonth { get; set; }
        public int ExpiryYear { get; set; }
        public bool IsDefault { get; set; }
    }
}