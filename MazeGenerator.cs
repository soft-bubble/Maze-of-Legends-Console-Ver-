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

        public List<SquareClass> Squares = new List<SquareClass>();

        private Random random = new Random();

        public MazeGenerator() //el generador vv
        {
            SquaresGeneration();
            GenerateStructure(1, 1);
            ChangeType();
        }

        private void SquaresGeneration()
        {
            for (int i = 0; i < Size * Size; i++) //creamos 225 cuadrados 
            {
                Squares.Add(new SquareClass(i));
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
                            Console.Write(Emoji.Known.Star);
                            break;
                        case SquareClass.CellType.NoxusPlayer:
                            Console.Write(Emoji.Known.MoneyBag);
                            break;
                    }
                }
                Console.WriteLine();    //nuueva línea al final de cada fila
            }
        }
    }
}
