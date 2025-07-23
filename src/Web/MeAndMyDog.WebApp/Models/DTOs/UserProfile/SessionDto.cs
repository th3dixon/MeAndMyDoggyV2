using System;

namespace MeAndMyDog.WebApp.Models.DTOs.UserProfile
{
    /// <summary>
    /// DTO for session information
    /// </summary>
    public class SessionDto
    {
        public string Id { get; set; }
        public string DeviceType { get; set; }
        public string DeviceName { get; set; }
        public string Browser { get; set; }
        public string IpAddress { get; set; }
        public string Location { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastActivityAt { get; set; }
        public bool IsCurrent { get; set; }
    }
}