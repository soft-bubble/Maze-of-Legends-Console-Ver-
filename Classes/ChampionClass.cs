using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Maze_of_Legends.Classes
{
    internal class ChampionClass
    {
        public string mainSkill { get; set; }
        public bool mainSkillAvailable { get; set; }
        public bool secondarySkillAvailable { get; set; }
        public int speed { get; set; }
        public bool skillCurse { get; set; }
        public bool trapCurse { get; set; }
        public int positionIndex { get; set; }

        public ChampionClass(string mainSkill, int positionIndex)
        {
            this.mainSkill = mainSkill;
            this.mainSkillAvailable = true;
            this.secondarySkillAvailable = true;
            this.speed = 3;
            this.skillCurse = false;
            this.trapCurse = false;
            this.positionIndex = positionIndex;
        }
    }
}
