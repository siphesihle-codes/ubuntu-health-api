using Microsoft.EntityFrameworkCore;
using ubuntu_health_api.Data;
using ubuntu_health_api.Models;

namespace ubuntu_health_api.Repositories
{
  public class InvitationRepository(AppDbContext dbContext) : IInvitationRepository
  {
    private readonly AppDbContext _dbContext = dbContext;

    public async Task<IEnumerable<Invitation>> GetPendingInvitationsAsync(string tenantId, CancellationToken cancellationToken = default)
    {
      return await _dbContext.Invitations
        .Where(i => i.TenantId == tenantId && i.AcceptedAt == null && i.RevokedAt == null)
        .OrderByDescending(i => i.CreatedAt)
        .ToListAsync(cancellationToken);
    }

    public async Task<Invitation?> GetPendingByEmailAsync(string tenantId, string email, CancellationToken cancellationToken = default)
    {
      return await _dbContext.Invitations
        .FirstOrDefaultAsync(
          i => i.TenantId == tenantId
            && i.Email == email
            && i.AcceptedAt == null
            && i.RevokedAt == null
            && i.ExpiresAt > DateTime.UtcNow,
          cancellationToken);
    }

    public async Task<Invitation?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default)
    {
      return await _dbContext.Invitations
        .FirstOrDefaultAsync(i => i.TokenHash == tokenHash, cancellationToken);
    }

    public async Task<Invitation?> GetByIdAsync(int id, string tenantId, CancellationToken cancellationToken = default)
    {
      return await _dbContext.Invitations
        .FirstOrDefaultAsync(i => i.Id == id && i.TenantId == tenantId, cancellationToken);
    }

    public async Task AddInvitationAsync(Invitation invitation)
    {
      await _dbContext.Invitations.AddAsync(invitation);
      await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateInvitationAsync(Invitation invitation)
    {
      invitation.UpdatedAt = DateTime.UtcNow;
      _dbContext.Invitations.Update(invitation);
      await _dbContext.SaveChangesAsync();
    }
  }
}
