using System;
using System.Collections.Generic;
using Xunit;
using PetCareManagementSystem.Models;
using PetCareManagementSystem.Services;

namespace PetCareManagementSystem.Tests
{
    public class VaccinationServiceTests
    {
        private Vaccination MakeVaccination()
        {
            return new Vaccination
            {
                PetId       = Guid.NewGuid().ToString(),
                VaccineName = "Rabies",
                DateGiven   = DateTime.Now.AddYears(-1),
                NextDueDate = DateTime.Now.AddYears(2)
            };
        }

        [Fact]
        public void AddVaccination_SavesTheVaccination()
        {
            TestHelper.ResetTestFiles();
            var service = new VaccinationService();
            Vaccination vax = MakeVaccination();

            service.AddVaccination(vax);
            List<Vaccination> results = service.GetVaccinations(vax.PetId);

            Assert.Single(results);
            Assert.Equal("Rabies", results[0].VaccineName);
        }

        [Fact]
        public void GetDueVaccinations_ReturnsOverdueVaccination()
        {
            TestHelper.ResetTestFiles();
            var service = new VaccinationService();

            service.AddVaccination(new Vaccination
            {
                PetId       = Guid.NewGuid().ToString(),
                VaccineName = "Bordetella",
                DateGiven   = DateTime.Now.AddYears(-1),
                NextDueDate = DateTime.Now.AddDays(-5)   // overdue
            });

            List<Vaccination> due = service.GetDueVaccinations();

            Assert.Single(due);
            Assert.Equal("Bordetella", due[0].VaccineName);
        }

        [Fact]
        public void DeleteVaccination_RemovesTheVaccination()
        {
            TestHelper.ResetTestFiles();
            var service = new VaccinationService();
            Vaccination vax = MakeVaccination();

            service.AddVaccination(vax);
            service.DeleteVaccination(vax);

            Assert.Empty(service.GetVaccinations(vax.PetId));
        }
    }
}
