using PetCareManagementSystem.Models;
using PetCareManagementSystem.Data;

namespace PetCareManagementSystem.Services
{
    /// <summary>
    /// Handles saving and retrieving appointment records from persistent file storage.
    /// </summary>
    public class AppointmentService
    {
        private FileStorageService storage = new FileStorageService();

        /// <summary>
        /// Saves a new appointment to the appointments file.
        /// </summary>
        public void AddAppointment(Appointment appointment)
        {
            storage.Save(
                FilePaths.AppointmentsFile,
                $"{appointment.PetId}|{appointment.AppointmentType}|{appointment.Date}|{appointment.Location}"
            );
        }

        /// <summary>
        /// Retrieves all appointments for a specific pet.
        /// </summary>
        public List<Appointment> GetAppointments(string petId)
        {
            var lines = storage.Load(FilePaths.AppointmentsFile);
            var appointments = new List<Appointment>();

            foreach (var line in lines)
            {
                var parts = line.Split('|');

                if (parts[0] == petId)
                {
                    appointments.Add(new Appointment
                    {
                        PetId           = parts[0],
                        AppointmentType = parts[1],
                        Date            = DateTime.Parse(parts[2]),
                        Location        = parts[3]
                    });
                }
            }

            return appointments;
        }
    }
}