namespace ubuntu_health_api.Models.DTO
{
  public class ImportSummaryDto
  {
    public int PatientsCreated { get; set; }
    public int PatientsMatched { get; set; }
    public int AppointmentsCreated { get; set; }
    public int ClinicalNotesCreated { get; set; }
    public int PrescriptionsCreated { get; set; }
    public int InvoicesCreated { get; set; }
    public List<string> Skipped { get; set; } = [];
  }
}
