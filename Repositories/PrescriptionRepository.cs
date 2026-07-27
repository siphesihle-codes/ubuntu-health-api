using ubuntu_health_api.Models;
using ubuntu_health_api.Data;
using Microsoft.EntityFrameworkCore;

namespace ubuntu_health_api.Repositories
{
  public class PrescriptionRepository(AppDbContext dbContext) : IPrescriptionRepository
  {
    private readonly AppDbContext _dbContext = dbContext;

    public async Task<IEnumerable<Prescription>> GetAllPrescriptionsAsync(string tenantId)
    {
      return await _dbContext.Prescriptions
      .Include(p => p.Medications)
      .Where(p => p.TenantId == tenantId)
      .ToListAsync();
    }

    public async Task<Prescription> GetPrescriptionByIdAsync(int id, string tenantId)
    {
      var prescription = await _dbContext.Prescriptions
      .Include(p => p.Medications)
      .FirstOrDefaultAsync(
        p => p.Id == id && p.TenantId == tenantId
      ) ?? throw new KeyNotFoundException(
        $"Prescription with ID {id} was not found."
      );

      return prescription;
    }

    public async Task AddPrescriptionAsync(Prescription prescription)
    {
      await _dbContext.Prescriptions.AddAsync(prescription);
      await _dbContext.SaveChangesAsync();
    }


    public async Task UpdatePrescriptionAsync(Prescription prescription, string tenantId)
    {
      var existing = await _dbContext.Prescriptions
      .FirstOrDefaultAsync(e => e.Id == prescription.Id && e.TenantId == tenantId)
      ?? throw new InvalidOperationException("Prescription not fond or tenant mismatch");

      existing.EndDate = prescription.EndDate;
      existing.Frequency = prescription.Frequency;
      existing.Refills = prescription.Refills;
      existing.Status = prescription.Status;
      existing.Instructions = prescription.Instructions;
      existing.UpdatedAt = DateTime.UtcNow;

      await _dbContext.SaveChangesAsync();
    }

    public async Task DeletePrescriptionAsync(int id, string tenantId)
    {
      var prescription = await _dbContext.Prescriptions
      .FirstOrDefaultAsync(p => p.Id == id && p.TenantId == tenantId);

      if (prescription != null)
      {
        _dbContext.Prescriptions.Remove(prescription);
        await _dbContext.SaveChangesAsync();
      }
    }
  }
}