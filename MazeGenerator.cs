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
                int x = i / Size;
                int y = i % Size;
                Squares.Add(new SquareClass(i, x, y));

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
                    int index = newDirectionX * Size + newDirectionY;
                    if (Squares[index].Type == SquareClass.CellType.Wall)
                    {
                        Squares[index].Type = SquareClass.CellType.Path;
                        int betweenIndex = (x + directionX / 2) * Size + (y + directionY / 2);
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

           
            int randomIndexD = random.Next(pathCells.Count);
            pathCells[randomIndexD].Type = SquareClass.CellType.DemaciaPlayer;
            demaciaPosition = pathCells[randomIndexD].Position;
            pathCells.RemoveAt(randomIndexD);

            int randomIndexN = random.Next(pathCells.Count);
            pathCells[randomIndexN].Type = SquareClass.CellType.NoxusPlayer;
            noxusPosition = pathCells[randomIndexN].Position;
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
                    }
                }
                Console.WriteLine();    //nueva línea al final de cada fila
            }
        }

        public void UpdateMaze()
        {

        }
    }
}
