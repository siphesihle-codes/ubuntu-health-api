using Microsoft.EntityFrameworkCore;
using ubuntu_health_api.Data;
using ubuntu_health_api.Models;

namespace ubuntu_health_api.Repositories
{
  public class ImportRepository(AppDbContext dbContext) : IImportRepository
  {
    private readonly AppDbContext _dbContext = dbContext;

    public async Task<Dictionary<string, int>> GetPatientIdsByIdNumberAsync(string tenantId, CancellationToken cancellationToken = default)
    {
      var patients = await _dbContext.Patients
        .Where(p => p.TenantId == tenantId && p.IdNumber != null && p.IdNumber != "")
        .Select(p => new { p.Id, p.IdNumber })
        .ToListAsync(cancellationToken);

      return patients
        .GroupBy(p => p.IdNumber!)
        .ToDictionary(group => group.Key, group => group.First().Id);
    }

    public async Task AddPatientsAsync(IEnumerable<Patient> patients, CancellationToken cancellationToken = default)
    {
      await _dbContext.Patients.AddRangeAsync(patients, cancellationToken);
      await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task AddAppointmentsAsync(IEnumerable<Appointment> appointments, CancellationToken cancellationToken = default)
    {
      await _dbContext.Appointments.AddRangeAsync(appointments, cancellationToken);
      await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task AddClinicalNotesAsync(IEnumerable<ClinicalNote> clinicalNotes, CancellationToken cancellationToken = default)
    {
      await _dbContext.ClinicalNotes.AddRangeAsync(clinicalNotes, cancellationToken);
      await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task AddPrescriptionsAsync(IEnumerable<Prescription> prescriptions, CancellationToken cancellationToken = default)
    {
      await _dbContext.Prescriptions.AddRangeAsync(prescriptions, cancellationToken);
      await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task AddInvoicesAsync(IEnumerable<Invoice> invoices, CancellationToken cancellationToken = default)
    {
      await _dbContext.Invoices.AddRangeAsync(invoices, cancellationToken);
      await _dbContext.SaveChangesAsync(cancellationToken);
    }
  }
}
