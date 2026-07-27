using ubuntu_health_api.Models;
using ubuntu_health_api.Models.DTO;

namespace ubuntu_health_api.Services
{
  public interface IAppointmentService
  {
    Task<PagedResult<AppointmentResponseDto>> GetPaginatedAppointmentsAsync(string tenantId, int page, int pageSize);
    Task<IEnumerable<AppointmentResponseDto>> GetAllAppointmentsAsync(string tenantId);
    Task<IEnumerable<AppointmentResponseDto>> GetDiaryAsync(string tenantId, string fromDate, string toDate);
    Task<AppointmentResponseDto> GetAppointmentByIdAsync(int id, string tenantId);
    Task<AppointmentResponseDto> AddAppointmentAsync(AppointmentCreateDto appointment, string tenantId);
    Task<AppointmentResponseDto> UpdateAppointmentAsync(int id, AppointmentUpdateDto appointment, string tenantId);
    Task<bool> DeleteAppointmentAsync(int id, string tenantId);
  }
}