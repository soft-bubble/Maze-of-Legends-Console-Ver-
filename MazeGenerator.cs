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
        public int Size; //tamaño del laberinto

        public (int x, int y) demaciaPosition; //posiciones actualizables de los campeones
        public (int x, int y) noxusPosition;

        public (int x, int y) firstDemaciaPosition; //posiciones iniciales de los campeones
        public (int x, int y) firstNoxusPosition;

        public bool isValid; //bool para determinar si el mov. es válido
        public bool obstacle; //bool para determinar si es obstáculo
        public bool trap; //bool para determinar si es trampa
        public bool honeyFruit; //bool para determinar si es fruta

        public List<SquareClass> Squares = new List<SquareClass>(); //lista de las casillas
        public List<(int x, int y)> Positions = new List<(int x, int y)>(); //lista de las posiciones que se le asiganrán a las casillas

        private Random random = new Random(); //random para los metodos

        public MazeGenerator(int Size) //el cosntructor para generar el laberinto
        {
            this.Size = Size; //tamaño asignado según dificultad
            SquaresGeneration(); //se generan las casillas con su posición determinada e Id
            GenerateStructure(1, 1); //llamada de método para generar caminos desde la posicón inicial fija (1,1)
            ChangeType(); //llamada de método para agregar los otros tipos de casilla a las posiciones de camino
        }

        public (int x, int y) getDemaciaPosition() //getter de la posición de demacia
        {
            return (demaciaPosition);
        }
        public (int x, int y) getNoxusPosition() //getter de la posición de noxus
        {
            return (noxusPosition);
        }

        private void SquaresGeneration() //método para generar los objetos tipo Square y añadirlos a la lista del laberinto
        {
            for (int i = 0; i < Size * Size; i++) 
            {
                int x = i % Size;
                int y = i / Size;

                Squares.Add(new SquareClass(i, (x, y)));

                Positions.Add((x, y));
            }
        }
        
        private void GenerateStructure(int x, int y) //generador del laberinto
        {
            var directions = new List<(int directionX, int directionY)> //lista de direcciones
            {
                (0, -2),    //arriba
                (0, 2),     //abajo
                (2, 0),     //izquierda
                (-2, 0)     //derecha
            };

            MixDirections(directions); //llamada de método que mezcla las direcciones para la generación aleatoria 

            foreach (var (directionX, directionY) in directions) 
            {
                int newDirectionX = x + directionX; //nuevas direciones
                int newDirectionY = y + directionY;

                if (newDirectionX > 0 && newDirectionY > 0 && newDirectionX < Size && newDirectionY < Size) //asegurar que siga dentro del rango del laberinto
                {
                    int index = newDirectionX + newDirectionY * Size; //se busca la casilla square asociada a la posición
                    if (Squares[index].Type == SquareClass.CellType.Wall) //si es pared, cavar caminos
                    {
                        Squares[index].Type = SquareClass.CellType.Path;
                        int betweenIndex = (x + directionX / 2) + (y + directionY / 2) * Size;
                        Squares[betweenIndex].Type = SquareClass.CellType.Path;
                        GenerateStructure(newDirectionX, newDirectionY);    //llamada recursiva hasta que no sea pared, iniciando en la posición asignada
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
            var pathCells = Squares.Where(square => square.Type == SquareClass.CellType.Path).ToList(); //todos las casillas camino se agregan a la lista

            int traps = 4; //cantidades predeterminadas 
            int obstacles = 3;

            switch(Size) //cantidades por dificultad (tamaño)
            {
                case 9:
                    break;
                case 11:
                    break;
                case 13:
                    break;
                case 15:
                    break;
                case 17:
                    traps += 4;
                    obstacles += 3;
                break;
                case 19:
                    traps += 8;
                    obstacles += 6;
                break;
                case 21:
                    traps += 12;
                    obstacles += 9;
                break;
                case 23:
                    traps += 16;
                    obstacles += 12;
                break;
                case 25:
                    traps += 20;
                    obstacles += 15;
                break;
                case 27:
                    traps += 24;
                    obstacles += 18;
                break;

            }

            for (int i = 0; i < traps; i++) //generación de trampas
            {
                int randomIndex = random.Next(pathCells.Count);
                pathCells[randomIndex].Type = SquareClass.CellType.Trap;
                pathCells.RemoveAt(randomIndex);
            }
             
            for (int i = 0; i < obstacles; i++) //generación de obstáculos
            {
                int randomIndex = random.Next(pathCells.Count);
                pathCells[randomIndex].Type = SquareClass.CellType.Obstacle;
                pathCells.RemoveAt(randomIndex);
            }

            for (int i = 0; i < 5; i++) //generación de cantidad fija de frutas
            {
                int randomIndexHF = random.Next(pathCells.Count);
                pathCells[randomIndexHF].Type = SquareClass.CellType.HoneyFruit;
                pathCells.RemoveAt(randomIndexHF);
            }

            int randomIndexD = random.Next(pathCells.Count); 
            pathCells[randomIndexD].Type = SquareClass.CellType.DemaciaPlayer; //generación aleatoria del campeón
            demaciaPosition = pathCells[randomIndexD].Position; //se aguarda la posición actual e inicial
            firstDemaciaPosition = demaciaPosition;
            pathCells.RemoveAt(randomIndexD);

            int randomIndexN = random.Next(pathCells.Count);
            pathCells[randomIndexN].Type = SquareClass.CellType.NoxusPlayer;
            noxusPosition = pathCells[randomIndexN].Position;
            firstNoxusPosition = noxusPosition;
            pathCells.RemoveAt(randomIndexN);

        }

        public void PrintMaze() //imprimir el laberinto desde la lista
        {
            for (int y = 0; y < Size; y++)
            {
                for (int x = 0; x < Size; x++)
                {
                    int index = x + y * Size; //se calcula el index referente a la lista
                    switch (Squares[index].Type) //asignar los emojis según el enum
                    {
                        case SquareClass.CellType.Wall:
                            Console.Write(Emoji.Known.BlueSquare); 
                            break;
                        case SquareClass.CellType.Path:
                            Console.Write(Emoji.Known.BlackCircle); 
                            break;
                        case SquareClass.CellType.Trap:
                            Console.Write(Emoji.Known.Cyclone); 
                            break;
                        case SquareClass.CellType.Obstacle:
                            Console.Write(Emoji.Known.ChequeredFlag); 
                            break;
                        case SquareClass.CellType.DemaciaPlayer:
                            Console.Write(Emoji.Known.DimButton);
                            break;
                        case SquareClass.CellType.NoxusPlayer:
                            Console.Write(Emoji.Known.CrescentMoon);
                            break;
                        case SquareClass.CellType.HoneyFruit:
                            Console.Write(Emoji.Known.Peach);
                            break;
                    }
                }
                Console.WriteLine();    //garantiza el salto de una fila a otra al llegar al límite de columnas 
            }
        }

        public void MoveDemaciaChampion(ConsoleKey key) //método para mover campeón de demacia
        {
            (int x, int y) currentPosition = getDemaciaPosition(); //se obtiene la posición previa al cambio
            int newX = currentPosition.x; //se almacenan los valores x e y
            int newY = currentPosition.y;

            switch (key) //en dependencia de la tecla presionada, se efectúa un cambio de coordenada
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

            if (newX >= 0 && newY >= 0 && newX < Size && newY < Size) //si no sale del rango del laberinto
            {
                int index = newX + newY * Size; //calcular el index de la casilla que se desea visitar en la lista 
                if (Squares[index].Type == SquareClass.CellType.Path) //si es camino:
                {
                    Squares[currentPosition.x + currentPosition.y * Size].Type = SquareClass.CellType.Path; //cambiar el tipo de la casilla anterior a camino
                    Squares[index].Type = SquareClass.CellType.DemaciaPlayer; //cambiar el tipo de la casilla actual al jugador
                    demaciaPosition = (newX, newY); //asignar posición actualizada
                    isValid = true; //booleano necesario para garantizar que se pierdan puntos de velocidad al moverse
                }
                else if (Squares[index].Type == SquareClass.CellType.Trap) //si es trampa:
                {
                    Squares[currentPosition.x + currentPosition.y * Size].Type = SquareClass.CellType.Path;
                    Squares[index].Type = SquareClass.CellType.DemaciaPlayer;
                    demaciaPosition = (newX, newY);
                    isValid = true;
                    trap = true; //booleano para activar el efecto trampa
                }
                else if (Squares[index].Type == SquareClass.CellType.HoneyFruit) //si es fruta:
                {
                    Squares[currentPosition.x + currentPosition.y * Size].Type = SquareClass.CellType.Path;
                    Squares[index].Type = SquareClass.CellType.DemaciaPlayer;
                    demaciaPosition = (newX, newY);
                    isValid = true;
                    honeyFruit = true; //booleano para aumentar la cantidad de frutas en posesión
                }
                else if (Squares[index].Type == SquareClass.CellType.NoxusPlayer) //si es otro campeón:
                {
                    Squares[currentPosition.x + currentPosition.y * Size].Type = SquareClass.CellType.NoxusPlayer; //se retira el campeón enemigo de la casilla visitada a la casilla anteriormente utilizada por el campeón propio
                    noxusPosition = (currentPosition.x, currentPosition.y); //actualizar ambas posiciones
                    Squares[index].Type = SquareClass.CellType.DemaciaPlayer;
                    demaciaPosition = (newX, newY);
                    isValid = true;
                }
                else if (Squares[index].Type == SquareClass.CellType.Wall) //si es pared u obstáculo, no es válido el movimiento
                {
                    isValid = false;
                }
                else if (Squares[index].Type == SquareClass.CellType.Obstacle)
                {
                    isValid = false;
                }
            }
        }

        public void MoveNoxusChampion(ConsoleKey key) //mover campeón de demacia, misma lógica.
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
                else if (Squares[index].Type == SquareClass.CellType.DemaciaPlayer)
                {
                    Squares[currentPosition.x + currentPosition.y * Size].Type = SquareClass.CellType.DemaciaPlayer;
                    demaciaPosition = (currentPosition.x, currentPosition.y);
                    Squares[index].Type = SquareClass.CellType.NoxusPlayer;
                    noxusPosition = (newX, newY);
                    isValid = true;
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

        public void RemoveObstacleD() //método para remover obstáculo contiguo del campeón de demacia
        {
            (int x, int y) currentPosition = getDemaciaPosition(); //se obtiene la posición
            int nextX = currentPosition.x;
            int nextY = currentPosition.y;

            int up = nextX + (nextY - 1) * Size; //valores de casillas contiguas
            int down = nextX + (nextY + 1) * Size;
            int left = nextX - 1 + nextY * Size;
            int right = nextX + 1 + nextY * Size;

            if (Squares[up].Type == SquareClass.CellType.Obstacle && !obstacle) //verificación en orden para eliminar un obstáculo en la dirección definida
            {
                obstacle = true; //garantiza que solo si hay obstáculo se descuente un punto de velocidad
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

            PrintMaze(); //volver a imprimir el laberinto con el cambio
        }

        public void RemoveObstacleN() //método para remover obstáculo del campeón de noxus. misma lógica.
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
        
        public void TeleportTrapD() //trampa de teletransporte a la casilla inicial para el campeón de demacia
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

        //SKILLS:

        public void GenerateTrap()
        {
            var pathCells = Squares.Where(square => square.Type == SquareClass.CellType.Path).ToList();
            int randomIndex = random.Next(pathCells.Count);
            pathCells[randomIndex].Type = SquareClass.CellType.Trap;
        }

        public void GenerateObstacle()
        {
            var pathCells = Squares.Where(square => square.Type == SquareClass.CellType.Path).ToList();
            int randomIndex = random.Next(pathCells.Count);
            pathCells[randomIndex].Type = SquareClass.CellType.Obstacle;
        }

        public void TeleportEnemy(string enemy)
        {
            var pathCells = Squares.Where(square => square.Type == SquareClass.CellType.Path).ToList();
            int randomIndex = random.Next(pathCells.Count);

            if (enemy == "Noxus")
            {
                (int x, int y) currentPosition = getNoxusPosition();
                int indexOfCurrentPosition = currentPosition.x + currentPosition.y * Size;

                Squares[indexOfCurrentPosition].Type = SquareClass.CellType.Path;
                pathCells[randomIndex].Type = SquareClass.CellType.NoxusPlayer;
                noxusPosition = pathCells[randomIndex].Position;
            }
            else if (enemy == "Demacia")
            {
                (int x, int y) currentPosition = getDemaciaPosition();
                int indexOfCurrentPosition = currentPosition.x + currentPosition.y * Size;

                Squares[indexOfCurrentPosition].Type = SquareClass.CellType.Path;
                pathCells[randomIndex].Type = SquareClass.CellType.DemaciaPlayer;
                demaciaPosition = pathCells[randomIndex].Position;
            }

        }

        public void TeleportSelf(string self)
        {
            var pathCells = Squares.Where(square => square.Type == SquareClass.CellType.Path).ToList();
            int randomIndex = random.Next(pathCells.Count);

            if (self == "Noxus")
            {
                (int x, int y) currentPosition = getNoxusPosition();
                int indexOfCurrentPosition = currentPosition.x + currentPosition.y * Size;

                Squares[indexOfCurrentPosition].Type = SquareClass.CellType.Path;
                pathCells[randomIndex].Type = SquareClass.CellType.NoxusPlayer;
                noxusPosition = pathCells[randomIndex].Position;
            }
            else if (self == "Demacia")
            {
                (int x, int y) currentPosition = getDemaciaPosition();
                int indexOfCurrentPosition = currentPosition.x + currentPosition.y * Size;

                Squares[indexOfCurrentPosition].Type = SquareClass.CellType.Path;
                pathCells[randomIndex].Type = SquareClass.CellType.DemaciaPlayer;
                demaciaPosition = pathCells[randomIndex].Position;
            }
        }
    }
}
