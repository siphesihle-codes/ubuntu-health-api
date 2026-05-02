using ubuntu_health_api.Models;

namespace ubuntu_health_api.Repositories
{
  public interface IPatientRepository
  {
    Task<IEnumerable<Patient>> GetAllPatientsAsync(string tenantId, CancellationToken cancellationToken);
    Task<Patient> GetPatientByIdAsync(int id, string tenantId);
    Task AddPatientAsync(Patient patient);
    Task UpdatePatientAsync(Patient patient, string tenantId);
    Task DeletePatientAsync(int id, string tenantId);
  }
}