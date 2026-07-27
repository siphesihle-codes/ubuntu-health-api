namespace ubuntu_health_api.Models.DTO
{
  public class PractitionerDto
  {
    public required string Id { get; set; }
    public required string Name { get; set; }
    public string? Specialty { get; set; }
  }
}
