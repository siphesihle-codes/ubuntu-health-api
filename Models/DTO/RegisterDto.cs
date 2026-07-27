using System.ComponentModel.DataAnnotations;

namespace ubuntu_health_api.Models.DTO
{
  public class RegisterDto
  {
    [Required(ErrorMessage = "First name is required")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 50 characters")]
    public required string FirstName { get; set; }

    [Required(ErrorMessage = "Last name is required")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 50 characters")]
    public required string LastName { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 12, ErrorMessage = "Password must be between 12 and 100 characters")]
    public required string Password { get; set; }

    [Required(ErrorMessage = "Practice name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Practice name must be between 2 and 100 characters")]
    public required string PracticeName { get; set; }

    [Phone(ErrorMessage = "Invalid practice phone number format")]
    [StringLength(15, ErrorMessage = "Practice phone cannot exceed 15 characters")]
    public string? PracticePhone { get; set; }

    [Required(ErrorMessage = "Subscription plan is required")]
    [RegularExpression(@"^(Basic|Premium|Enterprise)$", ErrorMessage = "Subscription plan must be Basic, Premium, or Enterprise")]
    public required string SubscriptionPlan { get; set; }
  }
}
