using PetCareManagementSystem.Models;
using PetCareManagementSystem.Data;

namespace PetCareManagementSystem.Services
{
    public class VaccinationService
    {
        private FileStorageService storage = new FileStorageService();

        public void AddVaccination(Vaccination vaccination)
        {
            storage.Save(
                FilePaths.VaccinationsFile,
                $"{vaccination.PetId}|{vaccination.VaccineName}|{vaccination.DateGiven}|{vaccination.NextDueDate}"
            );
        }

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
                        PetId = parts[0],
                        VaccineName = parts[1],
                        DateGiven = DateTime.Parse(parts[2]),
                        NextDueDate = DateTime.Parse(parts[3])
                    });
                }
            }

            return vaccinations;
        }

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
                        PetId = parts[0],
                        VaccineName = parts[1],
                        DateGiven = DateTime.Parse(parts[2]),
                        NextDueDate = nextDate
                    });
                }
            }

            return due;
        }
    }
}