using System.ComponentModel.DataAnnotations;

namespace ubuntu_health_api.Models.DTO
{
  public class ClinicalNoteUpdateDto
  {
    [Required(ErrorMessage = "Diagnosis code is required")]
    [StringLength(20, ErrorMessage = "Diagnosis code cannot exceed 20 characters")]
    public required string DiagnosesCode { get; set; }

    [StringLength(10000, ErrorMessage = "Notes cannot exceed 10 000 characters")]
    public string? Notes { get; set; }
  }
}
