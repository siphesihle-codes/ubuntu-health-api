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
  public class ImportController(IImportService importService,
  IHttpContextAccessor httpContextAccessor,
  ILogger<ImportController> logger) : ControllerBase
  {
    private readonly IImportService _importService = importService;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly ILogger<ImportController> _logger = logger;

    [HttpPost]
    public async Task<ActionResult<ImportSummaryDto>> ImportPractice([FromBody] PracticeImportDto request, CancellationToken cancellationToken)
    {
      var tenantId = GetTenantId();
      if (tenantId == null) return Forbid();

      var summary = await _importService.ImportAsync(request, tenantId, cancellationToken);
      _logger.LogInformation(
        "Practice import for tenant {TenantId} created {Patients} patients and {Appointments} appointments",
        tenantId, summary.PatientsCreated, summary.AppointmentsCreated);

      return Ok(summary);
    }

    private string? GetTenantId()
    {
      if (_httpContextAccessor.HttpContext == null) return null;
      return TenantHelper.GetTenantId(_httpContextAccessor.HttpContext);
    }
  }
}
