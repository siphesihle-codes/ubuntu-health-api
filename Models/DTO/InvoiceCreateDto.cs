using System.ComponentModel.DataAnnotations;

namespace ubuntu_health_api.Models.DTO
{
  public class InvoiceCreateDto
  {
    [Range(1, int.MaxValue, ErrorMessage = "Patient ID must be a positive number")]
    public int PatientId { get; set; }

    public int? AppointmentId { get; set; }

    [Required(ErrorMessage = "Patient first name is required")]
    public required string PatientFirstName { get; set; }

    [Required(ErrorMessage = "Patient last name is required")]
    public required string PatientLastName { get; set; }

    [Range(0, 9999999, ErrorMessage = "Amount must be between 0 and 9 999 999")]
    public decimal TotalAmount { get; set; }

    [RegularExpression(@"^(draft|pending|paid|overdue|cancelled|partiallyPaid)$",
      ErrorMessage = "Status must be one of: draft, pending, paid, overdue, cancelled, partiallyPaid")]
    public string? Status { get; set; } = "draft";

    [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters")]
    public string? Notes { get; set; }

    [Required(ErrorMessage = "Due date is required")]
    [RegularExpression(@"^\d{4}-\d{2}-\d{2}$", ErrorMessage = "Due date must be in YYYY-MM-DD format")]
    public required string DueDate { get; set; }
  }
}
