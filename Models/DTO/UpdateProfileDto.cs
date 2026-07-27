using System.ComponentModel.DataAnnotations;

namespace ubuntu_health_api.Models.DTO
{
  public class UpdateProfileDto
  {
    [Required(ErrorMessage = "First name is required")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 50 characters")]
    public required string FirstName { get; set; }

    [Required(ErrorMessage = "Last name is required")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 50 characters")]
    public required string LastName { get; set; }

    [Phone(ErrorMessage = "Invalid phone number format")]
    [StringLength(15, ErrorMessage = "Phone cannot exceed 15 characters")]
    public string? Phone { get; set; }

    [StringLength(50, ErrorMessage = "License number cannot exceed 50 characters")]
    public string? LicenseNumber { get; set; }

    [StringLength(100, ErrorMessage = "Specialty cannot exceed 100 characters")]
    public string? Specialty { get; set; }
  }
}
