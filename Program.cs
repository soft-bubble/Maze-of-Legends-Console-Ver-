using Spectre.Console;
using Maze_of_Legends;
using System.Runtime.CompilerServices;
using Maze_of_Legends.Classes;
using System.Diagnostics.Metrics;
using System.Transactions;

internal class Program
{
    public static MazeGenerator Maze = new MazeGenerator();
    #pragma warning disable //para no obtener la alerta de que gameChampions null
    public static ChampionClass demaciaChampion;
    public static ChampionClass noxusChampion;

    public static Random random = new Random();

    private static void Main(string[] args)
    {
        bool running = true;            //para empezar juego
        bool gameRunning = false;       //para elegir campeones
        bool gameReallyRunning = false; //para jugar
        bool Turn = true;               //para cambiar de turno
        (int x, int y) demaciaPosition;
        (int x, int y) noxusPosition;
        string demaciaChampionName = string.Empty;
        string noxusChampionName = string.Empty;
        bool demaciaWin = false;
        bool noxusWin = false;

        while (running) {
            var firstPage = AnsiConsole.Prompt(new SelectionPrompt<string>()
            .Title("[turquoise4] Welcome to Maze of Legends![/]")
            .PageSize(3)
            .AddChoices(new[] {
                "Start", "Credits", "Exit"
            }));

            if (firstPage == "Start") 
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
                AnsiConsole.Markup("[cornflowerblue]Thanks to me, my family and friends, and Piad for the mind blowing game idea...[/]" + "" +
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
                
                demaciaPosition = Maze.getDemaciaPosition();
                noxusPosition = Maze.getNoxusPosition();
                demaciaChampion = new ChampionClass(demaciaChampionName, "habilidad", demaciaPosition);
                noxusChampion = new ChampionClass(noxusChampionName, "habilidad", noxusPosition);
                
                int speedSN = 0;
                int speedSD = 0;

                int RootCooldownD = 0;
                int RootCooldownN = 0;

                int HoneyFruitsD = 0;
                int HoneyFruitsN = 0;

                List<int> traps = new List<int>() { 0, 1, 2 };
                
                gameReallyRunning = true;
                
                while (gameReallyRunning)
                {

                    if (Turn)
                    {
                        Console.Clear();
                        Maze.PrintMaze();
                        Console.WriteLine();

                        if (RootCooldownD == 0)
                        {
                            demaciaChampion.trapCurse = false;
                        }
                        else
                        {
                            demaciaChampion.speed = 0;
                        }

                        Console.WriteLine(demaciaChampion.ToString());
                        Console.WriteLine("HoneyFruits collected: " + HoneyFruitsD);

                        if (HoneyFruitsD == 3)
                        {
                            gameReallyRunning = false;
                            demaciaWin = true;
                        }

                        ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                        if (keyInfo.Key != ConsoleKey.Enter && demaciaChampion.speed != 0)
                        {
                            switch(keyInfo.Key)
                            {
                                case ConsoleKey.UpArrow:
                                    Console.Clear();
                                    Maze.MoveDemaciaChampion(ConsoleKey.UpArrow);  
                                    demaciaChampion.positionIndex = Maze.demaciaPosition;
                                    if (Maze.isValid)  demaciaChampion.speed--;
                                    if (Maze.trap)
                                    {
                                        int randomIndex = random.Next(traps.Count);
                                        if (traps[randomIndex] == 0)
                                        {
                                            Maze.TeleportTrapD();
                                            demaciaChampion.positionIndex = Maze.demaciaPosition;
                                            demaciaChampion.speed = 0;
                                            Maze.trap = false;
                                        }
                                        else if (traps[randomIndex] == 1)
                                        {
                                            RootCooldownD = 3;
                                            demaciaChampion.speed = 0;
                                            Maze.trap = false;
                                            demaciaChampion.trapCurse = true;
                                        }
                                        else if (traps[randomIndex] == 2)
                                        {
                                            demaciaChampion.speed = 0;
                                            Maze.trap = false;
                                            HoneyFruitsD--;
                                            Maze.GenerateHoneyFruits();
                                        }
                                    }
                                    if (Maze.honeyFruit)
                                    {
                                        HoneyFruitsD++;
                                        Maze.honeyFruit = false;
                                    }
                                    break;
                                case ConsoleKey.DownArrow:
                                    Console.Clear();
                                    Maze.MoveDemaciaChampion(ConsoleKey.DownArrow);
                                    demaciaChampion.positionIndex = Maze.demaciaPosition;
                                    if (Maze.isValid) demaciaChampion.speed--;
                                    if (Maze.trap)
                                    {
                                        int randomIndex = random.Next(traps.Count);
                                        if (traps[randomIndex] == 0)
                                        {
                                            Maze.TeleportTrapD();
                                            demaciaChampion.positionIndex = Maze.demaciaPosition;
                                            demaciaChampion.speed = 0;
                                            Maze.trap = false;
                                        }
                                        else if (traps[randomIndex] == 1)
                                        {
                                            RootCooldownD = 3;
                                            demaciaChampion.speed = 0;
                                            Maze.trap = false;
                                            demaciaChampion.trapCurse = true;
                                        }
                                        else if (traps[randomIndex] == 2)
                                        {
                                            demaciaChampion.speed = 0;
                                            Maze.trap = false;
                                            HoneyFruitsD--;
                                            Maze.GenerateHoneyFruits();
                                        }
                                    }
                                    if (Maze.honeyFruit)
                                    {
                                        HoneyFruitsD++;
                                        Maze.honeyFruit = false;
                                    }
                                    break;
                                case ConsoleKey.LeftArrow:
                                    Console.Clear();
                                    Maze.MoveDemaciaChampion(ConsoleKey.LeftArrow);
                                    demaciaChampion.positionIndex = Maze.demaciaPosition;
                                    if (Maze.isValid) demaciaChampion.speed--;
                                    if (Maze.trap)
                                    {
                                        int randomIndex = random.Next(traps.Count);
                                        if (traps[randomIndex] == 0)
                                        {
                                            Maze.TeleportTrapD();
                                            demaciaChampion.positionIndex = Maze.demaciaPosition;
                                            demaciaChampion.speed = 0;
                                            Maze.trap = false;
                                        }
                                        else if (traps[randomIndex] == 1)
                                        {
                                            RootCooldownD = 3;
                                            demaciaChampion.speed = 0;
                                            Maze.trap = false;
                                            demaciaChampion.trapCurse = true;
                                        }
                                        else if (traps[randomIndex] == 2)
                                        {
                                            demaciaChampion.speed = 0;
                                            Maze.trap = false;
                                            HoneyFruitsD--;
                                            Maze.GenerateHoneyFruits();
                                        }
                                    }
                                    if (Maze.honeyFruit)
                                    {
                                        HoneyFruitsD++;
                                        Maze.honeyFruit = false;
                                    }
                                    break;
                                case ConsoleKey.RightArrow:
                                    Console.Clear();
                                    Maze.MoveDemaciaChampion(ConsoleKey.RightArrow);
                                    demaciaChampion.positionIndex = Maze.demaciaPosition;
                                    if (Maze.isValid) demaciaChampion.speed--;
                                    if (Maze.trap)
                                    {
                                        int randomIndex = random.Next(traps.Count);
                                        if (traps[randomIndex] == 0)
                                        {
                                            Maze.TeleportTrapD();
                                            demaciaChampion.positionIndex = Maze.demaciaPosition;
                                            demaciaChampion.speed = 0;
                                            Maze.trap = false;
                                        }
                                        else if (traps[randomIndex] == 1)
                                        {
                                            RootCooldownD = 3;
                                            demaciaChampion.speed = 0;
                                            Maze.trap = false;
                                            demaciaChampion.trapCurse = true;
                                        }
                                        else if (traps[randomIndex] == 2)
                                        {
                                            demaciaChampion.speed = 0;
                                            Maze.trap = false;
                                            HoneyFruitsD--;
                                            Maze.GenerateHoneyFruits();
                                        }
                                    }
                                    if (Maze.honeyFruit)
                                    {
                                        HoneyFruitsD++;
                                        Maze.honeyFruit = false;
                                    }
                                    break;
                                case ConsoleKey.O:                          //IMPLEMENTAR COOLDOWN
                                    Console.Clear();
                                    if (demaciaChampion.secondarySkillAvailable) Maze.RemoveObstacleD();
                                    if (Maze.obstacle)
                                    {
                                        demaciaChampion.secondarySkillAvailable = false;
                                        Maze.obstacle = false;
                                        demaciaChampion.speed--;
                                    }
                                    break;
                            }
                        }
                        else 
                        {
                            speedSD++;
                            demaciaChampion.SpeedCooldown(speedSD);
                            if (RootCooldownD != 0) RootCooldownD--;
                            Turn = false;
                        }
                    }

                        
                    else if (!Turn)
                    {
                        Console.Clear();
                        Maze.PrintMaze();
                        Console.WriteLine();

                        if (RootCooldownN == 0)
                        {
                            noxusChampion.trapCurse = false;
                        }
                        else
                        {
                            noxusChampion.speed = 0;
                        }

                        Console.WriteLine(noxusChampion.ToString());
                        Console.WriteLine("HoneyFruits collected: " + HoneyFruitsN);

                        if (HoneyFruitsN == 3)
                        {
                            gameReallyRunning = false;
                            noxusWin = true;
                        }

                        ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                        if (keyInfo.Key != ConsoleKey.Enter && noxusChampion.speed != 0)
                        {
                            switch (keyInfo.Key)
                            {
                                case ConsoleKey.UpArrow:
                                    Console.Clear();
                                    Maze.MoveNoxusChampion(ConsoleKey.UpArrow);
                                    noxusChampion.positionIndex = Maze.noxusPosition;
                                    if (Maze.isValid) noxusChampion.speed--;
                                    if (Maze.trap)
                                    {
                                        int randomIndex = random.Next(traps.Count);
                                        if (traps[randomIndex] == 0)
                                        {
                                            Maze.TeleportTrapN();
                                            noxusChampion.positionIndex = Maze.noxusPosition;
                                            noxusChampion.speed = 0;
                                            Maze.trap = false;
                                        }
                                        else if (traps[randomIndex] == 1)
                                        {
                                            RootCooldownN = 3;
                                            noxusChampion.speed = 0;
                                            Maze.trap = false;
                                            noxusChampion.trapCurse = true;
                                        }
                                        else if (traps[randomIndex] == 2)
                                        {
                                            noxusChampion.speed = 0;
                                            Maze.trap = false;
                                            HoneyFruitsN--;
                                            Maze.GenerateHoneyFruits();
                                        }
                                    }
                                    if (Maze.honeyFruit)
                                    {
                                        HoneyFruitsN++;
                                        Maze.honeyFruit = false;
                                    }
                                    break;
                                case ConsoleKey.DownArrow:
                                    Console.Clear();
                                    Maze.MoveNoxusChampion(ConsoleKey.DownArrow);
                                    noxusChampion.positionIndex = Maze.noxusPosition;
                                    if (Maze.isValid) noxusChampion.speed--;
                                    if (Maze.trap)
                                    {
                                        int randomIndex = random.Next(traps.Count);
                                        if (traps[randomIndex] == 0)
                                        {
                                            Maze.TeleportTrapN();
                                            noxusChampion.positionIndex = Maze.noxusPosition;
                                            noxusChampion.speed = 0;
                                            Maze.trap = false;
                                        }
                                        else if (traps[randomIndex] == 1)
                                        {
                                            RootCooldownN = 3;
                                            noxusChampion.speed = 0;
                                            Maze.trap = false;
                                            noxusChampion.trapCurse = true;
                                        }
                                        else if (traps[randomIndex] == 2)
                                        {
                                            noxusChampion.speed = 0;
                                            Maze.trap = false;
                                            HoneyFruitsN--;
                                            Maze.GenerateHoneyFruits();
                                        }
                                    }
                                    if (Maze.honeyFruit)
                                    {
                                        HoneyFruitsN++;
                                        Maze.honeyFruit = false;
                                    }
                                    break;
                                case ConsoleKey.LeftArrow:
                                    Console.Clear();
                                    Maze.MoveNoxusChampion(ConsoleKey.LeftArrow);
                                    noxusChampion.positionIndex = Maze.noxusPosition;
                                    if (Maze.isValid) noxusChampion.speed--;
                                    if (Maze.trap)
                                    {
                                        int randomIndex = random.Next(traps.Count);
                                        if (traps[randomIndex] == 0)
                                        {
                                            Maze.TeleportTrapN();
                                            noxusChampion.positionIndex = Maze.noxusPosition;
                                            noxusChampion.speed = 0;
                                            Maze.trap = false;
                                        }
                                        else if (traps[randomIndex] == 1)
                                        {
                                            RootCooldownN = 3;
                                            noxusChampion.speed = 0;
                                            Maze.trap = false;
                                            noxusChampion.trapCurse = true;
                                        }
                                        else if (traps[randomIndex] == 2)
                                        {
                                            noxusChampion.speed = 0;
                                            Maze.trap = false;
                                            HoneyFruitsN--;
                                            Maze.GenerateHoneyFruits();
                                        }
                                    }
                                    if (Maze.honeyFruit)
                                    {
                                        HoneyFruitsN++;
                                        Maze.honeyFruit = false;
                                    }
                                    break;
                                case ConsoleKey.RightArrow:
                                    Console.Clear();
                                    Maze.MoveNoxusChampion(ConsoleKey.RightArrow);
                                    noxusChampion.positionIndex = Maze.noxusPosition;
                                    if (Maze.isValid) noxusChampion.speed--;
                                    if (Maze.trap)
                                    {
                                        int randomIndex = random.Next(traps.Count);
                                        if (traps[randomIndex] == 0)
                                        {
                                            Maze.TeleportTrapN();
                                            noxusChampion.positionIndex = Maze.noxusPosition;
                                            noxusChampion.speed = 0;
                                            Maze.trap = false;
                                        }
                                        else if (traps[randomIndex] == 1)
                                        {
                                            RootCooldownN = 3;
                                            noxusChampion.speed = 0;
                                            Maze.trap = false;
                                            noxusChampion.trapCurse = true;
                                        }
                                        else if (traps[randomIndex] == 2)
                                        {
                                            noxusChampion.speed = 0;
                                            Maze.trap = false;
                                            HoneyFruitsN--;
                                            Maze.GenerateHoneyFruits();
                                        }
                                    }
                                    if (Maze.honeyFruit)
                                    {
                                        HoneyFruitsN++;
                                        Maze.honeyFruit = false;
                                    }
                                    break;
                                case ConsoleKey.O:                     //IMPLEMENTAR COOLDOWN
                                    Console.Clear();
                                    if (noxusChampion.secondarySkillAvailable) Maze.RemoveObstacleN();
                                    if (Maze.obstacle)
                                    {
                                        noxusChampion.secondarySkillAvailable = false;
                                        Maze.obstacle = false;
                                        noxusChampion.speed--;
                                    }
                                    break;
                            }
                        }
                        else 
                        {
                            speedSN++;
                            noxusChampion.SpeedCooldown(speedSN);
                            if (RootCooldownN != 0) RootCooldownN--;
                            Turn = true;
                        }
                    }
                }
            }
            gameRunning = false;
            running = false; 
        }

        if (demaciaWin)
        {
            Console.WriteLine("Demacia won.");
        }
        else if (noxusWin)
        {
            Console.WriteLine("Noxus won.");
        }
    }
}