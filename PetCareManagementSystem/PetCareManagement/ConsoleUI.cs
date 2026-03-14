namespace PetCareManagement;

using Spectre.Console;

public class ConsoleUI
{
    
    public void Show()
    {
        var user = AnsiConsole.Prompt()
    }

    public static string AskForInput(string message) {
        Console.Write(message);
        return Console.ReadLine();
    }
}
