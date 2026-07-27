using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ubuntu_health_api.Exceptions;
using ubuntu_health_api.Helpers;
using ubuntu_health_api.Models;
using ubuntu_health_api.Models.DTO;
using ubuntu_health_api.Repositories;

namespace ubuntu_health_api.Services
{
  public class SubscriptionService(
    UserManager<ApplicationUser> userManager,
    IInvitationRepository invitationRepository,
    IPracticeRepository practiceRepository) : ISubscriptionService
  {
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IInvitationRepository _invitationRepository = invitationRepository;
    private readonly IPracticeRepository _practiceRepository = practiceRepository;

    public async Task<SubscriptionDto> GetSubscriptionAsync(string tenantId, CancellationToken cancellationToken = default)
    {
      var practice = await GetPracticeAsync(tenantId, cancellationToken);
      var practitionersInUse = await GetPractitionersInUseAsync(tenantId, cancellationToken);

      return MapSubscription(practice, practitionersInUse);
    }

    public async Task<SubscriptionDto> UpgradeAsync(string tenantId, string plan, CancellationToken cancellationToken = default)
    {
      if (!SubscriptionPlans.IsPaid(plan))
      {
        throw new ValidationException($"Plan '{plan}' is not a paid plan");
      }

      var practice = await GetPracticeAsync(tenantId, cancellationToken);
      var practitionersInUse = await GetPractitionersInUseAsync(tenantId, cancellationToken);
      var seats = SubscriptionPlans.PractitionerSeats(plan);

      if (practitionersInUse > seats)
      {
        throw new ValidationException(
          $"The {plan} plan includes {SeatLabel(seats)} and your practice is using {practitionersInUse}. Choose a larger plan or remove a practitioner first.");
      }

      practice.SubscriptionPlan = plan;
      practice.TrialEndsAt = null;
      await _practiceRepository.UpdatePracticeAsync(practice);

      return MapSubscription(practice, practitionersInUse);
    }

    public async Task EnsurePractitionerSeatAvailableAsync(string tenantId, CancellationToken cancellationToken = default)
    {
      var practice = await GetPracticeAsync(tenantId, cancellationToken);
      var seats = SubscriptionPlans.PractitionerSeats(practice.SubscriptionPlan);
      var practitionersInUse = await GetPractitionersInUseAsync(tenantId, cancellationToken);

      if (practitionersInUse >= seats)
      {
        throw new ValidationException(
          $"Your plan includes {SeatLabel(seats)} and all of them are in use. Upgrade your plan to add another practitioner.");
      }
    }

    private async Task<int> GetPractitionersInUseAsync(string tenantId, CancellationToken cancellationToken)
    {
      var users = await _userManager.Users
        .Where(u => u.TenantId == tenantId && u.IsActive)
        .ToListAsync(cancellationToken);

      var practitioners = 0;
      foreach (var user in users)
      {
        var roles = await _userManager.GetRolesAsync(user);
        if (roles.Any(Roles.Prescribing.Contains))
        {
          practitioners++;
        }
      }

      var pendingInvitations = await _invitationRepository.GetPendingInvitationsAsync(tenantId, cancellationToken);

      return practitioners + pendingInvitations.Count(
        i => i.ExpiresAt > DateTime.UtcNow && Roles.Prescribing.Contains(i.Role));
    }

    private async Task<Practice> GetPracticeAsync(string tenantId, CancellationToken cancellationToken)
    {
      return await _practiceRepository.GetByTenantIdAsync(tenantId, cancellationToken)
        ?? throw new NotFoundException($"Practice for tenant {tenantId} was not found.");
    }

    private static string SeatLabel(int seats) =>
      seats == 1 ? "1 practitioner" : $"{seats} practitioners";

    private static SubscriptionDto MapSubscription(Practice practice, int practitionersInUse)
    {
      return new SubscriptionDto
      {
        Plan = practice.SubscriptionPlan,
        TrialEndsAt = practice.TrialEndsAt,
        TrialDaysRemaining = SubscriptionPlans.TrialDaysRemaining(practice.TrialEndsAt),
        IsTrialExpired = SubscriptionPlans.IsTrialExpired(practice.TrialEndsAt),
        PractitionerSeats = SubscriptionPlans.PractitionerSeats(practice.SubscriptionPlan),
        PractitionersInUse = practitionersInUse
      };
    }
  }
}
