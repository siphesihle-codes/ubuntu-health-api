using System.Net;
using System.Text.Json;
using ubuntu_health_api.Helpers;
using ubuntu_health_api.Repositories;

namespace ubuntu_health_api.Middleware
{
  public class TrialExpiryMiddleware
  {
    private static readonly string[] AllowedPaths = ["/api/auth", "/api/subscription", "/api/invitations"];

    private readonly RequestDelegate _next;
    private readonly ILogger<TrialExpiryMiddleware> _logger;

    public TrialExpiryMiddleware(RequestDelegate next, ILogger<TrialExpiryMiddleware> logger)
    {
      _next = next;
      _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IPracticeRepository practiceRepository)
    {
      var tenantId = TenantHelper.GetTenantId(context);

      if (string.IsNullOrEmpty(tenantId) || IsAllowed(context.Request.Path))
      {
        await _next(context);
        return;
      }

      var practice = await practiceRepository.GetByTenantIdAsync(tenantId, context.RequestAborted);
      if (!SubscriptionPlans.IsTrialExpired(practice?.TrialEndsAt))
      {
        await _next(context);
        return;
      }

      _logger.LogInformation("Blocked {Path} for tenant {TenantId} because the free trial ended",
        context.Request.Path, tenantId);

      await WriteTrialExpiredAsync(context);
    }

    private static bool IsAllowed(PathString path)
    {
      return !path.StartsWithSegments("/api")
        || AllowedPaths.Any(allowed => path.StartsWithSegments(allowed));
    }

    private static async Task WriteTrialExpiredAsync(HttpContext context)
    {
      var response = new ErrorResponse
      {
        StatusCode = (int)HttpStatusCode.PaymentRequired,
        Message = $"Your {SubscriptionPlans.TrialLengthDays}-day free trial has ended. Upgrade to a paid plan to continue."
      };

      context.Response.StatusCode = response.StatusCode;
      context.Response.ContentType = "application/json";

      var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
      {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
      });

      await context.Response.WriteAsync(jsonResponse);
    }
  }
}
