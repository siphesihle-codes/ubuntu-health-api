using ubuntu_health_api.Models.DTO;

namespace ubuntu_health_api.Services
{
  public interface IImportService
  {
    Task<ImportSummaryDto> ImportAsync(PracticeImportDto import, string tenantId, CancellationToken cancellationToken = default);
  }
}
