using System.ComponentModel.DataAnnotations;

namespace ubuntu_health_api.Models.DTO
{
  public class UpgradeSubscriptionDto
  {
    [Required(ErrorMessage = "Plan is required")]
    [RegularExpression(@"^(Solo|Practice|Clinic)$", ErrorMessage = "Plan must be Solo, Practice, or Clinic")]
    public required string Plan { get; set; }
  }
}
