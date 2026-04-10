using PetCareManagementSystem.Data;
using PetCareManagementSystem.Models;

namespace PetCareManagementSystem.Services
{
    /// <summary>
    /// Manages user account creation and retrieval.
    /// Users are stored as pipe-delimited lines: Id|Name
    /// </summary>
    public class UserService
    {
        private FileStorageService storage = new FileStorageService();

        /// <summary>
        /// Creates a new user with a generated unique ID and saves them to the users file.
        /// </summary>
        public void CreateUser(string name)
        {
            string id = Guid.NewGuid().ToString();
            storage.Save(FilePaths.UsersFile, $"{id}|{name}");
        }

        /// <summary>
        /// Loads and returns all registered users.
        /// </summary>
        public List<User> GetUsers()
        {
            var lines = storage.Load(FilePaths.UsersFile);
            var users = new List<User>();

            foreach (var line in lines)
            {
                var parts = line.Split('|');

                users.Add(new User
                {
                    Id      = parts[0],
                    Name    = parts[1]
                });
            }

            return users;
        }

        /// <summary>
        /// Updates the name of an existing user matched by their Id.
        /// </summary>
        public void UpdateUser(User updatedUser)
        {
            string newLine = $"{updatedUser.Id}|{updatedUser.Name}";

            bool MatchesUser(string line)
            {
                return line.StartsWith(updatedUser.Id + "|");
            }

            storage.UpdateLine(FilePaths.UsersFile, MatchesUser, newLine);
        }

        /// <summary>
        /// Deletes a user record by their unique ID.
        /// </summary>
        public void DeleteUser(string userId)
        {
            bool MatchesUser(string line)
            {
                return line.StartsWith(userId + "|");
            }

            storage.DeleteLine(FilePaths.UsersFile, MatchesUser);
        }
    }
}