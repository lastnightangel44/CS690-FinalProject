using PetCareManagementSystem.Models;
using PetCareManagementSystem.Data;

namespace PetCareManagementSystem.Services
{
    /// <summary>
    /// Handles saving, retrieving, updating, and deleting medication and treatment records.
    /// Medications are stored as pipe-delimited lines:
    /// Id|PetId|Name|Dosage|Frequency|StartDate|EndDate|Notes
    /// EndDate is stored as an empty string if the medication is ongoing.
    /// </summary>
    public class MedicationService
    {
        private readonly FileStorageService storage = new FileStorageService();

        /// <summary>
        /// Saves a new medication record to the medications file.
        /// A new unique ID is assigned if the medication does not already have one.
        /// </summary>
        public void AddMedication(Medication medication)
        {
            if (string.IsNullOrWhiteSpace(medication.Id))
                medication.Id = Guid.NewGuid().ToString();

            storage.Save(FilePaths.MedicationsFile, Serialize(medication));
        }

        /// <summary>
        /// Retrieves all medication records for a specific pet.
        /// </summary>
        public List<Medication> GetMedications(string petId)
        {
            var lines = storage.Load(FilePaths.MedicationsFile);
            var medications = new List<Medication>();

            foreach (var line in lines)
            {
                var parts = line.Split('|');

                if (parts.Length < 8) continue;

                if (parts[1] == petId)
                    medications.Add(ParseMedication(parts));
            }

            return medications;
        }

        /// <summary>
        /// Retrieves all active medication records for a specific pet.
        /// A medication is active if it has no end date or the end date has not yet passed.
        /// </summary>
        public List<Medication> GetActiveMedications(string petId)
        {
            var medications = GetMedications(petId);
            var active = new List<Medication>();

            foreach (var medication in medications)
            {
                if (medication.IsActive)
                    active.Add(medication);
            }

            return active;
        }

        /// <summary>
        /// Retrieves all medication records across every pet.
        /// Used by the UI to display a numbered list for editing and deleting.
        /// </summary>
        public List<Medication> GetAllMedications()
        {
            var lines = storage.Load(FilePaths.MedicationsFile);
            var medications = new List<Medication>();

            foreach (var line in lines)
            {
                var parts = line.Split('|');

                if (parts.Length < 8) continue;

                medications.Add(ParseMedication(parts));
            }

            return medications;
        }

        /// <summary>
        /// Updates an existing medication record matched by its unique ID.
        /// </summary>
        public void UpdateMedication(Medication updated)
        {
            bool MatchesMedication(string line)
            {
                return line.StartsWith(updated.Id + "|");
            }

            storage.UpdateLine(FilePaths.MedicationsFile, MatchesMedication, Serialize(updated));
        }

        /// <summary>
        /// Deletes a medication record by its unique ID.
        /// </summary>
        public void DeleteMedication(string medicationId)
        {
            bool MatchesMedication(string line)
            {
                return line.StartsWith(medicationId + "|");
            }

            storage.DeleteLine(FilePaths.MedicationsFile, MatchesMedication);
        }

        /// <summary>
        /// Converts Medication object to a pipe-delimited string for storage.
        /// </summary>
        private string Serialize(Medication medication)
        {
            string adminTime = medication.AdministrationTime.HasValue
                ? medication.AdministrationTime.Value.ToString(@"hh\:mm")
                : string.Empty;

            string endDate = medication.EndDate.HasValue
                ? medication.EndDate.Value.ToString()
                : string.Empty;

            return $"{medication.Id}|{medication.PetId}|{medication.Name}|{medication.Dosage}|{medication.Frequency}|{adminTime}|{medication.StartDate}|{endDate}|{medication.Notes}";
        }

        /// <summary>
        /// Parses a pipe-delimited string array into a Medication object.
        /// </summary>
        private Medication ParseMedication(string[] parts)
        {
            return new Medication
            {
                Id                 = parts[0],
                PetId              = parts[1],
                Name               = parts[2],
                Dosage             = parts[3],
                Frequency          = parts[4],
                AdministrationTime = string.IsNullOrWhiteSpace(parts[5]) ? null : TimeSpan.Parse(parts[5]),
                StartDate          = DateTime.Parse(parts[6]),
                EndDate            = string.IsNullOrWhiteSpace(parts[7]) ? null : DateTime.Parse(parts[7]),
                Notes              = parts[8]
            };
        }
    }
}
