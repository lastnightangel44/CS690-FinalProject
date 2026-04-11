using System;
using System.Collections.Generic;
using Xunit;
using PetCareManagementSystem.Models;
using PetCareManagementSystem.Services;

namespace PetCareManagementSystem.Tests
{
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
}
