using Microsoft.AspNetCore.Identity;

namespace ubuntu_health_api.Models
{
  public class ApplicationUser : IdentityUser
  {
    public string? TenantId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Phone { get; set; }
    public string? LicenseNumber { get; set; }
    public string? Specialty { get; set; }
    public bool IsOwner { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
  }
}
