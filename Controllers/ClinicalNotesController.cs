using Microsoft.AspNetCore.Mvc;
using ubuntu_health_api.Models;
using ubuntu_health_api.Services;
using Microsoft.AspNetCore.Authorization;
using ubuntu_health_api.Helpers;
using ubuntu_health_api.Models.DTO;
using AutoMapper;

namespace ubuntu_health_api.Controllers
{
  [Authorize]
  [ApiController]
  [Route("api/[controller]")]
  public class ClinicalNotesController(IClinicalNoteService clinicalNoteService
  , IHttpContextAccessor httpContextAccessor, IMapper mapper) : ControllerBase
  {
    private readonly IClinicalNoteService _clinicalNoteService = clinicalNoteService;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IMapper _mapper = mapper;

    [Authorize(Roles = "admin,doctor,nurse")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClinicalNoteResponseDto>>> GetAllClinicalNotes()
    {
      if (_httpContextAccessor.HttpContext == null) return Forbid();
      var tenantId = TenantHelper.GetTenantId(_httpContextAccessor.HttpContext);
      if (tenantId == null) return Forbid();

      var clinicalNotes = await _clinicalNoteService.GetAllClinicalNotesAsync(tenantId);
      return Ok(clinicalNotes);
    }

    [Authorize(Roles = "admin,doctor,nurse")]
    [HttpGet("{id}")]
    public async Task<ActionResult<ClinicalNoteResponseDto>> GetClinicalNoteById(int id)
    {
      if (_httpContextAccessor.HttpContext == null) return Forbid();
      var tenantId = TenantHelper.GetTenantId(_httpContextAccessor.HttpContext);
      if (tenantId == null) return Forbid();

      var clinicalNote = await _clinicalNoteService.GetClinicalNoteByIdAsync(id, tenantId);
      if (clinicalNote == null) return NotFound();

      return Ok(clinicalNote);
    }

    [Authorize(Roles = "admin,doctor,nurse")]
    [HttpPost]
    public async Task<ActionResult> AddClinicalNote([FromBody] ClinicalNoteCreateDto clinicalNote)
    {
      if (_httpContextAccessor.HttpContext == null) return Forbid();
      var tenantId = TenantHelper.GetTenantId(_httpContextAccessor.HttpContext);
      var doctorId = CurrentUser.GetId(_httpContextAccessor.HttpContext);
      if (tenantId == null || doctorId == null) return Forbid();

      var created = await _clinicalNoteService.AddClinicalNoteAsync(clinicalNote, tenantId, doctorId);
      return CreatedAtAction(nameof(GetClinicalNoteById), new { id = created.Id }, created);
    }

    [Authorize(Roles = "admin,doctor,nurse")]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteClinicalNote(int id)
    {
      if (_httpContextAccessor.HttpContext == null) return Forbid();
      var tenantId = TenantHelper.GetTenantId(_httpContextAccessor.HttpContext);
      if (tenantId == null) return Forbid();

      var result = await _clinicalNoteService.DeleteClinicalNoteAsync(id, tenantId);
      if (!result) return NotFound();

      return NoContent();
    }

    [Authorize(Roles = "admin,doctor,nurse")]
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateClinicalNote(int id, [FromBody] ClinicalNoteUpdateDto clinicalNote)
    {
      if (_httpContextAccessor.HttpContext == null) return Forbid();
      var tenantId = TenantHelper.GetTenantId(_httpContextAccessor.HttpContext);
      if (tenantId == null) return Forbid();

      var updated = await _clinicalNoteService.UpdateClinicalNoteAsync(id, clinicalNote, tenantId);
      if (updated == null) return NotFound();

      return Ok(updated);
    }
  }
}