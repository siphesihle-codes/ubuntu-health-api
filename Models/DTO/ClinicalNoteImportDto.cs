namespace ubuntu_health_api.Models.DTO
{
  public class ClinicalNoteImportDto
  {
    public int PatientId { get; set; }
    public string? DoctorId { get; set; }
    public string? DiagnosesCode { get; set; }
    public string? Notes { get; set; }
    public DateTime? CreatedAt { get; set; }
  }
}
