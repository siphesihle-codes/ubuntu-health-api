namespace ubuntu_health_api.Models.DTO
{
  public class InvoiceImportDto
  {
    public int PatientId { get; set; }
    public decimal TotalAmount { get; set; }
    public string? Status { get; set; }
    public string? Notes { get; set; }
    public string? DueDate { get; set; }
    public DateTime? CreatedAt { get; set; }
  }
}
