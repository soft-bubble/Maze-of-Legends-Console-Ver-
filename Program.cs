using Spectre.Console;
using Maze_of_Legends;
using System.Runtime.CompilerServices;
using Maze_of_Legends.Classes;

internal class Program
{
    public static MazeGenerator Maze = new MazeGenerator();
    #pragma warning disable //para no obtener la alerta de que gameChampions null
    public static ChampionGenerator demaciaChampion;
    public static ChampionGenerator noxusChampion;

    private static void Main(string[] args)
    {
        bool running = true;
        bool gameRunning = false;
        bool gameReallyRunning = false;
        bool Turn = true; 
        (int x, int y) demaciaPosition;
        (int x, int y) noxusPosition;
        string demaciaChampionName = string.Empty;
        string noxusChampionName = string.Empty;

        while (running) {
            var firstPage = AnsiConsole.Prompt(new SelectionPrompt<string>()
            .Title("[turquoise4] Welcome to Maze of Legends![/]")
            .PageSize(3)
            .AddChoices(new[] {
                "Start", "Credits", "Exit"
            }));

            if (firstPage == "Start") // TODO: Faltan credits y exit
            {
                demaciaChampionName = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("[turquoise4] Select your Demacia Champion:[/]")
                .PageSize(5)
                .AddChoices(new[] {
                "Garen", "Lux", "Sona", "Vayne", "Shyvanna"
                }));

                noxusChampionName = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("[turquoise4] Select your Noxus Champion:[/]")
                .PageSize(5)
                .AddChoices(new[] {
                "Ambessa", "Swain", "Mordekaiser", "Katarina", "Samira"
                }));

                gameRunning = true;

                
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



            if (gameRunning)
            {
                demaciaPosition = Maze.demaciaPosition;
                noxusPosition = Maze.noxusPosition;
                demaciaChampion = new ChampionGenerator(demaciaChampionName, demaciaPosition);
                demaciaChampion.SelectGeneratedChampion();
                noxusChampion = new ChampionGenerator(noxusChampionName, noxusPosition);
                noxusChampion.SelectGeneratedChampion();
                gameReallyRunning = true;
                
                while (gameReallyRunning)
                {
                    if (Turn)
                    {
                        Console.Clear();
                        Maze.PrintMaze();
                        Console.WriteLine();
                        Console.WriteLine(demaciaChampion.DisplayStatus());
                        ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                        if (keyInfo.Key != ConsoleKey.Enter)
                        {
                            switch(keyInfo.Key)
                                {
                                    case ConsoleKey.UpArrow:
                                    break;
                                    case ConsoleKey.DownArrow: 
                                    break;
                                    case ConsoleKey.LeftArrow:
                                    break;
                                    case ConsoleKey.RightArrow:
                                    break;

                                }
                        }
                        else { Turn = false; }
                    }
                    else if (!Turn)
                    {
                        Console.Clear();
                        Maze.PrintMaze();
                        Console.WriteLine();
                        Console.WriteLine(noxusChampion.DisplayStatus());
                        ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                        if (keyInfo.Key != ConsoleKey.Enter)
                        {
                            switch (keyInfo.Key)
                            {
                                case ConsoleKey.UpArrow:
                                    break;
                                case ConsoleKey.DownArrow:
                                    break;
                                case ConsoleKey.LeftArrow:
                                    break;
                                case ConsoleKey.RightArrow:
                                    break;

                            }
                        }
                        else { Turn = true; }
                    }

                }

                

            }







            gameRunning = false;
             running = false;
            
        }
    }
}