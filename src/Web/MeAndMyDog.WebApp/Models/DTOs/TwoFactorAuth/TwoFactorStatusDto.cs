using System;

namespace MeAndMyDog.WebApp.Models.DTOs.TwoFactorAuth
{
    /// <summary>
    /// DTO for 2FA status
    /// </summary>
    public class TwoFactorStatusDto
    {
        public bool SmsEnabled { get; set; }
        public bool AppEnabled { get; set; }
        public string PhoneNumber { get; set; } // Masked phone number
        public DateTime? LastUsed { get; set; }
        public int BackupCodesRemaining { get; set; }
    }
}