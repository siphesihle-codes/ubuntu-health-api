namespace ubuntu_health_api.Helpers;

public static class AuthCookie
{
  public const string Name = "access_token";

  public static CookieOptions CreateOptions(DateTimeOffset? expires = null)
  {
    return new CookieOptions
    {
      HttpOnly = true,
      Secure = true,
      SameSite = SameSiteMode.None,
      Path = "/",
      Expires = expires
    };
  }
}
