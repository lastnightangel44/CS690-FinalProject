using PetCareManagementSystem.Models;
using PetCareManagementSystem.Data;

namespace PetCareManagementSystem.Services
{
    public class AppointmentService
    {
        private FileStorageService storage = new FileStorageService();

        public void AddAppointment(Appointment appointment)
        {
            storage.Save(
                FilePaths.AppointmentsFile,
                $"{appointment.PetName}|{appointment.AppointmentName}|{appointment.AppointmentType}|{appointment.Date}|{appointment.Location}"
            );
        }

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
                        PetName = parts[0],
                        AppointmentName = parts[1],
                        AppointmentType = parts[2],
                        Date = DateTime.Parse(parts[3]),
                        Location = parts[4]
                    });
                }
            }

            return appointments;
        }
    }
}