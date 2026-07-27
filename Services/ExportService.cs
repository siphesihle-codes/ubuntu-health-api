using ubuntu_health_api.Models.DTO;
using ubuntu_health_api.Repositories;

namespace ubuntu_health_api.Services
{
  public class ExportService(
    IPracticeRepository practiceRepository,
    IStaffService staffService,
    IPatientService patientService,
    IAppointmentService appointmentService,
    IClinicalNoteService clinicalNoteService,
    IPrescriptionService prescriptionService,
    IInvoiceService invoiceService) : IExportService
  {
    private readonly IPracticeRepository _practiceRepository = practiceRepository;
    private readonly IStaffService _staffService = staffService;
    private readonly IPatientService _patientService = patientService;
    private readonly IAppointmentService _appointmentService = appointmentService;
    private readonly IClinicalNoteService _clinicalNoteService = clinicalNoteService;
    private readonly IPrescriptionService _prescriptionService = prescriptionService;
    private readonly IInvoiceService _invoiceService = invoiceService;

    public async Task<PracticeExportDto> ExportAsync(string tenantId, CancellationToken cancellationToken = default)
    {
      var practice = await _practiceRepository.GetByTenantIdAsync(tenantId, cancellationToken);

      return new PracticeExportDto
      {
        ExportedAt = DateTime.UtcNow,
        TenantId = tenantId,
        PracticeName = practice?.Name,
        SubscriptionPlan = practice?.SubscriptionPlan,
        Staff = await EmptyWhenMissingAsync(() => _staffService.GetStaffAsync(tenantId, cancellationToken)),
        Patients = await EmptyWhenMissingAsync(() => _patientService.GetAllPatientsAsync(tenantId, cancellationToken)),
        Appointments = await EmptyWhenMissingAsync(() => _appointmentService.GetAllAppointmentsAsync(tenantId)),
        ClinicalNotes = await EmptyWhenMissingAsync(() => _clinicalNoteService.GetAllClinicalNotesAsync(tenantId)),
        Prescriptions = await EmptyWhenMissingAsync(() => _prescriptionService.GetAllPrescriptionsAsync(tenantId)),
        Invoices = await EmptyWhenMissingAsync(() => _invoiceService.GetAllInvoicesAsync(tenantId))
      };
    }

    private static async Task<IEnumerable<T>> EmptyWhenMissingAsync<T>(Func<Task<IEnumerable<T>>> fetch)
    {
      try
      {
        return await fetch();
      }
      catch (KeyNotFoundException)
      {
        return [];
      }
    }
  }
}
