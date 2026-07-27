namespace ubuntu_health_api.Models.DTO
{
  public class InvitationCreatedDto
  {
    public int Id { get; set; }
    public required string Email { get; set; }
    public required string Role { get; set; }
    public DateTime ExpiresAt { get; set; }
    public required string Token { get; set; }
  }
}
