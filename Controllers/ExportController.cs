using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ubuntu_health_api.Helpers;
using ubuntu_health_api.Models.DTO;
using ubuntu_health_api.Services;

namespace ubuntu_health_api.Controllers
{
  [Authorize(Roles = Roles.Admin)]
  [ApiController]
  [Route("api/[controller]")]
  public class ExportController(IExportService exportService,
  IHttpContextAccessor httpContextAccessor,
  ILogger<ExportController> logger) : ControllerBase
  {
    private readonly IExportService _exportService = exportService;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly ILogger<ExportController> _logger = logger;

    [HttpGet]
    public async Task<ActionResult<PracticeExportDto>> ExportPractice(CancellationToken cancellationToken)
    {
      var tenantId = GetTenantId();
      if (tenantId == null) return Forbid();

      var export = await _exportService.ExportAsync(tenantId, cancellationToken);
      _logger.LogInformation("Practice export generated for tenant {TenantId}", tenantId);

      return Ok(export);
    }

    private string? GetTenantId()
    {
      if (_httpContextAccessor.HttpContext == null) return null;
      return TenantHelper.GetTenantId(_httpContextAccessor.HttpContext);
    }
  }
}
