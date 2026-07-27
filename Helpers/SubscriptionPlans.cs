namespace ubuntu_health_api.Helpers
{
  public static class SubscriptionPlans
  {
    public const string Free = "Free";
    public const string Basic = "Basic";
    public const string Standard = "Standard";
    public const string Premium = "Premium";

    public const int TrialLengthDays = 30;

    public static readonly string[] All = [Free, Basic, Standard, Premium];

    public static readonly string[] Paid = [Basic, Standard, Premium];

    public static bool IsKnown(string plan) => All.Contains(plan);

    public static bool IsPaid(string plan) => Paid.Contains(plan);

    public static DateTime? TrialEndFor(string plan) =>
      plan == Free ? DateTime.UtcNow.AddDays(TrialLengthDays) : null;

    public static bool IsTrialExpired(DateTime? trialEndsAt) =>
      trialEndsAt.HasValue && trialEndsAt.Value <= DateTime.UtcNow;

    public static int TrialDaysRemaining(DateTime? trialEndsAt) =>
      trialEndsAt.HasValue
        ? Math.Max((int)Math.Ceiling((trialEndsAt.Value - DateTime.UtcNow).TotalDays), 0)
        : 0;
  }
}
