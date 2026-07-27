using System.ComponentModel.DataAnnotations;

namespace ubuntu_health_api.Models.DTO
{
  public class ClinicalNoteCreateDto
  {
    [Range(1, int.MaxValue, ErrorMessage = "Patient ID must be a positive number")]
    public int PatientId { get; set; }

    [Required(ErrorMessage = "Diagnosis code is required")]
    [StringLength(20, ErrorMessage = "Diagnosis code cannot exceed 20 characters")]
    public required string DiagnosesCode { get; set; }

    [StringLength(10000, ErrorMessage = "Notes cannot exceed 10 000 characters")]
    public string? Notes { get; set; }
  }
}
