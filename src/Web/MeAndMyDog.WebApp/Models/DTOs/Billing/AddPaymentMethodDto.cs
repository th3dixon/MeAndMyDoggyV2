namespace MeAndMyDog.WebApp.Models.DTOs.Billing
{
    /// <summary>
    /// DTO for adding payment method
    /// </summary>
    public class AddPaymentMethodDto
    {
        public string PaymentMethodId { get; set; }
        public bool SetAsDefault { get; set; }
    }
}