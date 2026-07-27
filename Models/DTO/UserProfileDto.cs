namespace ubuntu_health_api.Models.DTO
{
  public class UserProfileDto
  {
    public required string Id { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Phone { get; set; }
    public string? LicenseNumber { get; set; }
    public string? Specialty { get; set; }
    public string? TenantId { get; set; }
    public bool IsOwner { get; set; }
    public bool IsActive { get; set; }
    public IEnumerable<string> Roles { get; set; } = [];
    public string? PracticeName { get; set; }
    public string? SubscriptionPlan { get; set; }
    public bool RequiresProfessionalDetails { get; set; }
  }
}
