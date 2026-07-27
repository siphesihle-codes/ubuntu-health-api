using Microsoft.AspNetCore.Mvc;
using ubuntu_health_api.Models;
using ubuntu_health_api.Services;
using Microsoft.AspNetCore.Authorization;
using ubuntu_health_api.Helpers;
using ubuntu_health_api.Models.DTO;
using AutoMapper;
using ubuntu_health_api.Exceptions;

namespace ubuntu_health_api.Controllers
{
  [Authorize]
  [ApiController]
  [Route("api/[controller]")]
  public class PrescriptionsController(IPrescriptionService prescriptionService,
  IHttpContextAccessor httpContextAccessor,
  IMapper mapper) : ControllerBase
  {
    private readonly IPrescriptionService _prescriptionService = prescriptionService;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IMapper _mapper = mapper;

    [Authorize(Roles = "admin, doctor, nurse")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Prescription>>> GetAllPrescriptions()
    {
      if (_httpContextAccessor.HttpContext == null) return Forbid();
      var tenantId = TenantHelper.GetTenantId(_httpContextAccessor.HttpContext);
      if (tenantId == null) return Forbid();

      var prescriptions = await _prescriptionService.GetAllPrescriptionsAsync(tenantId);
      return Ok(prescriptions);
    }

    [Authorize(Roles = "admin, doctor, nurse")]
    [HttpGet("{id}")]
    public async Task<ActionResult<Prescription>> GetPrescriptionById(int id)
    {
      if (_httpContextAccessor.HttpContext == null) return Forbid();
      var tenantId = TenantHelper.GetTenantId(_httpContextAccessor.HttpContext);
      if (tenantId == null) return Forbid();

      var prescription = await _prescriptionService.GetPrescriptionByIdAsync(id, tenantId);
      if (prescription == null)
      {
        return NotFound();
      }
      return Ok(prescription);
    }

    [Authorize(Roles = "admin, doctor")]
    [HttpPost]
    public async Task<ActionResult> AddPrescription([FromBody] PrescriptionCreateDto prescription)
    {
      if (_httpContextAccessor.HttpContext == null) return Forbid();
      var tenantId = TenantHelper.GetTenantId(_httpContextAccessor.HttpContext);
      var prescriberId = CurrentUser.GetId(_httpContextAccessor.HttpContext);
      if (tenantId == null || prescriberId == null) return Forbid();

      if (string.IsNullOrWhiteSpace(CurrentUser.GetLicenseNumber(_httpContextAccessor.HttpContext)))
      {
        throw new ValidationException("Add your medical license number to your profile before writing prescriptions");
      }

      var created = await _prescriptionService.AddPrescriptionAsync(prescription, tenantId, prescriberId);
      return CreatedAtAction(nameof(GetPrescriptionById), new { id = created.Id }, created);
    }

    [Authorize(Roles = "admin, doctor")]
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdatePrescription(int id, [FromBody] PrescriptionUpdateDto prescription)
    {
      if (_httpContextAccessor.HttpContext == null) return Forbid();
      var tenantId = TenantHelper.GetTenantId(_httpContextAccessor.HttpContext);
      if (tenantId == null) return Forbid();

      var updatedAppointment = await _prescriptionService.UpdatePrescriptionAsync(id, prescription, tenantId);
      if (updatedAppointment == null) return NotFound();

      return Ok(_mapper.Map<PrescriptionResponseDto>(updatedAppointment));
    }

    [Authorize(Roles = "admin, doctor")]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAppointment(int id)
    {
      if (_httpContextAccessor.HttpContext == null) return Forbid();
      var tenantId = TenantHelper.GetTenantId(_httpContextAccessor.HttpContext);
      if (tenantId == null) return Forbid();

      var prescription = await _prescriptionService.GetPrescriptionByIdAsync(id, tenantId);
      if (prescription == null)
      {
        return NotFound();
      }

      await _prescriptionService.DeletePrescriptionAsync(id, tenantId);
      return NoContent();
    }


  }
}