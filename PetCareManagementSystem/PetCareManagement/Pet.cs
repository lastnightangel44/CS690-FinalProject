namespace PetCareManagementSystem.Models
{
    public class Pet
    {
        public string Id { get; set; }
        public string UserId { get; set; }

        public string Name { get; set; }
        public string Species { get; set; }
        public string Breed { get; set; }
        public int Age { get; set; }

    }
}