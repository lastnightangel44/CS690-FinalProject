using PetCareManagementSystem.Models;
using PetCareManagementSystem.Data;

namespace PetCareManagementSystem.Services
{
    /// <summary>
    /// Handles persistence and retrieval of pet records.
    /// Pets are stored as pipe-delimited lines: Id|UserId|Name|Species|Breed|Age
    /// </summary>
    public class PetService
    {
        private FileStorageService storage = new FileStorageService();

        /// <summary>
        /// Saves a new pet record to the pets file.
        /// </summary>
        public void AddPet(Pet pet)
        {
            storage.Save(
                FilePaths.PetsFile,
                $"{pet.Id}|{pet.UserId}|{pet.Name}|{pet.Species}|{pet.Breed}|{pet.Age}"
            );
        }

        /// <summary>
        /// Retrieves all pets belonging to a specific user.
        /// </summary>
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
                        Id      = parts[0],
                        UserId  = parts[1],
                        Name    = parts[2],
                        Species = parts[3],
                        Breed   = parts[4],
                        Age     = int.Parse(parts[5]),
                    });
                }
            }

            return pets;
        }
        
        /// <summary>
        /// Overwrites an existing pet record matched by Id with updated values.
        /// </summary>
        public void UpdatePet(Pet updatedPet)
        {
            string newLine = $"{updatedPet.Id}|{updatedPet.UserId}|{updatedPet.Name}|{updatedPet.Species}|{updatedPet.Breed}|{updatedPet.Age}";

            bool MatchesPet(string line)
            {
                return line.StartsWith(updatedPet.Id + "|");
            }       

            storage.UpdateLine(FilePaths.PetsFile, MatchesPet, newLine);
        }

        /// <summary>
        /// Removes a pet record from the file by its unique ID.
        /// Note: this does not cascade-delete the pet's appointments or vaccinations.
        /// </summary>
        public void DeletePet(string petId)
        {
            bool MatchesPet(string line)
            {
                return line.StartsWith(petId + "|");
            }

            storage.DeleteLine(FilePaths.PetsFile, MatchesPet);
        }
    }
}