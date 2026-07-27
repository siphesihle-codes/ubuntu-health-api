namespace ubuntu_health_api.Models.DTO
{
  public class AppointmentImportDto
  {
    public int PatientId { get; set; }
    public string? PatientFirstName { get; set; }
    public string? PatientLastName { get; set; }
    public string? PractitionerId { get; set; }
    public string? PractitionerName { get; set; }
    public string? AppointmentDate { get; set; }
    public string? AppointmentTime { get; set; }
    public string? AppointmentType { get; set; }
    public string? Status { get; set; }
    public string? Notes { get; set; }
  }
}
