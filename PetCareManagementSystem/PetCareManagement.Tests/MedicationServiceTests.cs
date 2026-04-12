using System;
using System.Collections.Generic;
using Xunit;
using PetCareManagementSystem.Models;
using PetCareManagementSystem.Services;

namespace PetCareManagementSystem.Tests
{
    public class MedicationServiceTests
    {
        /// <summary>
        /// Creates a sample medication with all fields populated including
        /// AdministrationTime to match the v4.0 storage format.
        /// </summary>
        private Medication MakeMedication()
        {
            return new Medication
            {
                Id                 = Guid.NewGuid().ToString(),
                PetId              = Guid.NewGuid().ToString(),
                Name               = "Antibiotics",
                Dosage             = "5mg",
                Frequency          = "Once daily",
                AdministrationTime = new TimeSpan(8, 0, 0),   // 08:00
                StartDate          = DateTime.Today,
                EndDate            = DateTime.Today.AddDays(10),
                Notes              = "Give with food"
            };
        }

        [Fact]
        public void AddMedication_SavesTheMedication()
        {
            TestHelper.ResetTestFiles();
            var service = new MedicationService();
            Medication med = MakeMedication();

            service.AddMedication(med);
            List<Medication> results = service.GetMedications(med.PetId);

            Assert.Single(results);
            Assert.Equal("Antibiotics", results[0].Name);
        }

        [Fact]
        public void GetActiveMedications_ReturnsOnlyActiveMedications()
        {
            TestHelper.ResetTestFiles();
            var service = new MedicationService();
            string petId = Guid.NewGuid().ToString();

            // Active — end date in the future
            service.AddMedication(new Medication
            {
                Id                 = Guid.NewGuid().ToString(),
                PetId              = petId,
                Name               = "Antibiotics",
                Dosage             = "5mg",
                Frequency          = "Once daily",
                AdministrationTime = new TimeSpan(8, 0, 0),
                StartDate          = DateTime.Today,
                EndDate            = DateTime.Today.AddDays(10),
                Notes              = ""
            });

            // Inactive — end date in the past
            service.AddMedication(new Medication
            {
                Id                 = Guid.NewGuid().ToString(),
                PetId              = petId,
                Name               = "Old Treatment",
                Dosage             = "10mg",
                Frequency          = "Twice daily",
                AdministrationTime = null,
                StartDate          = DateTime.Today.AddDays(-30),
                EndDate            = DateTime.Today.AddDays(-10),
                Notes              = ""
            });

            List<Medication> active = service.GetActiveMedications(petId);

            Assert.Single(active);
            Assert.Equal("Antibiotics", active[0].Name);
        }

        [Fact]
        public void UpdateMedication_ChangesTheDosage()
        {
            TestHelper.ResetTestFiles();
            var service = new MedicationService();
            Medication med = MakeMedication();

            service.AddMedication(med);
            med.Dosage = "10mg";
            service.UpdateMedication(med);

            List<Medication> results = service.GetMedications(med.PetId);
            Assert.Equal("10mg", results[0].Dosage);
        }

        [Fact]
        public void DeleteMedication_RemovesTheMedication()
        {
            TestHelper.ResetTestFiles();
            var service = new MedicationService();
            Medication med = MakeMedication();

            service.AddMedication(med);
            service.DeleteMedication(med.Id);

            Assert.Empty(service.GetMedications(med.PetId));
        }
    }
}
