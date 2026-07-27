using ubuntu_health_api.Models.DTO;

namespace ubuntu_health_api.Services
{
  public interface IExportService
  {
    Task<PracticeExportDto> ExportAsync(string tenantId, CancellationToken cancellationToken = default);
  }
}
