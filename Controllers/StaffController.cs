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
  public class StaffController(IStaffService staffService,
  IHttpContextAccessor httpContextAccessor,
  ILogger<StaffController> logger) : ControllerBase
  {
    private readonly IStaffService _staffService = staffService;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly ILogger<StaffController> _logger = logger;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<StaffMemberDto>>> GetStaff(CancellationToken cancellationToken)
    {
      var tenantId = GetTenantId();
      if (tenantId == null) return Forbid();

      var staff = await _staffService.GetStaffAsync(tenantId, cancellationToken);
      return Ok(staff);
    }

    [HttpPut("{id}/role")]
    public async Task<ActionResult<StaffMemberDto>> UpdateStaffRole(string id, [FromBody] UpdateStaffRoleDto request, CancellationToken cancellationToken)
    {
      if (!ModelState.IsValid) return BadRequest(ModelState);

      var tenantId = GetTenantId();
      var actingUserId = GetActingUserId();
      if (tenantId == null || actingUserId == null) return Forbid();

      var staffMember = await _staffService.UpdateStaffRoleAsync(actingUserId, id, request.Role, tenantId, cancellationToken);
      _logger.LogInformation("Role for staff member {StaffId} changed to {Role} in tenant {TenantId}", id, request.Role, tenantId);

      return Ok(staffMember);
    }

    [HttpPut("{id}/status")]
    public async Task<ActionResult<StaffMemberDto>> UpdateStaffStatus(string id, [FromBody] UpdateStaffStatusDto request, CancellationToken cancellationToken)
    {
      var tenantId = GetTenantId();
      var actingUserId = GetActingUserId();
      if (tenantId == null || actingUserId == null) return Forbid();

      var staffMember = await _staffService.SetStaffActiveAsync(actingUserId, id, request.IsActive, tenantId, cancellationToken);
      _logger.LogInformation("Staff member {StaffId} active set to {IsActive} in tenant {TenantId}", id, request.IsActive, tenantId);

      return Ok(staffMember);
    }

    [HttpGet("invitations")]
    public async Task<ActionResult<IEnumerable<InvitationDto>>> GetInvitations(CancellationToken cancellationToken)
    {
      var tenantId = GetTenantId();
      if (tenantId == null) return Forbid();

      var invitations = await _staffService.GetInvitationsAsync(tenantId, cancellationToken);
      return Ok(invitations);
    }

    [HttpPost("invitations")]
    public async Task<ActionResult<InvitationCreatedDto>> CreateInvitation([FromBody] CreateInvitationDto request, CancellationToken cancellationToken)
    {
      if (!ModelState.IsValid) return BadRequest(ModelState);

      var tenantId = GetTenantId();
      var actingUserId = GetActingUserId();
      if (tenantId == null || actingUserId == null) return Forbid();

      var invitation = await _staffService.CreateInvitationAsync(actingUserId, request, tenantId, cancellationToken);
      _logger.LogInformation("Invitation {InvitationId} created for role {Role} in tenant {TenantId}", invitation.Id, invitation.Role, tenantId);

      return Ok(invitation);
    }

    [HttpDelete("invitations/{id}")]
    public async Task<ActionResult> RevokeInvitation(int id, CancellationToken cancellationToken)
    {
      var tenantId = GetTenantId();
      if (tenantId == null) return Forbid();

      await _staffService.RevokeInvitationAsync(id, tenantId, cancellationToken);
      _logger.LogInformation("Invitation {InvitationId} revoked in tenant {TenantId}", id, tenantId);

      return NoContent();
    }

    private string? GetTenantId()
    {
      if (_httpContextAccessor.HttpContext == null) return null;
      return TenantHelper.GetTenantId(_httpContextAccessor.HttpContext);
    }

    private string? GetActingUserId()
    {
      if (_httpContextAccessor.HttpContext == null) return null;
      return CurrentUser.GetId(_httpContextAccessor.HttpContext);
    }
  }
}
