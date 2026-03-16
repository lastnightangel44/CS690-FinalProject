using PetCareManagementSystem.Data;
using PetCareManagementSystem.Models;

namespace PetCareManagementSystem.Services
{
    public class UserService
    {
        private FileStorageService storage = new FileStorageService();

        public void CreateUser(string name)
        {
            string id = Guid.NewGuid().ToString();
            storage.Save(FilePaths.UsersFile, $"{id}|{name}");
        }

        public List<User> GetUsers()
        {
            var lines = storage.Load(FilePaths.UsersFile);
            var users = new List<User>();

            foreach (var line in lines)
            {
                var parts = line.Split('|');

                users.Add(new User
                {
                    Id = parts[0],
                    Name = parts[1]
                });
            }

            return users;
        }
    }
}