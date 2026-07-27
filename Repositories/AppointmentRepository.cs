using ubuntu_health_api.Models;
using ubuntu_health_api.Data;
using Microsoft.EntityFrameworkCore;

namespace ubuntu_health_api.Repositories
{
  public class AppointmentRepository(AppDbContext context) : IAppointmentRepository
  {
    private readonly AppDbContext _dbContext = context;

    public async Task<PagedResult<Appointment>> GetPaginatedAppointmentsAsync(string tenantId, int page, int pageSize)
    {
      var query = _dbContext.Appointments
        .Where(a => a.TenantId == tenantId)
        .OrderBy(a => a.Id);

      var totalCount = await query.CountAsync();

      var items = await query
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

      return new PagedResult<Appointment>
      {
        Items = items,
        TotalCount = totalCount,
        Page = page,
        PageSize = pageSize
      };
    }

    public async Task<IEnumerable<Appointment>> GetAllAppointmentsAsync(string tenantId)
    {
      return await _dbContext.Appointments
          .Where(a => a.TenantId == tenantId)
          .ToListAsync();
    }

    public async Task<IEnumerable<Appointment>> GetAppointmentsByDateRangeAsync(string tenantId, string fromDate, string toDate)
    {
      return await _dbContext.Appointments
          .Where(a => a.TenantId == tenantId
            && string.Compare(a.AppointmentDate, fromDate) >= 0
            && string.Compare(a.AppointmentDate, toDate) <= 0)
          .OrderBy(a => a.AppointmentDate)
          .ThenBy(a => a.AppointmentTime)
          .ToListAsync();
    }

    public async Task<Appointment> GetAppointmentByIdAsync(int id, string tenantId)
    {
      var appointment = await _dbContext.Appointments
          .FirstOrDefaultAsync(a => a.Id == id && a.TenantId == tenantId)
          ?? throw new InvalidOperationException("Appointment not found or tenant mismatch");
      return appointment;
    }

    public async Task AddAppointmentAsync(Appointment appointment)
    {
      await _dbContext.Appointments.AddAsync(appointment);
      await _dbContext.SaveChangesAsync();
    }
    public async Task UpdateAppointmentAsync(Appointment appointment, string tenantId)
    {
      var existing = await _dbContext.Appointments
          .FirstOrDefaultAsync(a => a.Id == appointment.Id && a.TenantId == tenantId)
          ?? throw new InvalidOperationException("Appointment not found or tenant mismatch");

      existing.PatientFirstName = appointment.PatientFirstName;
      existing.PatientLastName = appointment.PatientLastName;
      existing.PractitionerId = appointment.PractitionerId;
      existing.PractitionerName = appointment.PractitionerName;
      existing.AppointmentDate = appointment.AppointmentDate;
      existing.AppointmentTime = appointment.AppointmentTime;
      existing.AppointmentType = appointment.AppointmentType;
      existing.Status = appointment.Status;
      existing.Notes = appointment.Notes;

      await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAppointmentAsync(int id, string tenantId)
    {
      var appointment = await _dbContext.Appointments
          .FirstOrDefaultAsync(a => a.Id == id && a.TenantId == tenantId);

      if (appointment != null)
      {
        _dbContext.Appointments.Remove(appointment);
        await _dbContext.SaveChangesAsync();
      }
    }
  }
}