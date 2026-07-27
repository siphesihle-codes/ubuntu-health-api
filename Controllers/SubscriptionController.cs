using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ubuntu_health_api.Helpers;
using ubuntu_health_api.Models.DTO;
using ubuntu_health_api.Services;

namespace ubuntu_health_api.Controllers
{
  [Authorize]
  [ApiController]
  [Route("api/[controller]")]
  public class SubscriptionController(ISubscriptionService subscriptionService,
  IHttpContextAccessor httpContextAccessor,
  ILogger<SubscriptionController> logger) : ControllerBase
  {
    private readonly ISubscriptionService _subscriptionService = subscriptionService;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly ILogger<SubscriptionController> _logger = logger;

    [HttpGet]
    public async Task<ActionResult<SubscriptionDto>> GetSubscription(CancellationToken cancellationToken)
    {
      var tenantId = GetTenantId();
      if (tenantId == null) return Forbid();

      var subscription = await _subscriptionService.GetSubscriptionAsync(tenantId, cancellationToken);
      return Ok(subscription);
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpPost("upgrade")]
    public async Task<ActionResult<SubscriptionDto>> Upgrade([FromBody] UpgradeSubscriptionDto request, CancellationToken cancellationToken)
    {
      if (!ModelState.IsValid) return BadRequest(ModelState);

      var tenantId = GetTenantId();
      if (tenantId == null) return Forbid();

      var subscription = await _subscriptionService.UpgradeAsync(tenantId, request.Plan, cancellationToken);
      _logger.LogInformation("Tenant {TenantId} upgraded to the {Plan} plan", tenantId, request.Plan);

      return Ok(subscription);
    }

    private string? GetTenantId()
    {
      if (_httpContextAccessor.HttpContext == null) return null;
      return TenantHelper.GetTenantId(_httpContextAccessor.HttpContext);
    }
  }
}
