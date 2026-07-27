using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ubuntu_health_api.Models.DTO;
using ubuntu_health_api.Services;

namespace ubuntu_health_api.Controllers
{
  [AllowAnonymous]
  [ApiController]
  [Route("api/[controller]")]
  public class InvitationsController(IStaffService staffService,
  ILogger<InvitationsController> logger) : ControllerBase
  {
    private readonly IStaffService _staffService = staffService;
    private readonly ILogger<InvitationsController> _logger = logger;

    [HttpGet("{token}")]
    public async Task<ActionResult<InvitationPreviewDto>> GetInvitation(string token, CancellationToken cancellationToken)
    {
      var invitation = await _staffService.GetInvitationPreviewAsync(token, cancellationToken);
      return Ok(invitation);
    }

    [HttpPost("{token}/accept")]
    public async Task<ActionResult> AcceptInvitation(string token, [FromBody] AcceptInvitationDto request, CancellationToken cancellationToken)
    {
      if (!ModelState.IsValid) return BadRequest(ModelState);

      await _staffService.AcceptInvitationAsync(token, request, cancellationToken);
      _logger.LogInformation("Invitation accepted");

      return Ok(new AuthResponseDto
      {
        IsSuccess = true,
        Message = "Account created successfully! Please sign in."
      });
    }
  }
}
