namespace PetCareManagementSystem.Models
{
    /// <summary>
    /// Represents a vaccination record for a pet
    /// </summary>
    public class Vaccination
    {
        public string PetId { get; set; }
        public string PetName { get; set; }
        public string VaccineName { get; set; }
        public DateTime DateGiven { get; set; }
        public DateTime NextDueDate { get; set; }
    }
}