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
        public string name { get; set; }
        public string mainSkill { get; set; }
        public bool mainSkillAvailable { get; set; }
        public bool secondarySkillAvailable { get; set; }
        public int speed { get; set; }
        public bool skillCurse { get; set; }
        public bool trapCurse { get; set; }
        public (int x, int y) positionIndex { get; set; }

        public ChampionClass(string name, string mainSkill, (int x, int y) positionIndex)
        {
            this.name = name;
            this.mainSkill = mainSkill;
            this.mainSkillAvailable = true;
            this.secondarySkillAvailable = true;
            this.speed = 3;
            this.skillCurse = false;
            this.trapCurse = false;
            this.positionIndex = positionIndex;
        }

        public override string ToString()
        {
            return $"Name: {name}\n" +
                   $"Main Skill: {mainSkill}\n" +
                   $"Main Skill Available: {mainSkillAvailable}\n" +
                   $"Secondary Skill Available: {secondarySkillAvailable}\n" +
                   $"Speed: {speed}\n" +
                   $"Skill Curse:{skillCurse}\n" +
                   $"Trap Curse: {trapCurse}\n" +
                   $"Position: {positionIndex}";
        }

        
    }
}
