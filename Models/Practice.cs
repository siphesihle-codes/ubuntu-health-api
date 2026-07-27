namespace ubuntu_health_api.Models
{
  public class Practice
  {
    public int Id { get; set; }
    public required string TenantId { get; set; }
    public required string Name { get; set; }
    public string? Phone { get; set; }
    public required string SubscriptionPlan { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
  }
}
