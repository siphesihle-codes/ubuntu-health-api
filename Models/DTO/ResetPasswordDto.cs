using System.ComponentModel.DataAnnotations;

namespace ubuntu_health_api.Models.DTO
{
  public class ResetPasswordDto
  {
    [Required(ErrorMessage = "User is required")]
    public required string UserId { get; set; }

    [Required(ErrorMessage = "Reset token is required")]
    public required string Token { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 12, ErrorMessage = "Password must be between 12 and 100 characters")]
    public required string Password { get; set; }
  }
}
