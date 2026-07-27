using ubuntu_health_api.Models;
using ubuntu_health_api.Models.DTO;

namespace ubuntu_health_api.Services
{
  public interface IPrescriptionService
  {
    Task<IEnumerable<PrescriptionResponseDto>> GetAllPrescriptionsAsync(string tenantId);
    Task<PrescriptionResponseDto> GetPrescriptionByIdAsync(int id, string tenantId);
    Task<PrescriptionResponseDto> AddPrescriptionAsync(PrescriptionCreateDto prescription, string tenantId, string prescriberId);
    Task<PrescriptionResponseDto> UpdatePrescriptionAsync(int id, PrescriptionUpdateDto prescription, string tenantId);
    Task<bool> DeletePrescriptionAsync(int id, string tenantId);
  }
}