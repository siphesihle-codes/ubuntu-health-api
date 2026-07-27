namespace ubuntu_health_api.Helpers
{
  public static class SubscriptionPlans
  {
    public const string Free = "Free";
    public const string Solo = "Solo";
    public const string Practice = "Practice";
    public const string Clinic = "Clinic";

    public const int TrialLengthDays = 30;

    public const int SoloSeats = 1;
    public const int PracticeSeats = 3;
    public const int ClinicSeats = 8;

    public static readonly string[] All = [Free, Solo, Practice, Clinic];

    public static readonly string[] Paid = [Solo, Practice, Clinic];

    public static bool IsKnown(string plan) => All.Contains(plan);

    public static bool IsPaid(string plan) => Paid.Contains(plan);

    public static int PractitionerSeats(string plan) => plan switch
    {
      Solo => SoloSeats,
      Practice => PracticeSeats,
      _ => ClinicSeats
    };

    public static DateTime TrialEnd() => DateTime.UtcNow.AddDays(TrialLengthDays);

    public static bool IsTrialExpired(DateTime? trialEndsAt) =>
      trialEndsAt.HasValue && trialEndsAt.Value <= DateTime.UtcNow;

    public static int TrialDaysRemaining(DateTime? trialEndsAt) =>
      trialEndsAt.HasValue
        ? Math.Max((int)Math.Ceiling((trialEndsAt.Value - DateTime.UtcNow).TotalDays), 0)
        : 0;
  }
}
