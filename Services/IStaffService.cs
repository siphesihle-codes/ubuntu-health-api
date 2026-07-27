using ubuntu_health_api.Models.DTO;

namespace ubuntu_health_api.Services
{
  public interface IStaffService
  {
    Task<IEnumerable<StaffMemberDto>> GetStaffAsync(string tenantId, CancellationToken cancellationToken = default);
    Task<StaffMemberDto> UpdateStaffRoleAsync(string actingUserId, string staffId, string role, string tenantId, CancellationToken cancellationToken = default);
    Task<StaffMemberDto> SetStaffActiveAsync(string actingUserId, string staffId, bool isActive, string tenantId, CancellationToken cancellationToken = default);
    Task<IEnumerable<InvitationDto>> GetInvitationsAsync(string tenantId, CancellationToken cancellationToken = default);
    Task<InvitationCreatedDto> CreateInvitationAsync(string actingUserId, CreateInvitationDto createDto, string tenantId, CancellationToken cancellationToken = default);
    Task RevokeInvitationAsync(int id, string tenantId, CancellationToken cancellationToken = default);
    Task<InvitationPreviewDto> GetInvitationPreviewAsync(string token, CancellationToken cancellationToken = default);
    Task AcceptInvitationAsync(string token, AcceptInvitationDto acceptDto, CancellationToken cancellationToken = default);
  }
}
