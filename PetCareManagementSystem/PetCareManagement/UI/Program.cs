using PetCareManagementSystem.UI;

namespace PetCareManagementSystem
{
    /// <summary>
    /// Application entry point. Runs the console menu.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Console.Clear();
            ConsoleMenu menu = new ConsoleMenu();
            menu.Run();
        }
    }
}