using System.ComponentModel.DataAnnotations;

namespace ubuntu_health_api.Models.DTO
{
  public class PrescriptionUpdateDto
  {
    [RegularExpression(@"^\d{4}-\d{2}-\d{2}$", ErrorMessage = "End date must be in YYYY-MM-DD format")]
    public string? EndDate { get; set; }

    [StringLength(100, ErrorMessage = "Frequency cannot exceed 100 characters")]
    public string? Frequency { get; set; }

    [Range(0, 10, ErrorMessage = "Refills must be between 0 and 10")]
    public int Refills { get; set; } = 0;

    [RegularExpression(@"^(active|pending|completed|cancelled|expired)$",
      ErrorMessage = "Status must be one of: active, pending, completed, cancelled, expired")]
    public string? Status { get; set; }

    [StringLength(1000, ErrorMessage = "Instructions cannot exceed 1000 characters")]
    public string? Instructions { get; set; }
  }
}
