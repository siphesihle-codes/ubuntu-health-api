using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ubuntu_health_api.Exceptions;
using ubuntu_health_api.Helpers;
using ubuntu_health_api.Models;
using ubuntu_health_api.Models.DTO;
using ubuntu_health_api.Repositories;

namespace ubuntu_health_api.Services
{
  public class StaffService(
    UserManager<ApplicationUser> userManager,
    IInvitationRepository invitationRepository,
    IPracticeRepository practiceRepository) : IStaffService
  {
    private const int InvitationLifetimeDays = 7;

    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IInvitationRepository _invitationRepository = invitationRepository;
    private readonly IPracticeRepository _practiceRepository = practiceRepository;

    public async Task<IEnumerable<StaffMemberDto>> GetStaffAsync(string tenantId, CancellationToken cancellationToken = default)
    {
      var users = await _userManager.Users
        .Where(u => u.TenantId == tenantId)
        .OrderByDescending(u => u.IsOwner)
        .ThenBy(u => u.CreatedAt)
        .ToListAsync(cancellationToken);

      var staff = new List<StaffMemberDto>();
      foreach (var user in users)
      {
        staff.Add(await MapStaffMemberAsync(user));
      }

      return staff;
    }

    public async Task<StaffMemberDto> UpdateStaffRoleAsync(string actingUserId, string staffId, string role, string tenantId, CancellationToken cancellationToken = default)
    {
      cancellationToken.ThrowIfCancellationRequested();

      if (!Roles.IsKnown(role))
      {
        throw new ValidationException($"Role '{role}' does not exist");
      }

      var actingUser = await GetTenantUserAsync(actingUserId, tenantId);
      var staffMember = await GetTenantUserAsync(staffId, tenantId);

      if (staffMember.IsOwner)
      {
        throw new ValidationException("The practice owner's role cannot be changed");
      }

      if (actingUser.Id == staffMember.Id)
      {
        throw new ValidationException("You cannot change your own role");
      }

      var currentRoles = await _userManager.GetRolesAsync(staffMember);
      var grantsOrRemovesAdmin = role == Roles.Admin || currentRoles.Contains(Roles.Admin);
      if (grantsOrRemovesAdmin && !actingUser.IsOwner)
      {
        throw new UnauthorizedAccessException("Only the practice owner can manage administrators");
      }

      if (currentRoles.Count == 1 && currentRoles[0] == role)
      {
        return await MapStaffMemberAsync(staffMember);
      }

      if (currentRoles.Count > 0)
      {
        var removal = await _userManager.RemoveFromRolesAsync(staffMember, currentRoles);
        if (!removal.Succeeded)
        {
          throw new ValidationException(string.Join(", ", removal.Errors.Select(e => e.Description)));
        }
      }

      var assignment = await _userManager.AddToRoleAsync(staffMember, role);
      if (!assignment.Succeeded)
      {
        throw new ValidationException(string.Join(", ", assignment.Errors.Select(e => e.Description)));
      }

      staffMember.UpdatedAt = DateTime.UtcNow;
      await _userManager.UpdateAsync(staffMember);

      return await MapStaffMemberAsync(staffMember);
    }

    public async Task<StaffMemberDto> SetStaffActiveAsync(string actingUserId, string staffId, bool isActive, string tenantId, CancellationToken cancellationToken = default)
    {
      cancellationToken.ThrowIfCancellationRequested();

      var actingUser = await GetTenantUserAsync(actingUserId, tenantId);
      var staffMember = await GetTenantUserAsync(staffId, tenantId);

      if (staffMember.IsOwner)
      {
        throw new ValidationException("The practice owner cannot be deactivated");
      }

      if (actingUser.Id == staffMember.Id)
      {
        throw new ValidationException("You cannot deactivate your own account");
      }

      var staffRoles = await _userManager.GetRolesAsync(staffMember);
      if (staffRoles.Contains(Roles.Admin) && !actingUser.IsOwner)
      {
        throw new UnauthorizedAccessException("Only the practice owner can manage administrators");
      }

      staffMember.IsActive = isActive;
      staffMember.UpdatedAt = DateTime.UtcNow;

      var result = await _userManager.UpdateAsync(staffMember);
      if (!result.Succeeded)
      {
        throw new ValidationException(string.Join(", ", result.Errors.Select(e => e.Description)));
      }

      await _userManager.UpdateSecurityStampAsync(staffMember);

      return await MapStaffMemberAsync(staffMember);
    }

    public async Task<IEnumerable<InvitationDto>> GetInvitationsAsync(string tenantId, CancellationToken cancellationToken = default)
    {
      var invitations = await _invitationRepository.GetPendingInvitationsAsync(tenantId, cancellationToken);

      return invitations.Select(invitation => new InvitationDto
      {
        Id = invitation.Id,
        Email = invitation.Email,
        Role = invitation.Role,
        InvitedByEmail = invitation.InvitedByEmail,
        ExpiresAt = invitation.ExpiresAt,
        CreatedAt = invitation.CreatedAt,
        IsExpired = invitation.ExpiresAt <= DateTime.UtcNow
      });
    }

    public async Task<InvitationCreatedDto> CreateInvitationAsync(string actingUserId, CreateInvitationDto createDto, string tenantId, CancellationToken cancellationToken = default)
    {
      cancellationToken.ThrowIfCancellationRequested();

      var role = createDto.Role.ToLowerInvariant();
      if (!Roles.IsKnown(role))
      {
        throw new ValidationException($"Role '{createDto.Role}' does not exist");
      }

      var actingUser = await GetTenantUserAsync(actingUserId, tenantId);
      if (role == Roles.Admin && !actingUser.IsOwner)
      {
        throw new UnauthorizedAccessException("Only the practice owner can invite administrators");
      }

      var email = createDto.Email.Trim();

      var existingUser = await _userManager.FindByEmailAsync(email);
      if (existingUser != null)
      {
        throw new ConflictException("An account with that email already exists");
      }

      var existingInvitation = await _invitationRepository.GetPendingByEmailAsync(tenantId, email, cancellationToken);
      if (existingInvitation != null)
      {
        throw new ConflictException("That email already has a pending invitation");
      }

      var token = InvitationToken.Create();
      var invitation = new Invitation
      {
        TenantId = tenantId,
        Email = email,
        Role = role,
        TokenHash = InvitationToken.Hash(token),
        InvitedByEmail = actingUser.Email ?? string.Empty,
        ExpiresAt = DateTime.UtcNow.AddDays(InvitationLifetimeDays),
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
      };

      await _invitationRepository.AddInvitationAsync(invitation);

      return new InvitationCreatedDto
      {
        Id = invitation.Id,
        Email = invitation.Email,
        Role = invitation.Role,
        ExpiresAt = invitation.ExpiresAt,
        Token = token
      };
    }

    public async Task RevokeInvitationAsync(int id, string tenantId, CancellationToken cancellationToken = default)
    {
      var invitation = await _invitationRepository.GetByIdAsync(id, tenantId, cancellationToken)
        ?? throw new NotFoundException("Invitation", id, tenantId);

      if (invitation.AcceptedAt != null)
      {
        throw new ValidationException("That invitation has already been accepted");
      }

      invitation.RevokedAt = DateTime.UtcNow;
      await _invitationRepository.UpdateInvitationAsync(invitation);
    }

    public async Task<InvitationPreviewDto> GetInvitationPreviewAsync(string token, CancellationToken cancellationToken = default)
    {
      var invitation = await GetUsableInvitationAsync(token, cancellationToken);

      var practice = await _practiceRepository.GetByTenantIdAsync(invitation.TenantId, cancellationToken)
        ?? throw new NotFoundException("Practice was not found for this invitation");

      return new InvitationPreviewDto
      {
        Email = invitation.Email,
        Role = invitation.Role,
        PracticeName = practice.Name,
        ExpiresAt = invitation.ExpiresAt
      };
    }

    public async Task AcceptInvitationAsync(string token, AcceptInvitationDto acceptDto, CancellationToken cancellationToken = default)
    {
      cancellationToken.ThrowIfCancellationRequested();

      var invitation = await GetUsableInvitationAsync(token, cancellationToken);

      var existingUser = await _userManager.FindByEmailAsync(invitation.Email);
      if (existingUser != null)
      {
        throw new ConflictException("An account with that email already exists");
      }

      var user = new ApplicationUser
      {
        TenantId = invitation.TenantId,
        FirstName = acceptDto.FirstName,
        LastName = acceptDto.LastName,
        Email = invitation.Email,
        UserName = invitation.Email,
        IsOwner = false,
        IsActive = true,
        SecurityStamp = Guid.NewGuid().ToString()
      };

      var result = await _userManager.CreateAsync(user, acceptDto.Password);
      if (!result.Succeeded)
      {
        throw new ValidationException(string.Join(", ", result.Errors.Select(e => e.Description)));
      }

      var roleAssignment = await _userManager.AddToRoleAsync(user, invitation.Role);
      if (!roleAssignment.Succeeded)
      {
        await _userManager.DeleteAsync(user);
        throw new ValidationException(string.Join(", ", roleAssignment.Errors.Select(e => e.Description)));
      }

      invitation.AcceptedAt = DateTime.UtcNow;
      await _invitationRepository.UpdateInvitationAsync(invitation);
    }

    private async Task<Invitation> GetUsableInvitationAsync(string token, CancellationToken cancellationToken)
    {
      if (string.IsNullOrWhiteSpace(token))
      {
        throw new NotFoundException("Invitation was not found");
      }

      var invitation = await _invitationRepository.GetByTokenHashAsync(InvitationToken.Hash(token), cancellationToken)
        ?? throw new NotFoundException("Invitation was not found");

      if (invitation.RevokedAt != null)
      {
        throw new ValidationException("That invitation has been revoked");
      }

      if (invitation.AcceptedAt != null)
      {
        throw new ValidationException("That invitation has already been used");
      }

      if (invitation.ExpiresAt <= DateTime.UtcNow)
      {
        throw new ValidationException("That invitation has expired");
      }

      return invitation;
    }

    private async Task<ApplicationUser> GetTenantUserAsync(string userId, string tenantId)
    {
      var user = await _userManager.FindByIdAsync(userId);
      if (user == null || user.TenantId != tenantId)
      {
        throw new NotFoundException("Staff member was not found");
      }

      return user;
    }

    private async Task<StaffMemberDto> MapStaffMemberAsync(ApplicationUser user)
    {
      var roles = await _userManager.GetRolesAsync(user);

      return new StaffMemberDto
      {
        Id = user.Id,
        Email = user.Email,
        FirstName = user.FirstName,
        LastName = user.LastName,
        Phone = user.Phone,
        LicenseNumber = user.LicenseNumber,
        Specialty = user.Specialty,
        IsOwner = user.IsOwner,
        IsActive = user.IsActive,
        Roles = roles.Select(r => r.ToLowerInvariant()).ToList(),
        CreatedAt = user.CreatedAt
      };
    }
  }
}
