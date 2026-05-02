namespace ubuntu_health_api.Models.DTO
{
  public class AssignRoleDto
  {
    public required string Email { get; set; }
    public required IEnumerable<string> Roles { get; set; }
  }
}
