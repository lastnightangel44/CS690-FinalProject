using PetCareManagementSystem.Models;
using PetCareManagementSystem.Data;

namespace PetCareManagementSystem.Services
{
    /// <summary>
    /// Appointment records
    /// Appointments are stored as pipe-delimited lines: PetId|AppointmentType|Date|Location
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

        /// <summary>
        /// Retrieves every appointment across all pets.
        /// </summary>
        public List<Appointment> GetAllAppointments()
        {
            var lines = storage.Load(FilePaths.AppointmentsFile);
            var appointments = new List<Appointment>();

            foreach (var line in lines)
            {
                var parts = line.Split('|');

                if (parts.Length < 4) continue;

                appointments.Add(new Appointment
                {
                    PetId           = parts[0],
                    AppointmentType = parts[1],
                    Date            = DateTime.Parse(parts[2]),
                    Location        = parts[3]
                });
            }

            return appointments;
        }

        /// <summary>
        /// Updates an existing appointment
        /// </summary>
        public void UpdateAppointment(Appointment original, Appointment updated)
        {
            string originalKey = $"{original.PetId}|{original.AppointmentType}|{original.Date}|{original.Location}";
            string newLine = $"{updated.PetId}|{updated.AppointmentType}|{updated.Date}|{updated.Location}";

            bool MatchesAppointment(string line)
            {
                return line == originalKey;
            }

            storage.UpdateLine(FilePaths.AppointmentsFile, MatchesAppointment, newLine);
        }

        /// <summary>
        /// Deletes a specific appointment matched by PetId, type, date, and location.
        /// </summary>
        public void DeleteAppointment(Appointment appointment)
        {
            string key = $"{appointment.PetId}|{appointment.AppointmentType}|{appointment.Date}|{appointment.Location}";

            bool MatchesAppointment(string line)
            {
                return line == key;
            }

            storage.DeleteLine(FilePaths.AppointmentsFile, MatchesAppointment);
        }
    }
}