using AutoMapper;
using Microsoft.AspNetCore.Identity;
using ubuntu_health_api.Models;
using ubuntu_health_api.Models.DTO;
using ubuntu_health_api.Repositories;

namespace ubuntu_health_api.Services
{
  public class PrescriptionService(
    IPrescriptionRepository prescriptionRepository,
    UserManager<ApplicationUser> userManager,
    IMapper mapper) : IPrescriptionService
  {
    private readonly IPrescriptionRepository _prescriptionRepository = prescriptionRepository;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IMapper _mapper = mapper;

    public async Task<IEnumerable<PrescriptionResponseDto>> GetAllPrescriptionsAsync(string tenantId)
    {
      var prescriptions = await _prescriptionRepository.GetAllPrescriptionsAsync(tenantId) ??
      throw new KeyNotFoundException("No Prescriptions Found");
      return _mapper.Map<IEnumerable<PrescriptionResponseDto>>(prescriptions);
    }

    public async Task<PrescriptionResponseDto> GetPrescriptionByIdAsync(int id, string tenantId)
    {
      var prescription = await _prescriptionRepository.GetPrescriptionByIdAsync(id, tenantId) ??
      throw new KeyNotFoundException("Prescription not found");
      return _mapper.Map<PrescriptionResponseDto>(prescription);
    }

    public async Task<PrescriptionResponseDto> AddPrescriptionAsync(PrescriptionCreateDto createDto, string tenantId, string prescriberId)
    {
      var prescriber = await _userManager.FindByIdAsync(prescriberId)
        ?? throw new KeyNotFoundException("Prescriber not found");

      var prescription = _mapper.Map<Prescription>(createDto);
      prescription.TenantId = tenantId;
      prescription.PrescriberId = prescriber.Id;
      prescription.PrescriberName = $"{prescriber.FirstName} {prescriber.LastName}".Trim();
      prescription.PrescriberLicenseNumber = prescriber.LicenseNumber;
      prescription.CreatedAt = DateTime.UtcNow;
      prescription.UpdatedAt = DateTime.UtcNow;

      await _prescriptionRepository.AddPrescriptionAsync(prescription);

      return _mapper.Map<PrescriptionResponseDto>(prescription);
    }

    public async Task<PrescriptionResponseDto> UpdatePrescriptionAsync(int id, PrescriptionUpdateDto updateDto, string tenantId)
    {
      var existingPrescription = await _prescriptionRepository.GetPrescriptionByIdAsync(id, tenantId)
      ?? throw new KeyNotFoundException("Prescription not found");

      _mapper.Map(updateDto, existingPrescription);
      existingPrescription.UpdatedAt = DateTime.UtcNow;

      await _prescriptionRepository.UpdatePrescriptionAsync(existingPrescription, tenantId);
      return _mapper.Map<PrescriptionResponseDto>(existingPrescription);
    }

    public async Task<bool> DeletePrescriptionAsync(int id, string tenantId)
    {
      try
      {
        await _prescriptionRepository.DeletePrescriptionAsync(id, tenantId);
        return true;
      }
      catch
      {
        return false;
      }
    }
  }
}