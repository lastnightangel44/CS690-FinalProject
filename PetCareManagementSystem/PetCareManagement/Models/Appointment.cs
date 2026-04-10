namespace PetCareManagementSystem.Models
{
    /// <summary>
    /// Represents a scheduled appointment for a pet, such as a vet visit or grooming session.
    /// </summary>
    public class Appointment
    {
        public string PetId { get; set; }
        public string AppointmentType { get; set; }
        public DateTime Date { get; set; }
        public string Location { get; set; }
    }
}