using ubuntu_health_api.Models;

namespace ubuntu_health_api.Repositories
{
  public interface IInvitationRepository
  {
    Task<IEnumerable<Invitation>> GetPendingInvitationsAsync(string tenantId, CancellationToken cancellationToken = default);
    Task<Invitation?> GetPendingByEmailAsync(string tenantId, string email, CancellationToken cancellationToken = default);
    Task<Invitation?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default);
    Task<Invitation?> GetByIdAsync(int id, string tenantId, CancellationToken cancellationToken = default);
    Task AddInvitationAsync(Invitation invitation);
    Task UpdateInvitationAsync(Invitation invitation);
  }
}
