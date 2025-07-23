using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.WebApp.Models.DTOs.Billing
{
    /// <summary>
    /// DTO for account deletion request
    /// </summary>
    public class DeleteAccountDto
    {
        [Required]
        public string Password { get; set; }
        
        public string Reason { get; set; }
        
        public string Feedback { get; set; }
    }
}