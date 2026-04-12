using System;
using System.Collections.Generic;
using Xunit;
using PetCareManagementSystem.Models;
using PetCareManagementSystem.Services;

namespace PetCareManagementSystem.Tests
{
    public class MedicationServiceTests
    {
        private Medication Test_MakeMedication()
        {
            return new Medication
            {
                Id        = Guid.NewGuid().ToString(),
                PetId     = Guid.NewGuid().ToString(),
                Name      = "Antibiotics",
                Dosage    = "5mg",
                Frequency = "Once daily",
                StartDate = DateTime.Today,
                EndDate   = DateTime.Today.AddDays(10),
                Notes     = "Give with food"
            };
        }

        [Fact]
        public void Test_AddMedication()
        {
            TestHelper.ResetTestFiles();
            var service = new MedicationService();
            Medication med = Test_MakeMedication();

            service.AddMedication(med);
            List<Medication> results = service.GetMedications(med.PetId);

            Assert.Single(results);
            Assert.Equal("Antibiotics", results[0].Name);
        }

        [Fact]
        public void Test_GetActiveMedications()
        {
            TestHelper.ResetTestFiles();
            var service = new MedicationService();
            string petId = Guid.NewGuid().ToString();

            // Active — end date in the future
            service.AddMedication(new Medication
            {
                Id        = Guid.NewGuid().ToString(),
                PetId     = petId,
                Name      = "Antibiotics",
                Dosage    = "5mg",
                Frequency = "Once daily",
                StartDate = DateTime.Today,
                EndDate   = DateTime.Today.AddDays(10),
                Notes     = ""
            });

            // Inactive — end date in the past
            service.AddMedication(new Medication
            {
                Id        = Guid.NewGuid().ToString(),
                PetId     = petId,
                Name      = "Old Treatment",
                Dosage    = "10mg",
                Frequency = "Twice daily",
                StartDate = DateTime.Today.AddDays(-30),
                EndDate   = DateTime.Today.AddDays(-10),
                Notes     = ""
            });

            List<Medication> active = service.GetActiveMedications(petId);

            Assert.Single(active);
            Assert.Equal("Antibiotics", active[0].Name);
        }

        [Fact]
        public void Test_UpdateMedication()
        {
            TestHelper.ResetTestFiles();
            var service = new MedicationService();
            Medication med = Test_MakeMedication();

            service.AddMedication(med);
            med.Dosage = "10mg";
            service.UpdateMedication(med);

            List<Medication> results = service.GetMedications(med.PetId);
            Assert.Equal("10mg", results[0].Dosage);
        }

        [Fact]
        public void Test_DeleteMedication()
        {
            TestHelper.ResetTestFiles();
            var service = new MedicationService();
            Medication med = Test_MakeMedication();

            service.AddMedication(med);
            service.DeleteMedication(med.Id);

            Assert.Empty(service.GetMedications(med.PetId));
        }
    }
}
