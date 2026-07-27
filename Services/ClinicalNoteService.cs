using AutoMapper;
using ubuntu_health_api.Models;
using ubuntu_health_api.Models.DTO;
using ubuntu_health_api.Repositories;

namespace ubuntu_health_api.Services
{
  public class ClinicalNoteService(IClinicalNoteRepository clinicalNoteRepository, IMapper mapper) : IClinicalNoteService
  {
    private readonly IClinicalNoteRepository _clinicalNoteRepository = clinicalNoteRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<IEnumerable<ClinicalNoteResponseDto>> GetAllClinicalNotesAsync(string tenantId)
    {
      var clinicalNote = await _clinicalNoteRepository.GetAllClinicalNotesAsync(tenantId)
        ?? throw new KeyNotFoundException("No Clinical Notes Found");
      return _mapper.Map<IEnumerable<ClinicalNoteResponseDto>>(clinicalNote);
    }

    public async Task<ClinicalNoteResponseDto> GetClinicalNoteByIdAsync(int id, string tenantId)
    {
      var clinicalNote = await _clinicalNoteRepository.GetClinicalNoteByIdAsync(id, tenantId) ??
      throw new KeyNotFoundException($"Clinical note with {id} not found.");
      return _mapper.Map<ClinicalNoteResponseDto>(clinicalNote);
    }

    public async Task<ClinicalNoteResponseDto> AddClinicalNoteAsync(ClinicalNoteCreateDto createDto, string tenantId, string doctorId)
    {
      var clinicalNote = _mapper.Map<ClinicalNote>(createDto);
      clinicalNote.TenantId = tenantId;
      clinicalNote.DoctorId = doctorId;
      clinicalNote.CreatedAt = DateTime.UtcNow;
      clinicalNote.UpdatedAt = DateTime.UtcNow;

      await _clinicalNoteRepository.AddClinicalNoteAsync(clinicalNote);
      return _mapper.Map<ClinicalNoteResponseDto>(clinicalNote);
    }

    public async Task<ClinicalNoteResponseDto> UpdateClinicalNoteAsync(int id, ClinicalNoteUpdateDto updateDto, string tenantId)
    {
      var existingClinicalNote = await _clinicalNoteRepository
        .GetClinicalNoteByIdAsync(id, tenantId)
         ?? throw new KeyNotFoundException("Clinical Note not found");

      _mapper.Map(updateDto, existingClinicalNote);
      existingClinicalNote.UpdatedAt = DateTime.UtcNow;

      await _clinicalNoteRepository.UpdateClinicalNoteAsync(existingClinicalNote, tenantId);
      return _mapper.Map<ClinicalNoteResponseDto>(existingClinicalNote);
    }

    public async Task<bool> DeleteClinicalNoteAsync(int id, string tenantId)
    {
      try
      {
        await _clinicalNoteRepository.DeleteClinicalNoteAsync(id, tenantId);
        return true;
      }
      catch
      {
        return false;
      }

    }
  }
}