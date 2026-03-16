using PetCareManagementSystem.Models;
using PetCareManagementSystem.Services;

namespace PetCareManagementSystem.UI
{
    public class ConsoleMenu
    {
        private UserService userService = new UserService();

        public User Start()
        {
            while (true)
            {
                Console.WriteLine("==== Pet Care System ====");
                Console.WriteLine("1 Select User");
                Console.WriteLine("2 Create User");
                Console.WriteLine("0 Exit");

                var choice = Console.ReadLine();

                if (choice == "1")
                    return SelectUser();

                if (choice == "2")
                    CreateUser();

                if (choice == "0")
                    Environment.Exit(0);
            }
        }

        private User SelectUser()
        {
            var users = userService.GetUsers();

            // NEW: force user creation if none exist
            if (users.Count == 0)
            {
                Console.WriteLine("No users found. Please create a user.");

                CreateUser();

                users = userService.GetUsers();
            }

            Console.WriteLine("\nSelect a user:");

            for (int i = 0; i < users.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {users[i].Name}");
            }

            int choice = int.Parse(Console.ReadLine());

            return users[choice - 1];
        }

        private void CreateUser()
        {
            Console.Write("Enter name: ");
            var name = Console.ReadLine();

            userService.CreateUser(name);

            Console.WriteLine("User created.\n");
        }
    }
}