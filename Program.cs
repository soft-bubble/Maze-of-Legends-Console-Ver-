using Spectre.Console;
using Maze_of_Legends;
using System.Runtime.CompilerServices;
using Maze_of_Legends.Classes;
using System.Diagnostics.Metrics;
using System.Transactions;
using System.Net.Http.Headers;
using System.Diagnostics;
using System.IO;
using System.Collections.Immutable;
using static System.Net.Mime.MediaTypeNames;

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
        bool demaciaWin = false;      //victoria de demacia
        bool noxusWin = false;        //victoria de noxus

        while (running) {
            var firstPage = AnsiConsole.Prompt(new SelectionPrompt<string>()
            .Title("[turquoise4] Welcome to Maze of Legends![/]")
            .PageSize(4)
            .AddChoices(new[] {
                "Start", "Credits", "Controls", "Exit"
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
                var panelCr = new Panel(new Markup("[bold turquoise4]Thanks to my family[/] for supporting me throughout the whole process (I didn't get to wash many dishes) and for giving me some ideas.\n" +
                                  "[bold turquoise4]Thanks to my friends[/] for keeping my morale up, and [bold turquoise4]thanks to myself[/] for not having yet another crisis during this project.\n" +
                                  "[bold turquoise4]Also, thanks to you[/] for not creating yet another massacre with some Unity game idea."))
                {
                    Header = new PanelHeader("[bold white]Acknowledgments[/]"),
                    Border = BoxBorder.Square
                };

                AnsiConsole.Render(panelCr);
                Console.ReadLine();
                running = false;
            }
            if (firstPage == "Controls")
            {
                var Up = Emoji.Known.UpArrow;
                var Down = Emoji.Known.DownArrow;
                var Left = Emoji.Known.LeftArrow;
                var Right = Emoji.Known.RightArrow;

                var panelCo1 = new Panel(new Markup($"[turquoise4]Move up:[/] {Up}\n" 
                    + $"[turquoise4]Move down:[/] {Down}\n" 
                    + $"[turquoise4]Move left:[/] {Left}\n"
                    + $"[turquoise4]Move right:[/] {Right}\n"
                    + "[turquoise4]Use Secondary Skill:[/] O\n"
                    + "[turquoise4]Use Main Skill:[/] P\n"
                    + "[turquoise4]Pass turn:[/] Enter\n"
                    ))
                {
                    Header = new PanelHeader("[bold white]Game Controls[/]"),
                    Border = BoxBorder.Square
                };
                AnsiConsole.Render(panelCo1);

                var wall = Emoji.Known.BlueSquare;
                var path = Emoji.Known.BlackCircle;
                var trap = Emoji.Known.Cyclone;
                var obstacle = Emoji.Known.ChequeredFlag;
                var honeyFruits = Emoji.Known.Peach;
                var demaciaPlayer = Emoji.Known.DimButton;
                var noxusPlayer = Emoji.Known.CrescentMoon;

                var panelCo5 = new Panel(new Markup($"[turquoise4]Wall:[/] {wall}\n"
                    + $"[turquoise4]Path:[/] {path}\n"
                    + $"[turquoise4]Trap:[/] {trap}\n"
                    + $"[turquoise4]Obstacle:[/] {obstacle}\n"
                    + $"[turquoise4]HoneyFruit:[/] {honeyFruits}\n"
                    + $"[turquoise4]Demacia Champion:[/] {demaciaPlayer}\n"
                    + $"[turquoise4]Noxus Champion:[/] {noxusPlayer}\n"
                    ))
                {
                    Header = new PanelHeader("[bold white]Game Objects[/]"),
                    Border = BoxBorder.Square
                };
                AnsiConsole.Render(panelCo5);

                var panelCo2 = new Panel(new Markup("[turquoise4]Generate Random Trap:[/] A trap is generated randomly at some valid position.\n"
                    + "[turquoise4]Generate Random Obstacle:[/] An obstacle is generated randomly at some valid position.\n"
                    + "[turquoise4]Teleport Enemy To Random Address:[/] The enemy player is sent to a random valid position.\n"
                    + "[turquoise4]Teleport Oneself To Random Address:[/] The user of the skill teleports itself to a random valid position.\n"
                    + "[turquoise4]Root Enemy:[/] Enemy remains rooted to the current position during two turns.\n"
                    + "[turquoise4]Steal Speed:[/] Enemy speed is set to one, user speed is set to three for the next turn.\n"
                    + "[turquoise4]Remove Obstacle (Permanent secondary skill):[/] Removes the immediate obstacle.\n"
                     ))
                {
                    Header = new PanelHeader("[bold white]Skills tree:[/]"),
                    Border = BoxBorder.Square
                };
                AnsiConsole.Render(panelCo2);

                var panelCo3 = new Panel(new Markup("[turquoise4]Teleport Trap:[/] The victim is teleported to its initial position.\n"
                    + "[turquoise4]Rooted Trap:[/] The victim remains rooted to the place during three turns.\n"
                    + "[turquoise4]Lose a fruit:[/] The victim loses a fruit, which falls somewhere in the labyrinth. If the victim has no fruit, they receive a negative point to refill.\n"
                     ))
                {
                    Header = new PanelHeader("[bold white]Traps:[/]"),
                    Border = BoxBorder.Square
                };
                AnsiConsole.Render(panelCo3);

                var panelCo4 = new Panel(new Markup("Each player starts at a random position, with the champion they chose and a random skill.\n"
                    + "The HoneyFruit counter must be filled with three fruits for the victory condition to be met.\n"
                    + "Each champion has a speed that goes in a rhythm of 3-2-1 steps per turn.\n"
                    + "Each skill consumes one speed point. Skills, either main or secondary, have a three-turn cooldown, separately.\n"
                    + "Stepping on a trap reduces the current speed to 0.\n"
                    + "If two champions meet at the same cell, the one previously there is pushed to the currenlty moving champion's previous cell."
                    ))
                {
                    Header = new PanelHeader("[bold white]Game mission and specifications:[/]"),
                    Border = BoxBorder.Square
                };
                AnsiConsole.Render(panelCo4);

                Console.ReadLine();
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

                List<int> traps = new List<int>() { 0, 1, 2, 3 };
                
                gameReallyRunning = true;
                
                while (gameReallyRunning)
                {

                    if (Turn)
                    {
                        Console.Clear();

                        var panelD = new Panel(new Markup($" HoneyFruits: {HoneyFruitsD}"))
                        {
                            Border = BoxBorder.Square
                        };
                        AnsiConsole.Render(panelD);

                        var tableD = new Table();

                        tableD.AddColumn("Counter");
                        tableD.AddColumn("Value");

                        tableD.AddRow("Root Cooldown", RootCooldownD.ToString());
                        tableD.AddRow($"{mainSkillD} Cooldown", mainSkillCooldownD.ToString());
                        tableD.AddRow("Remove Wall Cooldown", secondaryCooldownD.ToString());

                        AnsiConsole.Write(tableD);

                        Maze.PrintMaze();

                        Console.WriteLine();

                        if (RootCooldownD == 0)
                        {
                            demaciaChampion.Cursed = false;
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

                        

                        demaciaChampion.PrintInfo();

                        

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
                                            demaciaChampion.Cursed = true;
                                        }
                                        else if (traps[randomIndex] == 2)
                                        {
                                            demaciaChampion.speed = 0;
                                            Maze.trap = false;
                                            HoneyFruitsD--;
                                            Maze.GenerateHoneyFruits();
                                        }
                                        else if ((traps[randomIndex] == 3))
                                        {
                                            demaciaChampion.speed = 0;
                                            Maze.trap = false;
                                            demaciaChampion.mainSkillAvailable = false;
                                            mainSkillCooldownD = 2;
                                            demaciaChampion.secondarySkillAvailable = false;
                                            secondaryCooldownD = 2;
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
                                            demaciaChampion.Cursed = true;
                                        }
                                        else if (traps[randomIndex] == 2)
                                        {
                                            demaciaChampion.speed = 0;
                                            Maze.trap = false;
                                            HoneyFruitsD--;
                                            Maze.GenerateHoneyFruits();
                                        }
                                        else if ((traps[randomIndex] == 3))
                                        {
                                            demaciaChampion.speed = 0;
                                            Maze.trap = false;
                                            demaciaChampion.mainSkillAvailable = false;
                                            mainSkillCooldownD = 2;
                                            demaciaChampion.secondarySkillAvailable = false;
                                            secondaryCooldownD = 2;
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
                                            demaciaChampion.Cursed = true;
                                        }
                                        else if (traps[randomIndex] == 2)
                                        {
                                            demaciaChampion.speed = 0;
                                            Maze.trap = false;
                                            HoneyFruitsD--;
                                            Maze.GenerateHoneyFruits();
                                        }
                                        else if ((traps[randomIndex] == 3))
                                        {
                                            demaciaChampion.speed = 0;
                                            Maze.trap = false;
                                            demaciaChampion.mainSkillAvailable = false;
                                            mainSkillCooldownD = 2;
                                            demaciaChampion.secondarySkillAvailable = false;
                                            secondaryCooldownD = 2;
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
                                            demaciaChampion.Cursed = true;
                                        }
                                        else if (traps[randomIndex] == 2)
                                        {
                                            demaciaChampion.speed = 0;
                                            Maze.trap = false;
                                            HoneyFruitsD--;
                                            Maze.GenerateHoneyFruits();
                                        }
                                        else if ((traps[randomIndex] == 3))
                                        {
                                            demaciaChampion.speed = 0;
                                            Maze.trap = false;
                                            demaciaChampion.mainSkillAvailable = false;
                                            mainSkillCooldownD = 2;
                                            demaciaChampion.secondarySkillAvailable = false;
                                            secondaryCooldownD = 2;
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
                                                speedSN = 2;
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

                        var panelN = new Panel(new Markup($" HoneyFruits: {HoneyFruitsN}"))
                        {
                            Border = BoxBorder.Square
                        };
                        AnsiConsole.Render(panelN);

                        var tableN = new Table();

                        tableN.AddColumn("Counter");
                        tableN.AddColumn("Value");

                        tableN.AddRow("Root Cooldown", RootCooldownN.ToString());
                        tableN.AddRow($"{mainSkillN} Cooldown", mainSkillCooldownN.ToString());
                        tableN.AddRow("Remove Wall Cooldown", secondaryCooldownN.ToString());

                        AnsiConsole.Write(tableN);

                        Maze.PrintMaze();
                        Console.WriteLine();

                        if (RootCooldownN == 0)
                        {
                            noxusChampion.Cursed = false;
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

                        

                        noxusChampion.PrintInfo();

                        

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
                                            noxusChampion.Cursed = true;
                                        }
                                        else if (traps[randomIndex] == 2)
                                        {
                                            noxusChampion.speed = 0;
                                            Maze.trap = false;
                                            HoneyFruitsN--;
                                            Maze.GenerateHoneyFruits();
                                        }
                                        else if ((traps[randomIndex] == 3))
                                        {
                                            noxusChampion.speed = 0;
                                            Maze.trap = false;
                                            noxusChampion.mainSkillAvailable = false;
                                            mainSkillCooldownN = 2;
                                            noxusChampion.secondarySkillAvailable = false;
                                            secondaryCooldownN = 2;
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
                                            noxusChampion.Cursed = true;
                                        }
                                        else if (traps[randomIndex] == 2)
                                        {
                                            noxusChampion.speed = 0;
                                            Maze.trap = false;
                                            HoneyFruitsN--;
                                            Maze.GenerateHoneyFruits();
                                        }
                                        else if ((traps[randomIndex] == 3))
                                        {
                                            noxusChampion.speed = 0;
                                            Maze.trap = false;
                                            noxusChampion.mainSkillAvailable = false;
                                            mainSkillCooldownN = 2;
                                            noxusChampion.secondarySkillAvailable = false;
                                            secondaryCooldownN = 2;
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
                                            noxusChampion.Cursed = true;
                                        }
                                        else if (traps[randomIndex] == 2)
                                        {
                                            noxusChampion.speed = 0;
                                            Maze.trap = false;
                                            HoneyFruitsN--;
                                            Maze.GenerateHoneyFruits();
                                        }
                                        else if ((traps[randomIndex] == 3))
                                        {
                                            noxusChampion.speed = 0;
                                            Maze.trap = false;
                                            noxusChampion.mainSkillAvailable = false;
                                            mainSkillCooldownN = 2;
                                            noxusChampion.secondarySkillAvailable = false;
                                            secondaryCooldownN = 2;
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
                                            noxusChampion.Cursed = true;
                                        }
                                        else if (traps[randomIndex] == 2)
                                        {
                                            noxusChampion.speed = 0;
                                            Maze.trap = false;
                                            HoneyFruitsN--;
                                            Maze.GenerateHoneyFruits();
                                        }
                                        else if ((traps[randomIndex] == 3))
                                        {
                                            noxusChampion.speed = 0;
                                            Maze.trap = false;
                                            noxusChampion.mainSkillAvailable = false;
                                            mainSkillCooldownN = 2;
                                            noxusChampion.secondarySkillAvailable = false;
                                            secondaryCooldownN = 2;
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
                                                speedSD = 2;
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
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(@"
    ┌───────────────────────────────┐
    │                               │
    │     ╔═══════════════╗         │
    │     ║ DEMACIA WINS! ║         │
    │     ╚═══════════════╝         │
    │                               │
    └───────────────────────────────┘
    ");
            Console.ResetColor();
        }
        else if (noxusWin)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(@"
    ┌───────────────────────────────┐
    │                               │
    │     ╔═══════════════╗         │
    │     ║ NOXUS WINS!   ║         │
    │     ╚═══════════════╝         │
    │                               │
    └───────────────────────────────┘
    ");
            Console.ResetColor();
        }
    }
}