using PetCareManagementSystem.Models;
using PetCareManagementSystem.Data;

namespace PetCareManagementSystem.Services
{
    /// <summary>
    /// Handles saving, retrieving, and evaluating vaccination records.
    /// Vaccinations are stored as pipe-delimited lines: PetId|VaccineName|DateGiven|NextDueDate
    /// </summary>
    public class VaccinationService
    {
        private FileStorageService storage = new FileStorageService();

        /// <summary>
        /// Saves a new vaccination record to the vaccinations file.
        /// </summary>
        public void AddVaccination(Vaccination vaccination)
        {
            storage.Save(
                FilePaths.VaccinationsFile,
                $"{vaccination.PetId}|{vaccination.VaccineName}|{vaccination.DateGiven}|{vaccination.NextDueDate}"
            );
        }


        /// <summary>
        /// Retrieves all vaccination records for a specific pet.
        /// </summary>
        public List<Vaccination> GetVaccinations(string petId)
        {
            var lines = storage.Load(FilePaths.VaccinationsFile);
            var vaccinations = new List<Vaccination>();

            foreach (var line in lines)
            {
                var parts = line.Split('|');

                if (parts[0] == petId)
                {
                    vaccinations.Add(new Vaccination
                    {
                        PetId       = parts[0],
                        VaccineName = parts[1],
                        DateGiven   = DateTime.Parse(parts[2]),
                        NextDueDate = DateTime.Parse(parts[3])
                    });
                }
            }

            return vaccinations;
        }

        public List<Vaccination> GetAllVaccinations()
        {
            var lines = storage.Load(FilePaths.VaccinationsFile);
            var vaccinations = new List<Vaccination>();

            foreach (var line in lines)
            {
                var parts = line.Split('|');

                if (parts.Length < 4) continue;

                vaccinations.Add(new Vaccination
                {
                    PetId       = parts[0],
                    VaccineName = parts[1],
                    DateGiven   = DateTime.Parse(parts[2]),
                    NextDueDate = DateTime.Parse(parts[3])
                });
            }

            return vaccinations;
        }

        /// <summary>
        /// Updates an existing vaccination record.
        /// </summary>
        public void UpdateVaccination(Vaccination original, Vaccination updated)
        {
            string originalKey = $"{original.PetId}|{original.VaccineName}|{original.DateGiven}|{original.NextDueDate}";
            string newLine = $"{updated.PetId}|{updated.VaccineName}|{updated.DateGiven}|{updated.NextDueDate}";

            bool MatchesVaccination(string line)
            {
                return line == originalKey;
            }

            storage.UpdateLine(FilePaths.VaccinationsFile, MatchesVaccination, newLine);
        }

        /// <summary>
        /// Deletes a specific vaccination record matched by all four fields.
        /// </summary>
        public void DeleteVaccination(Vaccination vaccination)
        {
            string key = $"{vaccination.PetId}|{vaccination.VaccineName}|{vaccination.DateGiven}|{vaccination.NextDueDate}";

            bool MatchesVaccination(string line)
            {
                return line == key;
            }

            storage.DeleteLine(FilePaths.VaccinationsFile, MatchesVaccination);
        }

        /// <summary>
        /// Returns all vaccinations whose next due date is today or in the past.
        /// </summary>
        public List<Vaccination> GetDueVaccinations()
        {
            var lines = storage.Load(FilePaths.VaccinationsFile);
            var due = new List<Vaccination>();

            foreach (var line in lines)
            {
                var parts = line.Split('|');

                if (parts.Length < 4) continue;

                DateTime nextDate = DateTime.Parse(parts[3]);

                if (nextDate <= DateTime.Now)
                {
                    due.Add(new Vaccination
                    {
                        PetId       = parts[0],
                        VaccineName = parts[1],
                        DateGiven   = DateTime.Parse(parts[2]),
                        NextDueDate = nextDate
                    });
                }
            }

            return due;
        }
    }
}