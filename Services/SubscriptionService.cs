using ubuntu_health_api.Exceptions;
using ubuntu_health_api.Helpers;
using ubuntu_health_api.Models;
using ubuntu_health_api.Models.DTO;
using ubuntu_health_api.Repositories;

namespace ubuntu_health_api.Services
{
  public class SubscriptionService(IPracticeRepository practiceRepository) : ISubscriptionService
  {
    private readonly IPracticeRepository _practiceRepository = practiceRepository;

    public async Task<SubscriptionDto> GetSubscriptionAsync(string tenantId, CancellationToken cancellationToken = default)
    {
      var practice = await GetPracticeAsync(tenantId, cancellationToken);

      return MapSubscription(practice);
    }

    public async Task<SubscriptionDto> UpgradeAsync(string tenantId, string plan, CancellationToken cancellationToken = default)
    {
      if (!SubscriptionPlans.IsPaid(plan))
      {
        throw new ValidationException($"Plan '{plan}' is not a paid plan");
      }

      var practice = await GetPracticeAsync(tenantId, cancellationToken);

      practice.SubscriptionPlan = plan;
      practice.TrialEndsAt = null;
      await _practiceRepository.UpdatePracticeAsync(practice);

      return MapSubscription(practice);
    }

    private async Task<Practice> GetPracticeAsync(string tenantId, CancellationToken cancellationToken)
    {
      return await _practiceRepository.GetByTenantIdAsync(tenantId, cancellationToken)
        ?? throw new NotFoundException($"Practice for tenant {tenantId} was not found.");
    }

    private static SubscriptionDto MapSubscription(Practice practice)
    {
      return new SubscriptionDto
      {
        Plan = practice.SubscriptionPlan,
        TrialEndsAt = practice.TrialEndsAt,
        TrialDaysRemaining = SubscriptionPlans.TrialDaysRemaining(practice.TrialEndsAt),
        IsTrialExpired = SubscriptionPlans.IsTrialExpired(practice.TrialEndsAt)
      };
    }
  }
}
