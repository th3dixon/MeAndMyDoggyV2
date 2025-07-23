using System.Collections.Generic;

namespace MeAndMyDog.WebApp.Models.DTOs.TwoFactorAuth
{
    /// <summary>
    /// Response DTO for 2FA setup
    /// </summary>
    public class Setup2FAResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string QrCodeUrl { get; set; } // For authenticator app setup
        public string ManualKey { get; set; } // Manual entry key for authenticator
        public List<string> BackupCodes { get; set; } // Backup recovery codes
    }
}