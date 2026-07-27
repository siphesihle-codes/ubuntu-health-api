using System.ComponentModel.DataAnnotations;

namespace ubuntu_health_api.Models.DTO
{
  public class CreateInvitationDto
  {
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "Role is required")]
    [RegularExpression(@"^(admin|doctor|nurse|receptionist)$", ErrorMessage = "Role must be admin, doctor, nurse, or receptionist")]
    public required string Role { get; set; }
  }
}
