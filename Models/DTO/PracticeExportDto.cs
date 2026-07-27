namespace ubuntu_health_api.Models.DTO
{
  public class PracticeExportDto
  {
    public DateTime ExportedAt { get; set; }
    public required string TenantId { get; set; }
    public string? PracticeName { get; set; }
    public string? SubscriptionPlan { get; set; }
    public IEnumerable<StaffMemberDto> Staff { get; set; } = [];
    public IEnumerable<PatientResponseDto> Patients { get; set; } = [];
    public IEnumerable<AppointmentResponseDto> Appointments { get; set; } = [];
    public IEnumerable<ClinicalNoteResponseDto> ClinicalNotes { get; set; } = [];
    public IEnumerable<PrescriptionResponseDto> Prescriptions { get; set; } = [];
    public IEnumerable<InvoiceResponseDto> Invoices { get; set; } = [];
  }
}
