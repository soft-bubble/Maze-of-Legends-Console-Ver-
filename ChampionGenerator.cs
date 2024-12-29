using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Maze_of_Legends.Classes;
using Spectre.Console;

namespace Maze_of_Legends
{
    internal class ChampionGenerator
    {
        string name;
        (int x, int y) position;
        ChampionClass champion;

        public ChampionGenerator(string name, (int x, int y) position) 
        {
           this.name = name;
           this.position = position;

        }

        public void SelectGeneratedChampion()
        {
                champion = new ChampionClass(name, "Habilidad", position);
        }

        public string DisplayStatus()
        {
            return champion.ToString();
        }
    }
}
