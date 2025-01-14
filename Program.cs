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
    public static MazeGenerator Maze = new MazeGenerator(); //iniciar el laberinto
    #pragma warning disable //para no obtener la alerta de que gameChampions null
    public static ChampionClass demaciaChampion; //iniciar los campeones sin valores
    public static ChampionClass noxusChampion;

    public static Random random = new Random();

    private static void Main(string[] args)
    {
        bool running = true;            //para empezar juego
        bool gameRunning = false;       //para elegir campeones
        bool gameReallyRunning = false; //para jugar
        bool Turn = true;               //para cambiar de turno
        (int x, int y) demaciaPosition; //posición de cada campeón
        (int x, int y) noxusPosition;
        string demaciaChampionName = string.Empty;  //nombre a seleccionar de cada campeón
        string noxusChampionName = string.Empty;
        bool demaciaWin = false;      //victoria de demacia
        bool noxusWin = false;        //victoria de noxus

        while (running) {
            var firstPage = AnsiConsole.Prompt(new SelectionPrompt<string>()  //primera página a mostrar
            .Title("[turquoise4] Welcome to Maze of Legends![/]")
            .PageSize(4)
            .AddChoices(new[] {
                "Start", "Credits", "Controls", "Exit"
            })); 

            if (firstPage == "Start") 
            {
                demaciaChampionName = AnsiConsole.Prompt(new SelectionPrompt<string>() //primer jugador elige personaje
                .Title("[turquoise4] Select your Demacia Champion:[/]")
                .PageSize(5)
                .AddChoices(new[] {
                "Garen", "Lux", "Sona", "Vayne", "Shyvanna"
                }));

                noxusChampionName = AnsiConsole.Prompt(new SelectionPrompt<string>() //segundo jugador elige personaje
                .Title("[turquoise4] Select your Noxus Champion:[/]")
                .PageSize(5)
                .AddChoices(new[] {
                "Ambessa", "Swain", "Mordekaiser", "Katarina", "Samira"
                }));

                gameRunning = true; //el juego inicia 

                
            }
            if (firstPage == "Credits") //página de los créditos
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
                running = false; //se termina la ejecución del while
            }
            if (firstPage == "Controls") //explicación de controles, objetivos y visual 
            {
                //Visual de las teclas con emojis:
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
                AnsiConsole.Render(panelCo1); //cuadro de controles

                //Visual del laberinto:
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
                AnsiConsole.Render(panelCo5); //cuadro de visual

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
                AnsiConsole.Render(panelCo2); //cuadro de habilidades

                var panelCo3 = new Panel(new Markup("[turquoise4]Teleport Trap:[/] The victim is teleported to its initial position.\n"
                    + "[turquoise4]Rooted Trap:[/] The victim remains rooted to the place during three turns.\n"
                    + "[turquoise4]Lose a fruit:[/] The victim loses a fruit, which falls somewhere in the labyrinth. If the victim has no fruit, they receive a negative point to refill.\n"
                     ))
                {
                    Header = new PanelHeader("[bold white]Traps:[/]"),
                    Border = BoxBorder.Square
                };
                AnsiConsole.Render(panelCo3); //cuadro de trampas

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
                AnsiConsole.Render(panelCo4); //cuadro de lógica del juego

                Console.ReadLine();
                running = false; //se termina la ejecución del while
            }
            if (firstPage == "Exit")
            { 
                Console.Clear();
                running = false; //se termina la ejecución del while
            }



            if (gameRunning) //si se eligen los personajes, inicia el juego
            {
                List<int> Skills = new List<int>() { 0, 1, 2, 3, 4, 5 }; //lista para determinar la habilidad de cada campeón

                int randomIndexD = random.Next(Skills.Count); //random para elegir hab. de demacia
                string mainSkillD = GetMainSkill(Skills[randomIndexD]); // se llama al string de hab. para asignar un valor

                Skills.RemoveAt(randomIndexD);     //para evitar la misma habilidad       

                int randomIndexN = random.Next(Skills.Count); //idem. para noxus
                string mainSkillN = GetMainSkill(Skills[randomIndexN]);

                string GetMainSkill(int skillIndex) //selección de string para habilidad principal en dependencia del int
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

                demaciaPosition = Maze.getDemaciaPosition(); //se busca la posición inicial del campeon al inicializarse el laberinto
                noxusPosition = Maze.getNoxusPosition();
                demaciaChampion = new ChampionClass(demaciaChampionName, mainSkillD, demaciaPosition); //se inicializa cada campeón con su nombre, hab. principal y posición inicial
                noxusChampion = new ChampionClass(noxusChampionName, mainSkillN , noxusPosition);
                
                int speedSN = 0; //contador para mantener ritmo 3->2->1 en la velocidad de cada campeón por turno
                int speedSD = 0;

                int RootCooldownD = 0; //contador para duración del stun por efecto trampa o habilidad enemiga
                int RootCooldownN = 0;

                int secondaryCooldownD = 0; //contador para enfriamiento de la hab. secundaria
                int secondaryCooldownN = 0;

                int mainSkillCooldownD = 0; //contador para enfriamiento de la hab. principal
                int mainSkillCooldownN = 0;

                int HoneyFruitsD = 0; //contador para la cantidad de frutas en posesión
                int HoneyFruitsN = 0;

                List<int> traps = new List<int>() { 0, 1, 2, 3 }; //lista para selección de trampas
                
                gameReallyRunning = true; //iniciar la impresión de laberinto y juego [fr]
                
                while (gameReallyRunning) //empieza juego tras inicializar jugadores y laberinto
                {

                    if (Turn) //turno de demacia
                    {
                        Console.Clear();

                        var panelD = new Panel(new Markup($" HoneyFruits: {HoneyFruitsD}"))
                        {
                            Border = BoxBorder.Square
                        };
                        AnsiConsole.Render(panelD); //panel de cantidad de frutas

                        var tableD = new Table(); //tabla de enfrimientos y contadores

                        tableD.AddColumn("Counter");
                        tableD.AddColumn("Value");

                        tableD.AddRow("Root Cooldown", RootCooldownD.ToString());
                        tableD.AddRow($"{mainSkillD} Cooldown", mainSkillCooldownD.ToString());
                        tableD.AddRow("Remove Wall Cooldown", secondaryCooldownD.ToString());

                        AnsiConsole.Write(tableD);

                        Maze.PrintMaze(); //imprimir laberinto

                        Console.WriteLine(); //espacio de una línea [estética]

                        if (RootCooldownD == 0) //verificar que no se encuentre bajo efecto stun
                        {
                            demaciaChampion.Cursed = false; //la maldición stun de trampa se elimina
                        }
                        else
                        {
                            demaciaChampion.speed = 0; //la maldición stun de trampa se mantiene, no es posible mover o ejecutar habilidades
                        }

                        if (secondaryCooldownD == 0) //verificar si se puede utilizar la hab. secundaria
                        {
                            demaciaChampion.secondarySkillAvailable = true;
                        }

                        if (mainSkillCooldownD == 0) //verificar si se puede utilizar hab. principal
                        {
                            demaciaChampion.mainSkillAvailable = true;
                        }

                        demaciaChampion.positionIndex = Maze.demaciaPosition; //verificar posición actual del campeón

                        demaciaChampion.PrintInfo(); //imprimir status del campeón actualizado, método en ChampionClass

                        if (HoneyFruitsD == 3) //si se cumple la condición de victoria, salir del ciclo y activar victoria de demacia
                        {
                            gameReallyRunning = false;
                            demaciaWin = true;
                        }

                        ConsoleKeyInfo keyInfo = Console.ReadKey(true); //leer tecla
                        if (keyInfo.Key != ConsoleKey.Enter && demaciaChampion.speed != 0) //se ejecuta solo si no se presiona enter o si existe disponibilidad de velocidad
                        {
                            switch(keyInfo.Key) 
                            {
                                case ConsoleKey.UpArrow: 
                                    Console.Clear();
                                    Maze.MoveDemaciaChampion(ConsoleKey.UpArrow); //ejecuta el método de mov. para demacia en MazeGenerator
                                    demaciaChampion.positionIndex = Maze.demaciaPosition; //actualiza la nueva posición
                                    if (Maze.isValid)  demaciaChampion.speed--; //si el mov. fue válido, eliminar un punto de velocidad
                                    if (Maze.trap) //si se activa una trampa
                                    {
                                        int randomIndex = random.Next(traps.Count); //random para decidir efecto de la trampa
                                        if (traps[randomIndex] == 0) //trampa de teletransportar a posición inicial
                                        {
                                            Maze.TeleportTrapD(); //método en MazeGenerator
                                            demaciaChampion.positionIndex = Maze.demaciaPosition; //actualizar posición
                                            demaciaChampion.speed = 0; //al activar trampas, la velocidad se pierde por el resto del turno
                                            Maze.trap = false; //se regresa la condición trampa a false para garantizar que se vuelva a activar con otras trampas
                                        }
                                        else if (traps[randomIndex] == 1) //trampa de stun 
                                        {
                                            RootCooldownD = 3; //contador de stuneo por tres turnos
                                            demaciaChampion.speed = 0; 
                                            Maze.trap = false;
                                            demaciaChampion.Cursed = true; //efecto maldición
                                        }
                                        else if (traps[randomIndex] == 2) //trampa de perder fruta 
                                        {
                                            demaciaChampion.speed = 0;
                                            Maze.trap = false;
                                            HoneyFruitsD--;
                                            Maze.GenerateHoneyFruits(); //al perder una fruta, esta se genera aleatoriamente en una casilla disponible del laberinto
                                        }
                                        else if ((traps[randomIndex] == 3)) //trampa de bloqueo de habilidades por dos turnos
                                        {
                                            demaciaChampion.speed = 0;
                                            Maze.trap = false;
                                            demaciaChampion.mainSkillAvailable = false; //hab. principal bloqueada 
                                            mainSkillCooldownD = 2; //contadores en 2 turnos
                                            demaciaChampion.secondarySkillAvailable = false; //hab. secundaria bloqueada
                                            secondaryCooldownD = 2;
                                        }
                                    }
                                    if (Maze.honeyFruit) //si la casilla poseía una fruta, se adiciona al contador del campeón
                                    {
                                        HoneyFruitsD++;
                                        Maze.honeyFruit = false; //se elimina la fruta de la casilla
                                    }
                                    break;
                                case ConsoleKey.DownArrow:
                                    Console.Clear();
                                    Maze.MoveDemaciaChampion(ConsoleKey.DownArrow); //Idem.
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
                                    Maze.MoveDemaciaChampion(ConsoleKey.LeftArrow); //Idem.
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
                                    Maze.MoveDemaciaChampion(ConsoleKey.RightArrow); //Idem.
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
                                    if (demaciaChampion.secondarySkillAvailable) Maze.RemoveObstacleD(); //hab. de remover obstáculo contiguo en MazeGenerator, se usa si está disponible
                                    if (Maze.obstacle) //si existía un obstáculo:
                                    {
                                        demaciaChampion.secondarySkillAvailable = false; //se activa el enfriamiento de la hab. secundaria
                                        secondaryCooldownD = 3;
                                        Maze.obstacle = false; //se reestablece el parámetro de obstáculo para futuro uso
                                        demaciaChampion.speed--; //la hab. consume un punto de velocidad del turno actual
                                    }
                                    break;
                                case ConsoleKey.P:
                                    Console.Clear();
                                    if (demaciaChampion.mainSkillAvailable) //si está disponible, se ejecuta la habilidad principal del campeón
                                    {
                                        switch (mainSkillD) //busca el valor de la hab. principal asignada anteriormente
                                        {
                                            case "Generate Random Trap": //generar trampa en lugar aleatorio disponible 
                                                Maze.GenerateTrap(); //método en MazeGenerator
                                                break;
                                            case "Generate Random Obstacle": //generar obstáculo en lugar aleatorio disponible
                                                Maze.GenerateObstacle(); //método en MazeGenerator
                                                break;
                                            case "Teleport Enemy To Random Address": //envía al enemigo a una casilla aleatoria disponible
                                                Maze.TeleportEnemy("Noxus"); //método de teletransporte de enemigo (noxus) en MazeGenerator 
                                                noxusChampion.positionIndex = Maze.noxusPosition; //actualizar posición de noxus
                                                break;
                                            case "Teleport Oneself To Random Address": //envía al propio campeón a una casilla aleatoria disponible
                                                Maze.TeleportSelf("Demacia"); //método de teletransporte propio (demacia) en MazeGenerator
                                                demaciaChampion.positionIndex = Maze.demaciaPosition; //actualizar posición de Demacia
                                                break;
                                            case "Root Enemy": //stun al enemigo por dos turnos
                                                RootCooldownN = 2;
                                                break;
                                            case "Steal Speed": //roba velocidad del enemigo para el próximo turno (1 punto para el enemigo y 3 puntos propios)
                                                speedSN = 2;
                                                noxusChampion.SpeedCooldown(speedSN); //se actualiza la vel. enemiga
                                                speedSD = 2;
                                                break;
                                        }
                                        mainSkillCooldownD = 3; //enfriamiento de la hab. principal por tres turnos
                                        demaciaChampion.mainSkillAvailable = false; //hab. principal no disponible
                                        demaciaChampion.speed--; //la hab. cuesta un punto de velocidad
                                    }
                                break;
                                    
                            }   
                        }
                        else //si no existen puntos de velocidad o si se presiona enter, se ejecuta el else 
                        {
                            speedSD++; 
                            demaciaChampion.SpeedCooldown(speedSD); //se calcula la velocidad para el próximo turno, método en ChampionClass
                            if (RootCooldownD != 0) RootCooldownD--; //se disminuye cada enfriamiento (si existe) a velocidad de un punto por turno
                            if (secondaryCooldownD != 0) secondaryCooldownD--;
                            if (mainSkillCooldownD != 0) mainSkillCooldownD--;
                            Turn = false; //finaliza turno de demacia
                        }
                    }

                        
                    else if (!Turn) //turno de noxus
                    {
                        Console.Clear();

                        var panelN = new Panel(new Markup($" HoneyFruits: {HoneyFruitsN}"))
                        {
                            Border = BoxBorder.Square
                        };
                        AnsiConsole.Render(panelN);

                        var tableN = new Table(); //Idem

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
                            switch (keyInfo.Key) //Idem.
                            {
                                case ConsoleKey.UpArrow:
                                    Console.Clear();
                                    Maze.MoveNoxusChampion(ConsoleKey.UpArrow); //Idem. método de MazeGenerator
                                    noxusChampion.positionIndex = Maze.noxusPosition;
                                    if (Maze.isValid) noxusChampion.speed--;
                                    if (Maze.trap)
                                    {
                                        int randomIndex = random.Next(traps.Count);
                                        if (traps[randomIndex] == 0) //trampa de teletransporte
                                        {
                                            Maze.TeleportTrapN(); //Idem. método de MazeGenerator
                                            noxusChampion.positionIndex = Maze.noxusPosition;
                                            noxusChampion.speed = 0;
                                            Maze.trap = false;
                                        }
                                        else if (traps[randomIndex] == 1) //trampa de stun
                                        {
                                            RootCooldownN = 3;
                                            noxusChampion.speed = 0;
                                            Maze.trap = false;
                                            noxusChampion.Cursed = true;
                                        }
                                        else if (traps[randomIndex] == 2) //trampa de perder fruta
                                        {
                                            noxusChampion.speed = 0;
                                            Maze.trap = false;
                                            HoneyFruitsN--;
                                            Maze.GenerateHoneyFruits();
                                        }
                                        else if ((traps[randomIndex] == 3)) //trampa de bloquear hab. por dos turnos
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
                                    if (noxusChampion.secondarySkillAvailable) Maze.RemoveObstacleN(); //Idem. método en MazeGenerator
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
                                    if (noxusChampion.mainSkillAvailable) //Idem.
                                    {
                                        switch (mainSkillN)
                                        {
                                            case "Generate Random Trap": //hab. de generar trampa aleatoria
                                                Maze.GenerateTrap(); //método en MazeGenerator
                                                break;
                                            case "Generate Random Obstacle": //hab. de generar obstáculo aleatorio
                                                Maze.GenerateObstacle(); //método en MazeGenerator
                                                break;
                                            case "Teleport Enemy To Random Address": //hab. de teletransporte enemigo aleatorio
                                                Maze.TeleportEnemy("Demacia"); //método en MazeGenerator
                                                demaciaChampion.positionIndex = Maze.demaciaPosition; //actualizar posición del enemigo
                                                break;
                                            case "Teleport Oneself To Random Address": //hab. de teletransporte propio aleatorio
                                                Maze.TeleportSelf("Noxus"); //método en MazeGenerator
                                                noxusChampion.positionIndex = Maze.noxusPosition; //actualizar posición propia
                                                break;
                                            case "Root Enemy": //hab. de stun
                                                RootCooldownD = 2;
                                                break;
                                            case "Steal Speed": //hab. de robar velocidad (1 punto enemigo, 3 puntos propios para el próximo turno)
                                                speedSD = 2;
                                                demaciaChampion.SpeedCooldown(speedSD); //método en ChampionClass
                                                speedSN = 2;
                                                break;
                                        }
                                        mainSkillCooldownN = 3; //enfriamiento de la hab. principal
                                        noxusChampion.mainSkillAvailable = false; //hab. principal no disponible
                                        noxusChampion.speed--; //-1 punto de velocidad
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            speedSN++;
                            noxusChampion.SpeedCooldown(speedSN); //se calcula la velocidad para el próximo turno, método en ChampionClass
                            if (RootCooldownN != 0) RootCooldownN--; //enfriamiento de stun y habilidades
                            if (secondaryCooldownN != 0) secondaryCooldownN--;
                            if (mainSkillCooldownN != 0) mainSkillCooldownN--;
                            Turn = true; //cambia el turno a demacia
                        }
                    }
                }
            }
            gameRunning = false; //finalizan todos los ciclos while
            running = false; 
        }

        Console.Clear();

        if (demaciaWin) //victoria de demacia: imprime cartel de victoria
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
        else if (noxusWin) //victoria de noxus: imprime cartel de victoria
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