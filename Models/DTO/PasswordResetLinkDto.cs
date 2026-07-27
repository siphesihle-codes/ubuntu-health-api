namespace ubuntu_health_api.Models.DTO
{
  public class PasswordResetLinkDto
  {
    public required string UserId { get; set; }
    public required string Email { get; set; }
    public required string Token { get; set; }
    public DateTime ExpiresAt { get; set; }
  }
}
