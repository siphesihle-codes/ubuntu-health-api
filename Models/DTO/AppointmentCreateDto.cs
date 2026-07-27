using System.ComponentModel.DataAnnotations;

namespace ubuntu_health_api.Models.DTO
{
  public class AppointmentCreateDto
  {
    [Required(ErrorMessage = "Patient ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Patient ID must be a positive number")]
    public int PatientId { get; set; }

    [Required(ErrorMessage = "Patient first name is required")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Patient first name must be between 2 and 50 characters")]
    public required string PatientFirstName { get; set; }

    [Required(ErrorMessage = "Patient last name is required")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Patient last name must be between 2 and 50 characters")]
    public required string PatientLastName { get; set; }

    public string? PractitionerId { get; set; }

    [StringLength(100, ErrorMessage = "Practitioner name cannot exceed 100 characters")]
    public string? PractitionerName { get; set; }

    [Required(ErrorMessage = "Appointment date is required")]
    [RegularExpression(@"^\d{4}-\d{2}-\d{2}$", ErrorMessage = "Appointment date must be in YYYY-MM-DD format")]
    public required string AppointmentDate { get; set; }

    [RegularExpression(@"^([01]?[0-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = "Appointment time must be in HH:MM format (24-hour)")]
    public string? AppointmentTime { get; set; }

    [StringLength(50, ErrorMessage = "Appointment type cannot exceed 50 characters")]
    public string? AppointmentType { get; set; }

    [RegularExpression(@"^(scheduled|confirmed|checkedIn|inProgress|completed|cancelled|noShow|rescheduled)$",
      ErrorMessage = "Status must be one of: scheduled, confirmed, checkedIn, inProgress, completed, cancelled, noShow, rescheduled")]
    public string? Status { get; set; } = "scheduled";

    [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters")]
    public string? Notes { get; set; }
  }
}
