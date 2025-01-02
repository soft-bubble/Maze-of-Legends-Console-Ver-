using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Maze_of_Legends.Classes;
using Spectre.Console;

namespace Maze_of_Legends
{
    internal class MazeGenerator
    {
        public int Size = 15;

        public (int x, int y) demaciaPosition;
        public (int x, int y) noxusPosition;

        public (int x, int y) firstDemaciaPosition;
        public (int x, int y) firstNoxusPosition;

        public bool isValid;
        public bool obstacle;
        public bool trap;
        public bool honeyFruit;

        public List<SquareClass> Squares = new List<SquareClass>();
        public List<(int x, int y)> Positions = new List<(int x, int y)>();

        private Random random = new Random();

        public MazeGenerator() //el generador vv
        {
            SquaresGeneration();
            GenerateStructure(1, 1);
            ChangeType();
        }

        public (int x, int y) getDemaciaPosition()
        {
            return (demaciaPosition);
        }
        public (int x, int y) getNoxusPosition()
        {
            return (noxusPosition);
        }

        private void SquaresGeneration()
        {
            for (int i = 0; i < Size * Size; i++) //creamos 225 cuadrados 
            {
                int x = i % Size;
                int y = i / Size;

                Squares.Add(new SquareClass(i, (x, y)));

                Positions.Add((x, y));
            }
        }
        
        private void GenerateStructure(int x, int y)
        {
            var directions = new List<(int directionX, int directionY)>
            {
                (0, -2),    //arriba
                (0, 2),     //abajo
                (2, 0),     //izquierda
                (-2, 0)     //derecha
            };

            MixDirections(directions); //lamezcla

            foreach (var (directionX, directionY) in directions)
            {
                int newDirectionX = x + directionX;
                int newDirectionY = y + directionY;

                if (newDirectionX > 0 && newDirectionY > 0 && newDirectionX < Size && newDirectionY < Size)
                {
                    int index = newDirectionX + newDirectionY * Size;
                    if (Squares[index].Type == SquareClass.CellType.Wall)
                    {
                        Squares[index].Type = SquareClass.CellType.Path;
                        int betweenIndex = (x + directionX / 2) + (y + directionY / 2) * Size;
                        Squares[betweenIndex].Type = SquareClass.CellType.Path;
                        GenerateStructure(newDirectionX, newDirectionY);    //recursividad vv
                    }
                }
            }
        }

        private void MixDirections(List<(int directionX, int directionY)> directions) //método para mixear las direcciones 
        {
            for (int i = directions.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (directions[i], directions[j]) = (directions[j], directions[i]);
            }
        }

        private void ChangeType() //generar las trampas y obstáculos
        {
            var pathCells = Squares.Where(square => square.Type == SquareClass.CellType.Path).ToList();

            for (int i = 0; i < 3; i++)
            {
                int randomIndex = random.Next(pathCells.Count);
                pathCells[randomIndex].Type = SquareClass.CellType.Trap;
                pathCells.RemoveAt(randomIndex);
            }

            for (int i = 0; i < 3; i++)
            {
                int randomIndex = random.Next(pathCells.Count);
                pathCells[randomIndex].Type = SquareClass.CellType.Obstacle;
                pathCells.RemoveAt(randomIndex);
            }

            for (int i = 0; i < 5; i++)
            {
                int randomIndexHF = random.Next(pathCells.Count);
                pathCells[randomIndexHF].Type = SquareClass.CellType.HoneyFruit;
                pathCells.RemoveAt(randomIndexHF);
            }

            int randomIndexD = random.Next(pathCells.Count);
            pathCells[randomIndexD].Type = SquareClass.CellType.DemaciaPlayer;
            demaciaPosition = pathCells[randomIndexD].Position;
            firstDemaciaPosition = demaciaPosition;
            pathCells.RemoveAt(randomIndexD);

            int randomIndexN = random.Next(pathCells.Count);
            pathCells[randomIndexN].Type = SquareClass.CellType.NoxusPlayer;
            noxusPosition = pathCells[randomIndexN].Position;
            firstNoxusPosition = noxusPosition;
            pathCells.RemoveAt(randomIndexN);

        }

        public void PrintMaze() //imprimir el coso
        {
            for (int y = 0; y < Size; y++)
            {
                for (int x = 0; x < Size; x++)
                {
                    int index = x + y * Size;
                    switch (Squares[index].Type)
                    {
                        case SquareClass.CellType.Wall:
                            Console.Write(Emoji.Known.BlueSquare); 
                            break;
                        case SquareClass.CellType.Path:
                            Console.Write(Emoji.Known.BlackCircle); 
                            break;
                        case SquareClass.CellType.Trap:
                            Console.Write(Emoji.Known.Fire); 
                            break;
                        case SquareClass.CellType.Obstacle:
                            Console.Write(Emoji.Known.ChequeredFlag); 
                            break;
                        case SquareClass.CellType.Exit:
                            Console.Write(Emoji.Known.CrossMarkButton); 
                            break;
                        case SquareClass.CellType.DemaciaPlayer:
                            Console.Write(Emoji.Known.HighVoltage);
                            break;
                        case SquareClass.CellType.NoxusPlayer:
                            Console.Write(Emoji.Known.MoneyBag);
                            break;
                        case SquareClass.CellType.HoneyFruit:
                            Console.Write(Emoji.Known.Peach);
                            break;
                    }
                }
                Console.WriteLine();    //nueva línea al final de cada fila
            }
        }

        public void MoveDemaciaChampion(ConsoleKey key) //mover campeón de demacia
        {
            (int x, int y) currentPosition = getDemaciaPosition();
            int newX = currentPosition.x;
            int newY = currentPosition.y;

            switch (key)
            {
                case ConsoleKey.UpArrow:
                    newY -= 1;
                    break;
                case ConsoleKey.DownArrow:
                    newY += 1;
                    break;
                case ConsoleKey.LeftArrow:
                    newX -= 1;
                    break;
                case ConsoleKey.RightArrow:
                    newX += 1;
                    break;
            }

            if (newX >= 0 && newY >= 0 && newX < Size && newY < Size)
            {
                int index = newX + newY * Size;
                if (Squares[index].Type == SquareClass.CellType.Path)
                {
                    Squares[currentPosition.x + currentPosition.y * Size].Type = SquareClass.CellType.Path;
                    Squares[index].Type = SquareClass.CellType.DemaciaPlayer;
                    demaciaPosition = (newX, newY);
                    isValid = true;
                }
                else if (Squares[index].Type == SquareClass.CellType.Trap)
                {
                    Squares[currentPosition.x + currentPosition.y * Size].Type = SquareClass.CellType.Path;
                    Squares[index].Type = SquareClass.CellType.DemaciaPlayer;
                    demaciaPosition = (newX, newY);
                    isValid = true;
                    trap = true;
                }
                else if (Squares[index].Type == SquareClass.CellType.HoneyFruit)
                {
                    Squares[currentPosition.x + currentPosition.y * Size].Type = SquareClass.CellType.Path;
                    Squares[index].Type = SquareClass.CellType.DemaciaPlayer;
                    demaciaPosition = (newX, newY);
                    isValid = true;
                    honeyFruit = true;
                }
                else if (Squares[index].Type == SquareClass.CellType.Wall)
                {
                    isValid = false;
                }
                else if (Squares[index].Type == SquareClass.CellType.Obstacle)
                {
                    isValid = false;
                }
            }
        }

        public void MoveNoxusChampion(ConsoleKey key) //mover campeón de demacia
        {
            (int x, int y) currentPosition = getNoxusPosition();
            int newX = currentPosition.x;
            int newY = currentPosition.y;

            switch (key)
            {
                case ConsoleKey.UpArrow:
                    newY -= 1;
                    break;
                case ConsoleKey.DownArrow:
                    newY += 1;
                    break;
                case ConsoleKey.LeftArrow:
                    newX -= 1;
                    break;
                case ConsoleKey.RightArrow:
                    newX += 1;
                    break;
            }

            if (newX >= 0 && newY >= 0 && newX < Size && newY < Size)
            {
                int index = newX + newY * Size;
                if (Squares[index].Type == SquareClass.CellType.Path)
                {
                    Squares[currentPosition.x + currentPosition.y * Size].Type = SquareClass.CellType.Path;
                    Squares[index].Type = SquareClass.CellType.NoxusPlayer;
                    noxusPosition = (newX, newY);
                    isValid = true;
                }
                else if (Squares[index].Type == SquareClass.CellType.Trap)
                {
                    Squares[currentPosition.x + currentPosition.y * Size].Type = SquareClass.CellType.Path;
                    Squares[index].Type = SquareClass.CellType.NoxusPlayer;
                    noxusPosition = (newX, newY);
                    isValid = true;
                    trap = true;
                }
                else if (Squares[index].Type == SquareClass.CellType.HoneyFruit)
                {
                    Squares[currentPosition.x + currentPosition.y * Size].Type = SquareClass.CellType.Path;
                    Squares[index].Type = SquareClass.CellType.NoxusPlayer;
                    noxusPosition = (newX, newY);
                    isValid = true;
                    honeyFruit = true;
                }
                else if (Squares[index].Type == SquareClass.CellType.Wall) 
                {
                    isValid = false;
                }
                else if (Squares[index].Type == SquareClass.CellType.Obstacle)
                {
                    isValid = false;
                }
            }
        }

        public void RemoveObstacleD()
        {
            (int x, int y) currentPosition = getDemaciaPosition();
            int nextX = currentPosition.x;
            int nextY = currentPosition.y;

            int up = nextX + (nextY - 1) * Size;
            int down = nextX + (nextY + 1) * Size;
            int left = nextX - 1 + nextY * Size;
            int right = nextX + 1 + nextY * Size;

            if (Squares[up].Type == SquareClass.CellType.Obstacle && !obstacle)
            {
                obstacle = true;
                Squares[up].Type = SquareClass.CellType.Path;
            }
            else if (Squares[down].Type == SquareClass.CellType.Obstacle && !obstacle)
            {
                obstacle = true;
                Squares[down].Type = SquareClass.CellType.Path;
            }
            else if (Squares[left].Type == SquareClass.CellType.Obstacle && !obstacle)
            {
                obstacle = true;
                Squares[left].Type = SquareClass.CellType.Path;
            }
            else if (Squares[right].Type == SquareClass.CellType.Obstacle && !obstacle)
            {
                obstacle = true;
                Squares[right].Type = SquareClass.CellType.Path;
            }

            PrintMaze();
        }

        public void RemoveObstacleN()
        {
            (int x, int y) currentPosition = getNoxusPosition();
            int nextX = currentPosition.x;
            int nextY = currentPosition.y;

            int up = nextX + (nextY - 1) * Size;
            int down = nextX + (nextY + 1) * Size;
            int left = nextX - 1 + nextY * Size;
            int right = nextX + 1 + nextY * Size;

            if (Squares[up].Type == SquareClass.CellType.Obstacle && !obstacle)
            {
                obstacle = true;
                Squares[up].Type = SquareClass.CellType.Path;
            }
            else if (Squares[down].Type == SquareClass.CellType.Obstacle && !obstacle)
            {
                obstacle = true;
                Squares[down].Type = SquareClass.CellType.Path;
            }
            else if (Squares[left].Type == SquareClass.CellType.Obstacle && !obstacle)
            {
                obstacle = true;
                Squares[left].Type = SquareClass.CellType.Path;
            }
            else if (Squares[right].Type == SquareClass.CellType.Obstacle && !obstacle)
            {
                obstacle = true;
                Squares[right].Type = SquareClass.CellType.Path;
            }

            PrintMaze();
        }


        //TRAPS:
        
        public void TeleportTrapD()
        {
            var pathCells = Squares.Where(square => square.Type == SquareClass.CellType.Path).ToList();

            (int x, int y) currentPosition = getDemaciaPosition();
            int indexOfCurrentPosition = currentPosition.x + currentPosition.y * Size;
            int indexOfFirstPosition = firstDemaciaPosition.x + firstDemaciaPosition.y * Size;

            if (Squares[indexOfFirstPosition].Type == SquareClass.CellType.Path)
            {
                Squares[indexOfCurrentPosition].Type = SquareClass.CellType.Path;
                Squares[indexOfFirstPosition].Type = SquareClass.CellType.DemaciaPlayer;
                demaciaPosition = (firstDemaciaPosition.x,  firstDemaciaPosition.y);
            }
            else
            {
                Squares[indexOfCurrentPosition].Type = SquareClass.CellType.Path;
                int randomIndex = random.Next(pathCells.Count);
                pathCells[randomIndex].Type = SquareClass.CellType.DemaciaPlayer;
                demaciaPosition = pathCells[randomIndex].Position;
            }
        }

        public void TeleportTrapN()
        {
            var pathCells = Squares.Where(square => square.Type == SquareClass.CellType.Path).ToList();

            (int x, int y) currentPosition = getNoxusPosition();
            int indexOfCurrentPosition = currentPosition.x + currentPosition.y * Size;
            int indexOfFirstPosition = firstNoxusPosition.x + firstNoxusPosition.y * Size;

            if (Squares[indexOfFirstPosition].Type == SquareClass.CellType.Path)
            {
                Squares[indexOfCurrentPosition].Type = SquareClass.CellType.Path;
                Squares[indexOfFirstPosition].Type = SquareClass.CellType.NoxusPlayer;
                noxusPosition = (firstNoxusPosition.x, firstNoxusPosition.y);
            }
            else
            {
                Squares[indexOfCurrentPosition].Type = SquareClass.CellType.Path;
                int randomIndex = random.Next(pathCells.Count);
                pathCells[randomIndex].Type = SquareClass.CellType.NoxusPlayer;
                noxusPosition = pathCells[randomIndex].Position;
            }
        }

        public void GenerateHoneyFruits()
        {
            var pathCells = Squares.Where(square => square.Type == SquareClass.CellType.Path).ToList();
            int randomIndex = random.Next(pathCells.Count);
            pathCells[randomIndex].Type = SquareClass.CellType.HoneyFruit;
        }
    }
}
