using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using ubuntu_health_api.Data;
using ubuntu_health_api.Models;
using ubuntu_health_api.Models.DTO;

namespace ubuntu_health_api.Repositories
{
  public class InvoiceRepository(AppDbContext dbContext) : IInvoiceRepository
  {
    private readonly AppDbContext _dbContext = dbContext;

    public async Task<IEnumerable<Invoice>> GetAllInvoicesAsync(string tenantId)
    {
      return await _dbContext.Invoices
      .Where(i => i.TenantId == tenantId)
      .ToListAsync();
    }

    public async Task<Invoice> GetInvoiceByIdAsync(int id, string tenantId)
    {
      var invoice = await _dbContext.Invoices
        .FirstOrDefaultAsync(i => i.Id == id && i.TenantId == tenantId)
        ?? throw new KeyNotFoundException($"Invoice with ID {id} was not found.");
      return invoice;
    }

    public async Task AddInvoiceAsync(Invoice invoice)
    {
      invoice.CreatedAt = DateTime.UtcNow;
      invoice.UpdatedAt = DateTime.UtcNow;
      await _dbContext.Invoices.AddAsync(invoice);
      await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateInvoiceAsync(Invoice invoice, string tenantId)
    {
      var existing = await _dbContext.Invoices
      .FirstOrDefaultAsync(e => e.Id == invoice.Id && e.TenantId == tenantId)
      ?? throw new InvalidOperationException("Invoice not found");

      existing.PatientId = invoice.PatientId;
      existing.PatientFirstName = invoice.PatientFirstName;
      existing.PatientLastName = invoice.PatientLastName;
      existing.TotalAmount = invoice.TotalAmount;
      existing.Status = invoice.Status;
      existing.Notes = invoice.Notes;
      existing.DueDate = invoice.DueDate;
      existing.UpdatedAt = DateTime.UtcNow;

      await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteInvoiceAsync(int id, string tenantId)
    {
      var invoice = await _dbContext.Invoices
        .FirstOrDefaultAsync(i => i.Id == id && i.TenantId == tenantId);

      if (invoice != null)
      {
        _dbContext.Invoices.Remove(invoice);
        await _dbContext.SaveChangesAsync();
      }
    }
  }
}