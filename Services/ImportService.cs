using ubuntu_health_api.Exceptions;
using ubuntu_health_api.Models;
using ubuntu_health_api.Models.DTO;
using ubuntu_health_api.Repositories;

namespace ubuntu_health_api.Services
{
  public class ImportService(IImportRepository importRepository) : IImportService
  {
    private const int MaxRecordsPerImport = 20000;

    private readonly IImportRepository _importRepository = importRepository;

    public async Task<ImportSummaryDto> ImportAsync(PracticeImportDto import, string tenantId, CancellationToken cancellationToken = default)
    {
      var patients = import.Patients.ToList();
      var appointments = import.Appointments.ToList();
      var clinicalNotes = import.ClinicalNotes.ToList();
      var prescriptions = import.Prescriptions.ToList();
      var invoices = import.Invoices.ToList();

      var total = patients.Count + appointments.Count + clinicalNotes.Count + prescriptions.Count + invoices.Count;
      if (total == 0)
      {
        throw new ValidationException("That file contains no records to import");
      }

      if (total > MaxRecordsPerImport)
      {
        throw new ValidationException($"An import can hold at most {MaxRecordsPerImport:N0} records. Split the file and import it in parts.");
      }

      var summary = new ImportSummaryDto();
      var now = DateTime.UtcNow;

      var existingByIdNumber = await _importRepository.GetPatientIdsByIdNumberAsync(tenantId, cancellationToken);
      var patientIdBySourceId = new Dictionary<int, int>();
      var newPatients = new List<Patient>();
      var newPatientSourceIds = new List<int>();
      var sourceIdByNewIdNumber = new Dictionary<string, int>();
      var duplicateSourceIds = new Dictionary<int, int>();

      foreach (var source in patients)
      {
        var idNumber = source.IdNumber?.Trim();

        if (!string.IsNullOrEmpty(idNumber) && existingByIdNumber.TryGetValue(idNumber, out var existingId))
        {
          patientIdBySourceId[source.Id] = existingId;
          summary.PatientsMatched++;
          continue;
        }

        if (!string.IsNullOrEmpty(idNumber) && sourceIdByNewIdNumber.TryGetValue(idNumber, out var firstSourceId))
        {
          duplicateSourceIds[source.Id] = firstSourceId;
          summary.Skipped.Add($"Patient {idNumber} appears more than once in the file");
          continue;
        }

        if (string.IsNullOrWhiteSpace(source.FirstName) && string.IsNullOrWhiteSpace(source.LastName))
        {
          summary.Skipped.Add($"Patient {source.Id} has no name");
          continue;
        }

        newPatients.Add(new Patient
        {
          TenantId = tenantId,
          FirstName = source.FirstName,
          LastName = source.LastName,
          IdNumber = idNumber,
          Sex = source.Sex ?? string.Empty,
          Email = source.Email,
          Phone = source.Phone,
          Street = source.Street,
          StreetTwo = source.StreetTwo,
          City = source.City,
          Province = source.Province,
          PostalCode = source.PostalCode,
          Allergies = source.Allergies,
          CurrentMedication = source.CurrentMedication,
          MedicalAidName = source.MedicalAidName,
          MembershipNumber = source.MembershipNumber,
          EmergencyContactFirstName = source.EmergencyContactFirstName,
          EmergencyContactLastName = source.EmergencyContactLastName,
          EmergencyContactPhone = source.EmergencyContactPhone,
          EmergencyContactRelationship = source.EmergencyContactRelationship,
          CreatedAt = now,
          UpdatedAt = now
        });

        newPatientSourceIds.Add(source.Id);

        if (!string.IsNullOrEmpty(idNumber))
        {
          sourceIdByNewIdNumber[idNumber] = source.Id;
        }
      }

      if (newPatients.Count > 0)
      {
        await _importRepository.AddPatientsAsync(newPatients, cancellationToken);

        for (var index = 0; index < newPatients.Count; index++)
        {
          patientIdBySourceId[newPatientSourceIds[index]] = newPatients[index].Id;
        }

        summary.PatientsCreated = newPatients.Count;
      }

      foreach (var (duplicateSourceId, firstSourceId) in duplicateSourceIds)
      {
        if (patientIdBySourceId.TryGetValue(firstSourceId, out var patientId))
        {
          patientIdBySourceId[duplicateSourceId] = patientId;
        }
      }

      var mappedAppointments = new List<Appointment>();
      foreach (var source in appointments)
      {
        if (!patientIdBySourceId.TryGetValue(source.PatientId, out var patientId))
        {
          summary.Skipped.Add($"Appointment on {source.AppointmentDate} has no matching patient");
          continue;
        }

        if (string.IsNullOrWhiteSpace(source.AppointmentDate))
        {
          summary.Skipped.Add("An appointment has no date");
          continue;
        }

        mappedAppointments.Add(new Appointment
        {
          TenantId = tenantId,
          PatientId = patientId,
          PatientFirstName = source.PatientFirstName ?? string.Empty,
          PatientLastName = source.PatientLastName ?? string.Empty,
          PractitionerId = source.PractitionerId,
          PractitionerName = source.PractitionerName,
          AppointmentDate = source.AppointmentDate,
          AppointmentTime = source.AppointmentTime,
          AppointmentType = source.AppointmentType,
          Status = source.Status,
          Notes = source.Notes,
          CreatedAt = now,
          UpdatedAt = now
        });
      }

      if (mappedAppointments.Count > 0)
      {
        await _importRepository.AddAppointmentsAsync(mappedAppointments, cancellationToken);
        summary.AppointmentsCreated = mappedAppointments.Count;
      }

      var mappedNotes = new List<ClinicalNote>();
      foreach (var source in clinicalNotes)
      {
        if (!patientIdBySourceId.TryGetValue(source.PatientId, out var patientId))
        {
          summary.Skipped.Add("A clinical note has no matching patient");
          continue;
        }

        mappedNotes.Add(new ClinicalNote
        {
          TenantId = tenantId,
          PatientId = patientId,
          DoctorId = source.DoctorId ?? string.Empty,
          DiagnosesCode = source.DiagnosesCode ?? string.Empty,
          Notes = source.Notes,
          CreatedAt = source.CreatedAt ?? now,
          UpdatedAt = now
        });
      }

      if (mappedNotes.Count > 0)
      {
        await _importRepository.AddClinicalNotesAsync(mappedNotes, cancellationToken);
        summary.ClinicalNotesCreated = mappedNotes.Count;
      }

      var mappedPrescriptions = new List<Prescription>();
      foreach (var source in prescriptions)
      {
        if (!patientIdBySourceId.TryGetValue(source.PatientId, out var patientId))
        {
          summary.Skipped.Add("A prescription has no matching patient");
          continue;
        }

        mappedPrescriptions.Add(new Prescription
        {
          TenantId = tenantId,
          PatientId = patientId,
          PrescriberName = source.PrescriberName,
          PrescriberLicenseNumber = source.PrescriberLicenseNumber,
          EndDate = source.EndDate,
          Frequency = source.Frequency,
          Refills = source.Refills,
          Status = source.Status,
          Instructions = source.Instructions,
          CreatedAt = source.CreatedAt ?? now,
          UpdatedAt = now
        });
      }

      if (mappedPrescriptions.Count > 0)
      {
        await _importRepository.AddPrescriptionsAsync(mappedPrescriptions, cancellationToken);
        summary.PrescriptionsCreated = mappedPrescriptions.Count;
      }

      var mappedInvoices = new List<Invoice>();
      foreach (var source in invoices)
      {
        if (!patientIdBySourceId.TryGetValue(source.PatientId, out var patientId))
        {
          summary.Skipped.Add("An invoice has no matching patient");
          continue;
        }

        mappedInvoices.Add(new Invoice
        {
          TenantId = tenantId,
          PatientId = patientId,
          TotalAmount = source.TotalAmount,
          Status = source.Status,
          Notes = source.Notes,
          DueDate = source.DueDate ?? string.Empty,
          CreatedAt = source.CreatedAt ?? now,
          UpdatedAt = now
        });
      }

      if (mappedInvoices.Count > 0)
      {
        await _importRepository.AddInvoicesAsync(mappedInvoices, cancellationToken);
        summary.InvoicesCreated = mappedInvoices.Count;
      }

      return summary;
    }
  }
}
