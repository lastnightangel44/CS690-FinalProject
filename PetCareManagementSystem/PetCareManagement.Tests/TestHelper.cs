using System.IO;
using PetCareManagementSystem.Data;

namespace PetCareManagementSystem.Tests
{
    /// <summary>
    /// Shared helper used by all test classes.
    /// Points FilePaths at dedicated test files and deletes them before each
    /// test so every test starts with a clean slate.
    /// </summary>
    public static class TestHelper
    {
        public static void ResetTestFiles()
        {
            string dir = Directory.GetCurrentDirectory();

            FilePaths.UsersFile        = Path.Combine(dir, "test_users.txt");
            FilePaths.PetsFile         = Path.Combine(dir, "test_pets.txt");
            FilePaths.AppointmentsFile = Path.Combine(dir, "test_appointments.txt");
            FilePaths.SuppliesFile     = Path.Combine(dir, "test_supplies.txt");
            FilePaths.VaccinationsFile = Path.Combine(dir, "test_vaccinations.txt");

            foreach (var file in new[]
            {
                FilePaths.UsersFile,
                FilePaths.PetsFile,
                FilePaths.AppointmentsFile,
                FilePaths.SuppliesFile,
                FilePaths.VaccinationsFile
            })
            {
                if (File.Exists(file))
                    File.Delete(file);
            }
        }
    }
}
