using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;

namespace ubuntu_health_api.Helpers
{
  public static class InvitationToken
  {
    public static string Create() =>
      WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(32));

    public static string Hash(string token) =>
      Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
  }
}
