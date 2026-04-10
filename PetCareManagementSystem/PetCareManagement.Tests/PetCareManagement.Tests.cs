using System;
using System.IO;
using System.Collections.Generic;
using Xunit;
using PetCareManagementSystem.Data;
using PetCareManagementSystem.Models;
using PetCareManagementSystem.Services;

namespace PetCareManagementSystem.Tests
{

    public static class TestHelper
    {
        // Delete file contents before each test
        public static void ResetTestFiles()
        {
           string dir = Directory.GetCurrentDirectory();

            FilePaths.UsersFile        = Path.Combine(dir, "test_users.txt");
            FilePaths.PetsFile         = Path.Combine(dir, "test_pets.txt");
            FilePaths.AppointmentsFile = Path.Combine(dir, "test_appointments.txt");
            FilePaths.SuppliesFile     = Path.Combine(dir, "test_supplies.txt");
            FilePaths.VaccinationsFile = Path.Combine(dir, "test_vaccinations.txt");

            foreach (var file in new[]
            {
                FilePaths.UsersFile,
                FilePaths.PetsFile,
                FilePaths.AppointmentsFile,
                FilePaths.SuppliesFile,
                FilePaths.VaccinationsFile
            })
            {
                if (File.Exists(file))
                    File.Delete(file);
            }
        }
    }
 
    // UserService Tests
    public class UserServiceTests
    {
        [Fact]
        public void CreateUser_SavesTheUser()
        {
            TestHelper.ResetTestFiles();
            var service = new UserService();

            service.CreateUser("Alice");
            List<User> users = service.GetUsers();

            Assert.Single(users);
            Assert.Equal("Alice", users[0].Name);
        }

        [Fact]
        public void DeleteUser_RemovesTheUser()
        {
            TestHelper.ResetTestFiles();
            var service = new UserService();

            service.CreateUser("Charlie");
            User user = service.GetUsers()[0];

            service.DeleteUser(user.Id);

            Assert.Empty(service.GetUsers());
        }
    }

    // PetService Tests
    public class PetServiceTests
    {
        private Pet MakePet()
        {
            return new Pet
            {
                Id      = Guid.NewGuid().ToString(),
                UserId  = Guid.NewGuid().ToString(),
                Name    = "Bella",
                Species = "Dog",
                Breed   = "Labrador",
                Age     = 3
            };
        }

        [Fact]
        public void AddPet_SavesThePet()
        {
            TestHelper.ResetTestFiles();
            var service = new PetService();
            Pet pet = MakePet();

            service.AddPet(pet);

            List<Pet> pets = service.GetPetsByUser(pet.UserId);
            Assert.Single(pets);
            Assert.Equal("Bella", pets[0].Name);
        }

        [Fact]
        public void UpdatePet_ChangesTheName()
        {
            TestHelper.ResetTestFiles();
            var service = new PetService();
            Pet pet = MakePet();

            service.AddPet(pet);
            pet.Name = "Bella Updated";
            service.UpdatePet(pet);

            Assert.Equal("Bella Updated", service.GetPetsByUser(pet.UserId)[0].Name);
        }

        [Fact]
        public void DeletePet_RemovesThePet()
        {
            TestHelper.ResetTestFiles();
            var service = new PetService();
            Pet pet = MakePet();

            service.AddPet(pet);
            service.DeletePet(pet.Id);

            Assert.Empty(service.GetPetsByUser(pet.UserId));
        }
    }

    // AppointmentService Tests
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

    // SupplyService Tests
    public class SupplyServiceTests
    {
        [Fact]
        public void AddSupply_SavesTheSupply()
        {
            TestHelper.ResetTestFiles();
            var service = new SupplyService();

            service.AddSupply(new Supply
            {
                Name         = "Dog Food",
                PurchaseDate = DateTime.Now,
                DurationDays = 30
            });

            List<Supply> supplies = service.GetSupplies();
            Assert.Single(supplies);
            Assert.Equal("Dog Food", supplies[0].Name);
        }

        [Fact]
        public void GetLowSupplies_ReturnsSupplyWithOneDayLeft()
        {
            TestHelper.ResetTestFiles();
            var service = new SupplyService();

            // Purchased 29 days ago with a 30-day duration — 1 day left
            service.AddSupply(new Supply
            {
                Name         = "Flea Treatment",
                PurchaseDate = DateTime.Now.AddDays(-29),
                DurationDays = 30
            });

            List<Supply> low = service.GetLowSupplies();

            Assert.Single(low);
            Assert.Equal("Flea Treatment", low[0].Name);
        }

        [Fact]
        public void DeleteSupply_RemovesTheSupply()
        {
            TestHelper.ResetTestFiles();
            var service = new SupplyService();

            Supply supply = new Supply
            {
                Name         = "Cat Food",
                PurchaseDate = DateTime.Now,
                DurationDays = 14
            };

            service.AddSupply(supply);
            service.DeleteSupply(supply);

            Assert.Empty(service.GetSupplies());
        }
    }

    // VaccinationService Tests
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
