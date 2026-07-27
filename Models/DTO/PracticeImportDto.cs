namespace ubuntu_health_api.Models.DTO
{
  public class PracticeImportDto
  {
    public IEnumerable<PatientImportDto> Patients { get; set; } = [];
    public IEnumerable<AppointmentImportDto> Appointments { get; set; } = [];
    public IEnumerable<ClinicalNoteImportDto> ClinicalNotes { get; set; } = [];
    public IEnumerable<PrescriptionImportDto> Prescriptions { get; set; } = [];
    public IEnumerable<InvoiceImportDto> Invoices { get; set; } = [];
  }
}
