namespace ubuntu_health_api.Models.DTO
{
  public class PrescriptionResponseDto
  {
    public int Id { get; set; }
    public string? TenantId { get; set; }
    public int PatientId { get; set; }
    public string? PrescriberId { get; set; }
    public string? PrescriberName { get; set; }
    public string? PrescriberLicenseNumber { get; set; }
    public string? EndDate { get; set; }
    public string? Frequency { get; set; }
    public int Refills { get; set; } = 0;
    public string? Status { get; set; }
    public string? Instructions { get; set; }
    public IEnumerable<PrescriptionMedicationDto> Medications { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
  }
}
