using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Maze_of_Legends.Classes
{
    internal class SquareClass
    {
        public int Id {  get; set; }
        public (int x, int y) Position { get; set; }
        public CellType Type {  get; set; }
        public bool OnUse {  get; set; }
 
        public SquareClass(int i, (int x, int y) position)
        {
            Id = i;
            Type = CellType.Wall;
            OnUse = false;
            Position = position;
        }

        public enum CellType
        {
            Path = 0,      
            Wall = 1,          
            Trap = 2,      
            Obstacle = 3,  
            Exit = 4,
            DemaciaPlayer = 5,
            NoxusPlayer = 6,
            HoneyFruit = 7,
        }
    }
}
