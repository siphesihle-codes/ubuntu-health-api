using ubuntu_health_api.Models;
using ubuntu_health_api.Models.DTO;

namespace ubuntu_health_api.Services
{
  public interface IClinicalNoteService
  {
    Task<IEnumerable<ClinicalNoteResponseDto>> GetAllClinicalNotesAsync(string tenantId);
    Task<ClinicalNoteResponseDto> GetClinicalNoteByIdAsync(int id, string tenantId);
    Task<ClinicalNoteResponseDto> AddClinicalNoteAsync(ClinicalNoteCreateDto clinicalNote, string tenantId, string doctorId);
    Task<ClinicalNoteResponseDto> UpdateClinicalNoteAsync(int id, ClinicalNoteUpdateDto clinicalNote, string tenantId);
    Task<bool> DeleteClinicalNoteAsync(int id, string tenantId);
  }
}