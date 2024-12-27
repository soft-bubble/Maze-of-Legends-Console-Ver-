using Spectre.Console;
using Maze_of_Legends;
using System.Runtime.CompilerServices;

internal class Program
{
    public static MazeGenerator Maze = new MazeGenerator();
    

    private static void Main(string[] args)
    {
        bool running = true;
        bool gameRunnning = false;

        while (running) {
            var firstPage = AnsiConsole.Prompt(new SelectionPrompt<string>()
            .Title("[turquoise4] Welcome to Maze of Legends![/]")
            .PageSize(3)
            .AddChoices(new[] {
                "Start", "Credits", "Exit"
            }));

            if (firstPage == "Start") // TODO: Faltan credits y exit
            {
                var secondPage = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("[turquoise4] Select your Demacia Champion:[/]")
                .PageSize(5)
                .AddChoices(new[] {
                "Garen", "Lux", "Sona", "Vayne", "Shyvanna"
                }));

                var thirdPage = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("[turquoise4] Select your Noxus Champion:[/]")
                .PageSize(5)
                .AddChoices(new[] {
                "Ambessa", "Swain", "Mordekaiser", "Katarina", "Samira"
                }));

                gameRunnning = true;
            }
            if (firstPage == "Credits")
            {
                AnsiConsole.Markup("[cornflowerblue]Thanks to me, Monica, my family and friends, and Piad for the mind blowing game idea...[/]" + "" +
                    "\n[maroon]Really, that free will of interface fucked me up.[/]"); 
                running = false;
            }
            if (firstPage == "Exit")
            { 
                Console.Clear(); 
                running = false;
            }

            while (gameRunnning)
            {

            }
        }
    }
}