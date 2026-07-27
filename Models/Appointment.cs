namespace ubuntu_health_api.Models
{
  public class Appointment
  {
    public int Id { get; set; }
    public required string TenantId { get; set; }
    public int PatientId { get; set; }
    public required string PatientFirstName { get; set; }
    public required string PatientLastName { get; set; }
    public string? PractitionerId { get; set; }
    public string? PractitionerName { get; set; }
    public required string AppointmentDate { get; set; }
    public string? AppointmentTime { get; set; }
    public string? AppointmentType { get; set; }
    public string? Status { get; set; }
    public string? Notes { get; set; }
    public Patient? Patient { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
  }
}