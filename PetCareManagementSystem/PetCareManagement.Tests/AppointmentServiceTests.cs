using System;
using System.Collections.Generic;
using Xunit;
using PetCareManagementSystem.Models;
using PetCareManagementSystem.Services;

namespace PetCareManagementSystem.Tests
{
    public class AppointmentServiceTests
    {
        private Appointment MakeAppointment()
        {
            return new Appointment
            {
                PetId           = Guid.NewGuid().ToString(),
                AppointmentType = "Vet",
                Date            = new DateTime(2025, 9, 15),
                Location        = "City Vet Clinic"
            };
        }

        [Fact]
        public void AddAppointment_SavesTheAppointment()
        {
            TestHelper.ResetTestFiles();
            var service = new AppointmentService();
            Appointment appt = MakeAppointment();

            service.AddAppointment(appt);
            List<Appointment> results = service.GetAppointments(appt.PetId);

            Assert.Single(results);
            Assert.Equal("Vet", results[0].AppointmentType);
        }

        [Fact]
        public void UpdateAppointment_ChangesTheType()
        {
            TestHelper.ResetTestFiles();
            var service = new AppointmentService();
            Appointment original = MakeAppointment();

            service.AddAppointment(original);

            Appointment updated = new Appointment
            {
                PetId           = original.PetId,
                AppointmentType = "Grooming",
                Date            = original.Date,
                Location        = original.Location
            };

            service.UpdateAppointment(original, updated);

            Assert.Equal("Grooming", service.GetAppointments(original.PetId)[0].AppointmentType);
        }

        [Fact]
        public void DeleteAppointment_RemovesTheAppointment()
        {
            TestHelper.ResetTestFiles();
            var service = new AppointmentService();
            Appointment appt = MakeAppointment();

            service.AddAppointment(appt);
            service.DeleteAppointment(appt);

            Assert.Empty(service.GetAppointments(appt.PetId));
        }
    }
}
