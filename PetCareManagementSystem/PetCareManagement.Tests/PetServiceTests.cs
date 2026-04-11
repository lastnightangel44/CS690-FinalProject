using System;
using System.Collections.Generic;
using Xunit;
using PetCareManagementSystem.Models;
using PetCareManagementSystem.Services;

namespace PetCareManagementSystem.Tests
{
    public class PetServiceTests
    {
        private Pet Test_MakePet()
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
        public void Test_AddPet()
        {
            TestHelper.ResetTestFiles();
            var service = new PetService();
            Pet pet = Test_MakePet();

            service.AddPet(pet);
            List<Pet> pets = service.GetPetsByUser(pet.UserId);

            Assert.Single(pets);
            Assert.Equal("Bella", pets[0].Name);
        }

        [Fact]
        public void Test_UpdatePet()
        {
            TestHelper.ResetTestFiles();
            var service = new PetService();
            Pet pet = Test_MakePet();

            service.AddPet(pet);
            pet.Name = "Bella Updated";
            service.UpdatePet(pet);

            Assert.Equal("Bella Updated", service.GetPetsByUser(pet.UserId)[0].Name);
        }

        [Fact]
        public void Test_DeletePet()
        {
            TestHelper.ResetTestFiles();
            var service = new PetService();
            Pet pet = Test_MakePet();

            service.AddPet(pet);
            service.DeletePet(pet.Id);

            Assert.Empty(service.GetPetsByUser(pet.UserId));
        }
    }
}
