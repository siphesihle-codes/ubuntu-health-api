namespace ubuntu_health_api.Models.DTO
{
  public class StaffMemberDto
  {
    public required string Id { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Phone { get; set; }
    public string? LicenseNumber { get; set; }
    public string? Specialty { get; set; }
    public bool IsOwner { get; set; }
    public bool IsActive { get; set; }
    public IEnumerable<string> Roles { get; set; } = [];
    public DateTime CreatedAt { get; set; }
  }
}
