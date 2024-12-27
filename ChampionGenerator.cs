using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Maze_of_Legends.Classes;

namespace Maze_of_Legends
{
    internal class ChampionGenerator
    {
        public ChampionGenerator(string champion) 
        {
            
        }

        static ChampionClass Garen = new ChampionClass("ja", 1);
        static ChampionClass Lux = new ChampionClass("ja", 1);
        static ChampionClass Sona = new ChampionClass("ja", 1);
        static ChampionClass Vayne = new ChampionClass("ja", 1);
        static ChampionClass Shyvanna = new ChampionClass("ja", 1);

        static ChampionClass Ambessa = new ChampionClass("ja", 1);
        static ChampionClass Swain = new ChampionClass("ja", 1);
        static ChampionClass Mordekaiser = new ChampionClass("ja", 1);
        static ChampionClass Katarina = new ChampionClass("ja", 1);
        static ChampionClass Samira = new ChampionClass("ja", 1);

        public List<ChampionClass> DemaciaChamps = new List<ChampionClass>
        {
            Garen,
            Lux,
            Sona,
            Vayne,
            Shyvanna
        };
    }
}
