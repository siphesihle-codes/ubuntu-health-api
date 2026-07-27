using Microsoft.EntityFrameworkCore;
using ubuntu_health_api.Data;
using ubuntu_health_api.Models;

namespace ubuntu_health_api.Repositories
{
  public class PracticeRepository(AppDbContext dbContext) : IPracticeRepository
  {
    private readonly AppDbContext _dbContext = dbContext;

    public async Task<Practice?> GetByTenantIdAsync(string tenantId, CancellationToken cancellationToken = default)
    {
      return await _dbContext.Practices
        .FirstOrDefaultAsync(p => p.TenantId == tenantId, cancellationToken);
    }

    public async Task AddPracticeAsync(Practice practice)
    {
      await _dbContext.Practices.AddAsync(practice);
      await _dbContext.SaveChangesAsync();
    }

    public async Task UpdatePracticeAsync(Practice practice)
    {
      practice.UpdatedAt = DateTime.UtcNow;
      _dbContext.Practices.Update(practice);
      await _dbContext.SaveChangesAsync();
    }
  }
}
