namespace PetCareManagementSystem.Models

{
    public class Appointment
    {
        public string PetName { get; set; }
        public string AppointmentType { get; set; }
        public DateTime Date { get; set; }
        public string Location { get; set; }
    }
}