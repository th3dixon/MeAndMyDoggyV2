using System.ComponentModel.DataAnnotations;

namespace MeAndMyDog.WebApp.Models.DTOs.Billing
{
    /// <summary>
    /// DTO for changing subscription plan
    /// </summary>
    public class ChangePlanDto
    {
        [Required]
        public string PlanId { get; set; }
    }
}