namespace PetCareManagementSystem.Models
{
    public class Vaccination
    {
        public string PetId { get; set; }
        public string VaccineName { get; set; }
        public DateTime DateGiven { get; set; }
        public DateTime NextDueDate { get; set; }
    }
}