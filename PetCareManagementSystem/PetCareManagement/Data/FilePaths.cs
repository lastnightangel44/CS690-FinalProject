namespace PetCareManagementSystem.Data
{
    /// <summary>
    /// Centralizes all file paths used for persistent storage
    /// </summary>
    public static class FilePaths
    {
       private static string DataFolder = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "Data", "Store"
        );

        public static string UsersFile        = Path.Combine(DataFolder, "users.txt");
        public static string PetsFile         = Path.Combine(DataFolder, "pets.txt");
        public static string AppointmentsFile = Path.Combine(DataFolder, "appointments.txt");
        public static string SuppliesFile     = Path.Combine(DataFolder, "supplies.txt");
        public static string VaccinationsFile = Path.Combine(DataFolder, "vaccinations.txt");
    }
}