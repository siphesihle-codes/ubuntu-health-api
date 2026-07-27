namespace ubuntu_health_api.Models.DTO
{
  public class InvitationPreviewDto
  {
    public required string Email { get; set; }
    public required string Role { get; set; }
    public required string PracticeName { get; set; }
    public DateTime ExpiresAt { get; set; }
  }
}
