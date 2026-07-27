namespace ubuntu_health_api.Models.DTO
{
  public class SubscriptionDto
  {
    public required string Plan { get; set; }
    public DateTime? TrialEndsAt { get; set; }
    public int TrialDaysRemaining { get; set; }
    public bool IsTrialExpired { get; set; }
    public int PractitionerSeats { get; set; }
    public int PractitionersInUse { get; set; }
  }
}
