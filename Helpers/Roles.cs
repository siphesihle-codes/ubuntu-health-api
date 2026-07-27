namespace ubuntu_health_api.Helpers
{
  public static class Roles
  {
    public const string Admin = "admin";
    public const string Doctor = "doctor";
    public const string Nurse = "nurse";
    public const string Receptionist = "receptionist";

    public static readonly string[] All = [Admin, Doctor, Nurse, Receptionist];

    public static readonly string[] Prescribing = [Admin, Doctor];

    public static bool IsKnown(string role) => All.Contains(role);
  }
}
