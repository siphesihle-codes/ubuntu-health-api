using System.ComponentModel.DataAnnotations;

namespace ubuntu_health_api.Models.DTO
{
  public class UpdateStaffRoleDto
  {
    [Required(ErrorMessage = "Role is required")]
    [RegularExpression(@"^(admin|doctor|nurse|receptionist)$", ErrorMessage = "Role must be admin, doctor, nurse, or receptionist")]
    public required string Role { get; set; }
  }
}
