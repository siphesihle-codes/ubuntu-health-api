namespace ubuntu_health_api.Models.DTO
{
  public class PrescriptionImportDto
  {
    public int PatientId { get; set; }
    public string? PrescriberName { get; set; }
    public string? PrescriberLicenseNumber { get; set; }
    public string? EndDate { get; set; }
    public string? Frequency { get; set; }
    public int Refills { get; set; }
    public string? Status { get; set; }
    public string? Instructions { get; set; }
    public DateTime? CreatedAt { get; set; }
  }
}
