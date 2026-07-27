using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ubuntu_health_api.Helpers;
using ubuntu_health_api.Models.DTO;
using ubuntu_health_api.Services;

namespace ubuntu_health_api.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class InvoicesController(IInvoiceService invoiceService
  , IHttpContextAccessor httpContextAccessor
  , IMapper mapper) : ControllerBase
  {
    private readonly IInvoiceService _invoiceService = invoiceService;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IMapper _mapper = mapper;

    [Authorize(Roles = "admin, receptionist")]
    [HttpGet]
    public async Task<IActionResult> GetAllInvoices()
    {
      if (_httpContextAccessor.HttpContext == null) return Forbid();
      var tenantId = TenantHelper.GetTenantId(_httpContextAccessor.HttpContext);
      if (tenantId == null) return Forbid();

      var invoices = await _invoiceService.GetAllInvoicesAsync(tenantId);
      return Ok(invoices);
    }

    [Authorize(Roles = "admin, receptionist")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetInvoiceById(int id)
    {
      if (_httpContextAccessor.HttpContext == null) return Forbid();
      var tenantId = TenantHelper.GetTenantId(_httpContextAccessor.HttpContext);
      if (tenantId == null) return Forbid();

      var invoice = await _invoiceService.GetInvoiceByIdAsync(id, tenantId);
      if (invoice == null) return NotFound();

      return Ok(invoice);
    }

    [Authorize(Roles = "admin, receptionist")]
    [HttpPost]
    public async Task<IActionResult> AddInvoice([FromBody] InvoiceCreateDto invoice)
    {
      if (_httpContextAccessor.HttpContext == null) return Forbid();
      var tenantId = TenantHelper.GetTenantId(_httpContextAccessor.HttpContext);
      if (tenantId == null) return Forbid();

      var created = await _invoiceService.AddInvoiceAsync(invoice, tenantId);
      return CreatedAtAction(nameof(GetInvoiceById), new { id = created.Id }, created);
    }

    [Authorize(Roles = "admin, receptionist")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateInvoice(int id, [FromBody] InvoiceUpdateDto invoice)
    {
      if (_httpContextAccessor.HttpContext == null) return Forbid();
      var tenantId = TenantHelper.GetTenantId(_httpContextAccessor.HttpContext);
      if (tenantId == null) return Forbid();

      var updatedInvoice = await _invoiceService.UpdateInvoiceAsync(id, invoice, tenantId);
      if (updatedInvoice == null) return NotFound();

      return Ok(_mapper.Map<InvoiceResponseDto>(updatedInvoice));
    }

    [Authorize(Roles = "admin, receptionist")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteInvoice(int id)
    {
      if (_httpContextAccessor.HttpContext == null) return Forbid();
      var tenantId = TenantHelper.GetTenantId(_httpContextAccessor.HttpContext);
      if (tenantId == null) return Forbid();

      var invoice = await _invoiceService.GetInvoiceByIdAsync(id, tenantId);
      if (invoice == null) return NotFound();

      await _invoiceService.DeleteInvoiceAsync(id, tenantId);
      return NoContent();
    }
  }
}

