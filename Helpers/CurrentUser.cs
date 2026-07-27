using System.Security.Claims;

namespace ubuntu_health_api.Helpers
{
  public class CurrentUser
  {
    public const string LicenseNumberClaim = "LicenseNumber";

    public static string? GetId(HttpContext context)
    {
      return context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }

    public static string? GetLicenseNumber(HttpContext context)
    {
      return context.User?.FindFirst(LicenseNumberClaim)?.Value;
    }
  }
}
