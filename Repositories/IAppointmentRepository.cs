using ubuntu_health_api.Models;
using ubuntu_health_api.Models.DTO;

namespace ubuntu_health_api.Repositories
{
  public interface IAppointmentRepository
  {
    Task<PagedResult<Appointment>> GetPaginatedAppointmentsAsync(string tenantId, int page, int pageSize);
    Task<IEnumerable<Appointment>> GetAllAppointmentsAsync(string tenantId);
    Task<IEnumerable<Appointment>> GetAppointmentsByDateRangeAsync(string tenantId, string fromDate, string toDate);
    Task<Appointment> GetAppointmentByIdAsync(int id, string tenantId);
    Task AddAppointmentAsync(Appointment appointment);
    Task UpdateAppointmentAsync(Appointment appointment, string tenantId);
    Task DeleteAppointmentAsync(int id, string tenantId);
  }
}