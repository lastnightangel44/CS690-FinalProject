namespace PetCareManagementSystem.Models

{
    public class Appointment
    {
        public string PetId { get; set; }
        public string AppointmentType { get; set; }
        public DateTime Date { get; set; }
        public string Location { get; set; }
    }
}