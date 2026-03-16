using PetCareManagementSystem.Models;
using PetCareManagementSystem.Data;

namespace PetCareManagementSystem.Services
{
    public class PetService
    {
        private FileStorageService storage = new FileStorageService();

        public void AddPet(Pet pet)
        {
            storage.Save(
                FilePaths.PetsFile,
                $"{pet.Id}|{pet.UserId}|{pet.Name}|{pet.Species}|{pet.Breed}|{pet.Age}|{pet.Weight}"
            );
        }

        public List<Pet> GetPetsByUser(string userId)
        {
            var lines = storage.Load(FilePaths.PetsFile);
            var pets = new List<Pet>();

            foreach (var line in lines)
            {
                var parts = line.Split('|');

                if (parts[1] == userId)
                {
                    pets.Add(new Pet
                    {
                        Id = parts[0],
                        UserId = parts[1],
                        Name = parts[2],
                        Species = parts[3],
                        Breed = parts[4],
                        Age = int.Parse(parts[5]),
                        Weight = float.Parse(parts[6])
                    });
                }
            }

            return pets;
        }

        public void DeletePet(string petId)
        {
            storage.DeleteLine(FilePaths.PetsFile, line => line.StartsWith(petId + "|"));
        }
    }
}