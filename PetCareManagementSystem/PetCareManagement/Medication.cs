namespace PetCareManagementSystem.Models
{
    public class Medication
    {
        public string PetId { get; set; }
        public string MedicationName { get; set; }
        public DateTime DateGiven { get; set; }
        public DateTime NextDueDate { get; set; }
    }
}