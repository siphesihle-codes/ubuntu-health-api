using ubuntu_health_api.Models.DTO;

namespace ubuntu_health_api.Services
{
  public interface ISubscriptionService
  {
    Task<SubscriptionDto> GetSubscriptionAsync(string tenantId, CancellationToken cancellationToken = default);
    Task<SubscriptionDto> UpgradeAsync(string tenantId, string plan, CancellationToken cancellationToken = default);
    Task EnsurePractitionerSeatAvailableAsync(string tenantId, CancellationToken cancellationToken = default);
  }
}
