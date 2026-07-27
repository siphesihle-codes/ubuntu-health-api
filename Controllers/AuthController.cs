using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ubuntu_health_api.Helpers;
using ubuntu_health_api.Models;
using ubuntu_health_api.Models.DTO;
using ubuntu_health_api.Repositories;
using ubuntu_health_api.Services;

namespace ubuntu_health_api.Controllers
{
  [Route("api/[controller]")]
  [ApiController]

  public class AuthController(
    UserManager<ApplicationUser> userManager,
    IPracticeRepository practiceRepository,
    IStaffService staffService,
    IConfiguration configuration) : ControllerBase
  {
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IPracticeRepository _practiceRepository = practiceRepository;
    private readonly IStaffService _staffService = staffService;
    private readonly IConfiguration _configuration = configuration;

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto request)
    {
      if (!ModelState.IsValid)
      {
        var errors = ModelState.Values
          .SelectMany(v => v.Errors)
          .Select(e => e.ErrorMessage)
          .ToList();

        return BadRequest(new AuthResponseDto
        {
          IsSuccess = false,
          Message = errors.Count > 0 ? string.Join(" ", errors) : "Invalid request",
          Errors = errors
        });
      }

      var userExists = await _userManager.FindByEmailAsync(request.Email);
      if (userExists != null)
        return BadRequest(new AuthResponseDto
        {
          IsSuccess = false,
          Message = "User already exists!",
        });

      var tenantId = $"org-{Guid.NewGuid().ToString()[..8]}";

      var user = new ApplicationUser
      {
        TenantId = tenantId,
        FirstName = request.FirstName,
        LastName = request.LastName,
        Email = request.Email,
        UserName = request.Email,
        IsOwner = true,
        IsActive = true,
        SecurityStamp = Guid.NewGuid().ToString(),
      };

      var result = await _userManager.CreateAsync(user, request.Password);
      if (!result.Succeeded)
        return BadRequest(new AuthResponseDto
        {
          IsSuccess = false,
          Message = string.Join(", ", result.Errors.Select(e => e.Description))
        });

      var roleAssignment = await _userManager.AddToRoleAsync(user, Roles.Admin);
      if (!roleAssignment.Succeeded)
      {
        await _userManager.DeleteAsync(user);
        return BadRequest(new AuthResponseDto
        {
          IsSuccess = false,
          Message = $"Role assignment failed: {string.Join(", ", roleAssignment.Errors.Select(e => e.Description))}"
        });
      }

      try
      {
        await _practiceRepository.AddPracticeAsync(new Practice
        {
          TenantId = tenantId,
          Name = request.PracticeName,
          Phone = request.PracticePhone,
          SubscriptionPlan = request.SubscriptionPlan,
          TrialEndsAt = SubscriptionPlans.TrialEnd(),
          CreatedAt = DateTime.UtcNow,
          UpdatedAt = DateTime.UtcNow
        });
      }
      catch
      {
        await _userManager.DeleteAsync(user);
        throw;
      }

      return Ok(new AuthResponseDto
      {
        IsSuccess = true,
        TenantId = tenantId,
        Email = user.Email,
        Message = "Practice created successfully!",
        Roles = [Roles.Admin]
      });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto request)
    {
      var user = await _userManager.FindByEmailAsync(request.Email);
      if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
        return Unauthorized(new AuthResponseDto
        {
          IsSuccess = false,
          Message = "Invalid email or password"
        });

      if (!user.IsActive)
        return Unauthorized(new AuthResponseDto
        {
          IsSuccess = false,
          Message = "This account has been deactivated. Contact your practice administrator."
        });

      var userRoles = await _userManager.GetRolesAsync(user);
      var lowerCaseRoles = userRoles.Select(r => r.ToLower()).ToList();

      var authClaims = new List<Claim>
      {
        new(ClaimTypes.NameIdentifier, user.Id),
        new(ClaimTypes.Email, user.Email ?? string.Empty),
        new("TenantId", user.TenantId ?? string.Empty),
        new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
      };

      foreach (var role in lowerCaseRoles)
      {
        authClaims.Add(new Claim(ClaimTypes.Role, role));
      }

      var token = GenerateJwtToken(authClaims);

      Response.Cookies.Append(
        AuthCookie.Name,
        new JwtSecurityTokenHandler().WriteToken(token),
        AuthCookie.CreateOptions(token.ValidTo)
      );

      return Ok(new AuthResponseDto
      {
        IsSuccess = true,
        RefreshToken = null,
        Message = "Login successful",
        Email = user.Email,
        TenantId = user.TenantId,
        Roles = lowerCaseRoles,
      });
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
      Response.Cookies.Delete(AuthCookie.Name, AuthCookie.CreateOptions());

      return Ok(new AuthResponseDto
      {
        IsSuccess = true,
        Message = "Logout successful"
      });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto request, CancellationToken cancellationToken)
    {
      if (!ModelState.IsValid) return BadRequest(ModelState);

      await _staffService.ResetPasswordAsync(request, cancellationToken);

      return Ok(new AuthResponseDto
      {
        IsSuccess = true,
        Message = "Password updated. Please sign in."
      });
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UserProfileDto>> GetProfile(CancellationToken cancellationToken)
    {
      var user = await GetAuthenticatedUserAsync();
      if (user == null) return Unauthorized();

      var roles = (await _userManager.GetRolesAsync(user))
        .Select(r => r.ToLower())
        .ToList();

      var practice = user.TenantId == null
        ? null
        : await _practiceRepository.GetByTenantIdAsync(user.TenantId, cancellationToken);

      return Ok(new UserProfileDto
      {
        Id = user.Id,
        Email = user.Email,
        FirstName = user.FirstName,
        LastName = user.LastName,
        Phone = user.Phone,
        LicenseNumber = user.LicenseNumber,
        Specialty = user.Specialty,
        TenantId = user.TenantId,
        IsOwner = user.IsOwner,
        IsActive = user.IsActive,
        Roles = roles,
        PracticeName = practice?.Name,
        SubscriptionPlan = practice?.SubscriptionPlan,
        TrialEndsAt = practice?.TrialEndsAt,
        TrialDaysRemaining = SubscriptionPlans.TrialDaysRemaining(practice?.TrialEndsAt),
        IsTrialExpired = SubscriptionPlans.IsTrialExpired(practice?.TrialEndsAt),
        RequiresProfessionalDetails =
          roles.Any(Roles.Prescribing.Contains) && string.IsNullOrWhiteSpace(user.LicenseNumber)
      });
    }

    [HttpPut("me")]
    [Authorize]
    public async Task<ActionResult<UserProfileDto>> UpdateProfile([FromBody] UpdateProfileDto request, CancellationToken cancellationToken)
    {
      if (!ModelState.IsValid) return BadRequest(ModelState);

      var user = await GetAuthenticatedUserAsync();
      if (user == null) return Unauthorized();

      user.FirstName = request.FirstName;
      user.LastName = request.LastName;
      user.Phone = request.Phone;
      user.LicenseNumber = request.LicenseNumber;
      user.Specialty = request.Specialty;
      user.UpdatedAt = DateTime.UtcNow;

      var result = await _userManager.UpdateAsync(user);
      if (!result.Succeeded)
      {
        return BadRequest(new AuthResponseDto
        {
          IsSuccess = false,
          Message = string.Join(", ", result.Errors.Select(e => e.Description))
        });
      }

      return await GetProfile(cancellationToken);
    }

    private async Task<ApplicationUser?> GetAuthenticatedUserAsync()
    {
      var userId = CurrentUser.GetId(HttpContext);
      if (string.IsNullOrEmpty(userId)) return null;

      return await _userManager.FindByIdAsync(userId);
    }

    private JwtSecurityToken GenerateJwtToken(List<Claim> authClaims)
    {
      var authSigningKey = new SymmetricSecurityKey(
        Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]!)
      );

      var token = new JwtSecurityToken(
        issuer: _configuration["JWT:ValidIssuer"],
        audience: _configuration["JWT:ValidAudience"],
        expires: DateTime.Now.AddHours(4),
        claims: authClaims,
        signingCredentials: new SigningCredentials(
            authSigningKey, SecurityAlgorithms.HmacSha256)
      );

      return token;
    }
  }
}
