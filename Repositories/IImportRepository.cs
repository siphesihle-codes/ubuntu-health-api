using ubuntu_health_api.Models;

namespace ubuntu_health_api.Repositories
{
  public interface IImportRepository
  {
    Task<Dictionary<string, int>> GetPatientIdsByIdNumberAsync(string tenantId, CancellationToken cancellationToken = default);
    Task AddPatientsAsync(IEnumerable<Patient> patients, CancellationToken cancellationToken = default);
    Task AddAppointmentsAsync(IEnumerable<Appointment> appointments, CancellationToken cancellationToken = default);
    Task AddClinicalNotesAsync(IEnumerable<ClinicalNote> clinicalNotes, CancellationToken cancellationToken = default);
    Task AddPrescriptionsAsync(IEnumerable<Prescription> prescriptions, CancellationToken cancellationToken = default);
    Task AddInvoicesAsync(IEnumerable<Invoice> invoices, CancellationToken cancellationToken = default);
  }
}
