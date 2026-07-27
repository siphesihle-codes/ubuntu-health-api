namespace ubuntu_health_api.Models
{
  public class PrescriptionMedication
  {
    public int Id { get; set; }
    public int PrescriptionId { get; set; }
    public required string Name { get; set; }
    public required string Dosage { get; set; }
    public string? Instructions { get; set; }
  }
}
