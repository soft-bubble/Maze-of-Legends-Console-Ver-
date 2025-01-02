using Spectre.Console;
using Maze_of_Legends;
using System.Runtime.CompilerServices;
using Maze_of_Legends.Classes;
using System.Diagnostics.Metrics;
using System.Transactions;
using System.Net.Http.Headers;

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
                List<int> Skills = new List<int>() { 0, 1, 2, 3, 4, 5 };

                int randomIndexD = random.Next(Skills.Count);
                string mainSkillD = GetMainSkill(Skills[randomIndexD]);

                Skills.RemoveAt(randomIndexD);     //Para evitar la misma habilidad       

                int randomIndexN = random.Next(Skills.Count);
                string mainSkillN = GetMainSkill(Skills[randomIndexN]);

                string GetMainSkill(int skillIndex)
                {
                    switch (skillIndex)
                    {
                        case 0:
                            return "Generate Random Trap";
                        case 1:
                            return "Generate Random Obstacle";
                        case 2:
                            return "Teleport Enemy To Random Address";
                        case 3:
                            return "Teleport Oneself To Random Address";
                        case 4:
                            return "Root Enemy";
                        case 5:
                            return "Steal Speed";
                        default:
                            return string.Empty;
                    }

                }

                demaciaPosition = Maze.getDemaciaPosition();
                noxusPosition = Maze.getNoxusPosition();
                demaciaChampion = new ChampionClass(demaciaChampionName, mainSkillD, demaciaPosition);
                noxusChampion = new ChampionClass(noxusChampionName, mainSkillN , noxusPosition);
                
                int speedSN = 0;
                int speedSD = 0;

                int RootCooldownD = 0;
                int RootCooldownN = 0;

                int secondaryCooldownD = 0;
                int secondaryCooldownN = 0;

                int mainSkillCooldownD = 0;
                int mainSkillCooldownN = 0;

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

                        if (secondaryCooldownD == 0)
                        {
                            demaciaChampion.secondarySkillAvailable = true;
                        }

                        if (mainSkillCooldownD == 0)
                        {
                            demaciaChampion.mainSkillAvailable = true;
                        }

                        demaciaChampion.positionIndex = Maze.demaciaPosition;

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
                                case ConsoleKey.O:                          
                                    Console.Clear();
                                    if (demaciaChampion.secondarySkillAvailable) Maze.RemoveObstacleD();
                                    if (Maze.obstacle)
                                    {
                                        demaciaChampion.secondarySkillAvailable = false;
                                        secondaryCooldownD = 3;
                                        Maze.obstacle = false;
                                        demaciaChampion.speed--;
                                    }
                                    break;
                                case ConsoleKey.P:
                                    Console.Clear();
                                    if (demaciaChampion.mainSkillAvailable)
                                    {
                                        switch (mainSkillD)
                                        {
                                            case "Generate Random Trap":
                                                Maze.GenerateTrap();
                                                break;
                                            case "Generate Random Obstacle":
                                                Maze.GenerateObstacle();
                                                break;
                                            case "Teleport Enemy To Random Address":
                                                Maze.TeleportEnemy("Noxus");
                                                noxusChampion.positionIndex = Maze.noxusPosition;
                                                break;
                                            case "Teleport Oneself To Random Address":
                                                Maze.TeleportSelf("Demacia");
                                                demaciaChampion.positionIndex = Maze.demaciaPosition;
                                                break;
                                            case "Root Enemy":
                                                RootCooldownN = 2;
                                                break;
                                            case "Steal Speed":
                                                speedSN = 1;
                                                noxusChampion.SpeedCooldown(speedSN);
                                                speedSD = 2;
                                                break;
                                        }
                                        mainSkillCooldownD = 3;
                                        demaciaChampion.mainSkillAvailable = false;
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
                            if (secondaryCooldownD != 0) secondaryCooldownD--;
                            if (mainSkillCooldownD != 0) mainSkillCooldownD--;
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

                        if (secondaryCooldownN == 0)
                        {
                            noxusChampion.secondarySkillAvailable = true;
                        }

                        if (mainSkillCooldownN == 0)
                        {
                            noxusChampion.mainSkillAvailable = true;
                        }

                        noxusChampion.positionIndex = Maze.noxusPosition;

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
                                case ConsoleKey.O:
                                    Console.Clear();
                                    if (noxusChampion.secondarySkillAvailable) Maze.RemoveObstacleN();
                                    if (Maze.obstacle)
                                    {
                                        noxusChampion.secondarySkillAvailable = false;
                                        secondaryCooldownN = 3;
                                        Maze.obstacle = false;
                                        noxusChampion.speed--;
                                    }
                                    break;
                                case ConsoleKey.P:
                                    Console.Clear();
                                    if (noxusChampion.mainSkillAvailable)
                                    {
                                        switch (mainSkillN)
                                        {
                                            case "Generate Random Trap":
                                                Maze.GenerateTrap();
                                                break;
                                            case "Generate Random Obstacle":
                                                Maze.GenerateObstacle();
                                                break;
                                            case "Teleport Enemy To Random Address":
                                                Maze.TeleportEnemy("Demacia");
                                                demaciaChampion.positionIndex = Maze.demaciaPosition;
                                                break;
                                            case "Teleport Oneself To Random Address":
                                                Maze.TeleportSelf("Noxus");
                                                noxusChampion.positionIndex = Maze.noxusPosition;
                                                break;
                                            case "Root Enemy":
                                                RootCooldownD = 2;
                                                break;
                                            case "Steal Speed":
                                                speedSD = 1;
                                                demaciaChampion.SpeedCooldown(speedSD);
                                                speedSN = 2;
                                                break;
                                        }
                                        mainSkillCooldownN = 3;
                                        noxusChampion.mainSkillAvailable = false;
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
                            if (secondaryCooldownN != 0) secondaryCooldownN--;
                            if (mainSkillCooldownN != 0) mainSkillCooldownN--;
                            Turn = true;
                        }
                    }
                }
            }
            gameRunning = false;
            running = false; 
        }

        Console.Clear();

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