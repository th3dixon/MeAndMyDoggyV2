using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.WebApp.Models.DTOs.TwoFactorAuth
{
    /// <summary>
    /// DTO for verifying 2FA code
    /// </summary>
    public class Verify2FADto
    {
        [Required]
        public string Code { get; set; }
        
        public string Type { get; set; } // "sms" or "app"
    }
}