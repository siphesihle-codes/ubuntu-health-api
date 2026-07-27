namespace ubuntu_health_api.Models.DTO
{
  public class InvoiceResponseDto
  {
    public int Id { get; set; }
    public string? TenantId { get; set; }
    public int PatientId { get; set; }
    public string? PatientFirstName { get; set; }
    public string? PatientLastName { get; set; }
    public int? AppointmentId { get; set; }
    public decimal TotalAmount { get; set; }
    public string? Status { get; set; }
    public string? Notes { get; set; }
    public string? DueDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
  }
}
