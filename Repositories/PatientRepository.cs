using ubuntu_health_api.Data;
using ubuntu_health_api.Models;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Serialization;

namespace ubuntu_health_api.Repositories
{
  public class PatientRepository(AppDbContext dbContext) : IPatientRepository
  {
    private readonly AppDbContext _dbContext = dbContext;

    public async Task<IEnumerable<Patient>> GetAllPatientsAsync(string tenantId, CancellationToken cancellationToken)
    {
      return await _dbContext.Patients
        .Where(p => p.TenantId == tenantId)
        .ToListAsync(cancellationToken);
    }

    public async Task<Patient> GetPatientByIdAsync(int id, string tenantId)
    {
      var patient = await _dbContext.Patients
        .FirstOrDefaultAsync(p => p.Id == id && p.TenantId == tenantId)
        ?? throw new KeyNotFoundException($"Patient with ID {id} and Tenant ID {tenantId} was not found.");

      return patient;
    }

    public async Task AddPatientAsync(Patient patient)
    {
      await _dbContext.Patients.AddAsync(patient);
      await _dbContext.SaveChangesAsync();
    }

    public async Task UpdatePatientAsync(Patient patient, string tenantId)
    {
      var existing = await _dbContext.Patients
          .FirstOrDefaultAsync(e => e.Id == patient.Id && e.TenantId == tenantId)
          ?? throw new InvalidOperationException($"Patient not found or tenant mismatch (Patient Tenant: {tenantId}, Request Tenant: {tenantId})");

      existing.FirstName = patient.FirstName;
      existing.LastName = patient.LastName;
      existing.IdNumber = patient.IdNumber;
      existing.Sex = patient.Sex;
      existing.Email = patient.Email;
      existing.Phone = patient.Phone;
      existing.Street = patient.Street;
      existing.StreetTwo = patient.StreetTwo;
      existing.City = patient.City;
      existing.Province = patient.Province;
      existing.PostalCode = patient.PostalCode;
      existing.Allergies = patient.Allergies;
      existing.CurrentMedication = patient.CurrentMedication;
      existing.EmergencyContactFirstName = patient.EmergencyContactFirstName;
      existing.EmergencyContactLastName = patient.EmergencyContactLastName;
      existing.EmergencyContactPhone = patient.EmergencyContactPhone;
      existing.EmergencyContactRelationship = patient.EmergencyContactRelationship;
      existing.MedicalAidName = patient.MedicalAidName;
      existing.MembershipNumber = patient.MembershipNumber;

      await _dbContext.SaveChangesAsync();
    }

    public async Task DeletePatientAsync(int id, string tenantId)
    {
      var patient = await _dbContext.Patients
      .FirstOrDefaultAsync(p => p.Id == id && p.TenantId == tenantId);

      if (patient != null)
      {
        _dbContext.Patients.Remove(patient);
        await _dbContext.SaveChangesAsync();
      }
    }

  }
}