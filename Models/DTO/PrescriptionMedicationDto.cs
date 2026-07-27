using System.ComponentModel.DataAnnotations;

namespace ubuntu_health_api.Models.DTO
{
  public class PrescriptionMedicationDto
  {
    [Required(ErrorMessage = "Medication name is required")]
    [StringLength(200, ErrorMessage = "Medication name cannot exceed 200 characters")]
    public required string Name { get; set; }

    [Required(ErrorMessage = "Dosage is required")]
    [StringLength(100, ErrorMessage = "Dosage cannot exceed 100 characters")]
    public required string Dosage { get; set; }

    [StringLength(500, ErrorMessage = "Instructions cannot exceed 500 characters")]
    public string? Instructions { get; set; }
  }
}
