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