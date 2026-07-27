using System.ComponentModel.DataAnnotations;

namespace ubuntu_health_api.Models.DTO
{
  public class UpgradeSubscriptionDto
  {
    [Required(ErrorMessage = "Plan is required")]
    [RegularExpression(@"^(Basic|Standard|Premium)$", ErrorMessage = "Plan must be Basic, Standard, or Premium")]
    public required string Plan { get; set; }
  }
}
