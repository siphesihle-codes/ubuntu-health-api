using ubuntu_health_api.Models;

namespace ubuntu_health_api.Repositories
{
  public interface IPracticeRepository
  {
    Task<Practice?> GetByTenantIdAsync(string tenantId, CancellationToken cancellationToken = default);
    Task AddPracticeAsync(Practice practice);
    Task UpdatePracticeAsync(Practice practice);
  }
}
