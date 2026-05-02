using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ubuntu_health_api.Models;
using ubuntu_health_api.Models.DTO;

namespace ubuntu_health_api.Controllers
{
  [Route("api/[controller]")]
  [ApiController]

  public class AuthController(
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager,
    IConfiguration configuration) : ControllerBase
  {
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IConfiguration _configuration = configuration;

    [HttpPost("create-role")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> CreateRole([FromBody] string Roles)
    {
      if (string.IsNullOrWhiteSpace(Roles))
      {
        return BadRequest(new AuthResponseDto
        {
          IsSuccess = false,
          Message = "Role name cannot be empty"
        });
      }

      var roleExists = await _roleManager.RoleExistsAsync(Roles);
      if (roleExists)
      {
        return BadRequest(new AuthResponseDto
        {
          IsSuccess = false,
          Message = "Role already exists"
        });
      }

      var result = await _roleManager.CreateAsync(new IdentityRole(Roles));
      if (!result.Succeeded)
      {
        return BadRequest(new AuthResponseDto
        {
          IsSuccess = false,
          Message = string.Join(", ", result.Errors.Select(e => e.Description))
        });
      }

      return Ok(new AuthResponseDto
      {
        IsSuccess = true,
        Message = "Role created successfully"
      });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto request)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(new AuthResponseDto
        {
          IsSuccess = false,
          Message = "Invalid request",
        });
      }

      var allowedRoles = new[] { "admin", "doctor", "nurse", "receptionist" };
      if (!allowedRoles.Contains(request.Role))
      {
        return BadRequest(new AuthResponseDto
        {
          IsSuccess = false,
          Message = "Invalid role selection"
        });
      }

      var userExists = await _userManager.FindByEmailAsync(request.Email);
      if (userExists != null)
        return BadRequest(new AuthResponseDto
        {
          IsSuccess = false,
          Message = "User already exists!",
        });

      var tenantId = string.IsNullOrEmpty(request.TenantId)
        ? $"org-{Guid.NewGuid().ToString()[..8]}"
        : request.TenantId;


      var user = new ApplicationUser
      {
        TenantId = tenantId,
        FirstName = request.FirstName,
        LastName = request.LastName,
        Email = request.Email,
        UserName = request.Email,
        SubscriptionPlan = request.SubscriptionPlan,
        SecurityStamp = Guid.NewGuid().ToString(),
      };

      var result = await _userManager.CreateAsync(user, request.Password);
      if (!result.Succeeded)
        return BadRequest(new AuthResponseDto
        {
          IsSuccess = false,
          Message = string.Join(", ", result.Errors.Select(e => e.Description))
        });


      var roleAssignment = await _userManager.AddToRoleAsync(user, request.Role);
      if (!roleAssignment.Succeeded)
      {
        await _userManager.DeleteAsync(user);
        return BadRequest(new AuthResponseDto
        {
          IsSuccess = false,
          Message = $"Role assignment failed: {string.Join(", ", roleAssignment.Errors)}"
        });
      }

      return Ok(new AuthResponseDto
      {
        IsSuccess = true,
        TenantId = tenantId,
        Message = "User created successfully!",
        Roles = [request.Role]
      });
    }

    [HttpPost("assign-role")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> AssignRoleToUser([FromBody] AssignRoleDto request)
    {
      var user = await _userManager.FindByEmailAsync(request.Email);
      if (user == null)
      {
        return NotFound(new AuthResponseDto
        {
          IsSuccess = false,
          Message = "User not found"
        });
      }

      var currentUserEmail = User.FindFirst(ClaimTypes.Email)?.Value;
      if (currentUserEmail == null) return Unauthorized();
      var currentUser = await _userManager.FindByEmailAsync(currentUserEmail);
      if (currentUser == null) return Unauthorized();

      if (User.IsInRole("admin") && user.TenantId != currentUser.TenantId)
      {
        return NotFound();
      }

      foreach (var role in request.Roles)
      {
        var roleExists = await _roleManager.RoleExistsAsync(role.ToLower());
        if (!roleExists)
        {
          return BadRequest(new AuthResponseDto
          {
            IsSuccess = false,
            Message = $"Role '{role.ToLower()}' does not exist"
          });
        }
      }

      if (request.Roles.Any(role => _userManager.IsInRoleAsync(user, role.ToLower()).Result))
      {
        return BadRequest(new AuthResponseDto
        {
          IsSuccess = false,
          Message = "User already has this role"
        });
      }

      foreach (var role in request.Roles)
      {
        var lowerCaseRole = role.ToLower();
        var result = await _userManager.AddToRoleAsync(user, lowerCaseRole);
        if (!result.Succeeded)
        {
          return BadRequest(new AuthResponseDto
          {
            IsSuccess = false,
            Message = string.Join(", ", result.Errors.Select(e => e.Description))
          });
        }
      }

      return Ok(new AuthResponseDto
      {
        IsSuccess = true,
        Message = $"Role {request.Roles} assigned to user successfully"
      });
    }

    [HttpPost("remove-role")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> RemoveRoleFromUser([FromBody] AssignRoleDto request)
    {
      var user = await _userManager.FindByEmailAsync(request.Email);
      if (user == null)
      {
        return NotFound(new AuthResponseDto
        {
          IsSuccess = false,
          Message = "User not found"
        });
      }

      var currentUserEmail = User.FindFirst(ClaimTypes.Email)?.Value;
      if (currentUserEmail == null) return Unauthorized();
      var currentUser = await _userManager.FindByEmailAsync(currentUserEmail);
      if (currentUser == null) return Unauthorized();

      if (User.IsInRole("admin") && user.TenantId != currentUser.TenantId)
      {
        return Forbid();
      }

      if (request.Roles.Contains("Admin"))
      {
        var adminUsers = await _userManager.GetUsersInRoleAsync("admin");
        if (adminUsers.Count <= 1 && adminUsers.Contains(user))
        {
          return BadRequest(new AuthResponseDto
          {
            IsSuccess = false,
            Message = "Cannot remove the last system administrator"
          });
        }
      }

      if (!request.Roles.All(role => _userManager.IsInRoleAsync(user, role).Result))
      {
        return BadRequest(new AuthResponseDto
        {
          IsSuccess = false,
          Message = "User does not have this role"
        });
      }

      foreach (var role in request.Roles)
      {
        var result = await _userManager.RemoveFromRoleAsync(user, role);
        if (!result.Succeeded)
        {
          return BadRequest(new AuthResponseDto
          {
            IsSuccess = false,
            Message = string.Join(", ", result.Errors.Select(e => e.Description))
          });
        }
      }

      return Ok(new AuthResponseDto
      {
        IsSuccess = true,
        Message = $"Role {request.Roles} removed from user successfully"
      });
    }

    [HttpGet("user-roles")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> GetUserRoles(string email)
    {
      var user = await _userManager.FindByEmailAsync(email);
      if (user == null)
      {
        return NotFound(new AuthResponseDto
        {
          IsSuccess = false,
          Message = "User not found"
        });
      }

      // Security check for tenant boundaries
      var currentUserEmail = User.FindFirst(ClaimTypes.Email)?.Value;
      if (string.IsNullOrEmpty(currentUserEmail))
      {
        return Unauthorized(new AuthResponseDto
        {
          IsSuccess = false,
          Message = "Current user email is not available"
        });
      }

      var currentUser = await _userManager.FindByEmailAsync(currentUserEmail);
      if (currentUser == null)
      {
        return Unauthorized(new AuthResponseDto
        {
          IsSuccess = false,
          Message = "Current user not found"
        });
      }


      if (User.IsInRole("admin") && user.TenantId != currentUser.TenantId)
      {
        return Forbid();
      }

      var roles = await _userManager.GetRolesAsync(user);
      var lowercaseRoles = roles.Select(r => r.ToLower()).ToList();

      return Ok(new
      {
        IsSuccess = true,
        Email = email,
        Roles = lowercaseRoles
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

      var userRoles = await _userManager.GetRolesAsync(user);
      var lowerCaseRoles = userRoles.Select(r => r.ToLower()).ToList();

      var authClaims = new List<Claim>
      {
        new(ClaimTypes.Email, user.Email ?? string.Empty),
        new("TenantId", user.TenantId ?? string.Empty),
        new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
      };

      foreach (var role in lowerCaseRoles)
      {
        authClaims.Add(new Claim(ClaimTypes.Role, role));
      }

      var token = GenerateJwtToken(authClaims);

      return Ok(new AuthResponseDto
      {
        IsSuccess = true,
        Token = new JwtSecurityTokenHandler().WriteToken(token),
        RefreshToken = null,
        Message = "Login successful",
        Email = user.Email,
        TenantId = user.TenantId,
        Roles = userRoles,
      });
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