using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Spectre.Console;

namespace Maze_of_Legends.Classes
{
    internal class ChampionClass
    {
        public string name { get; set; }
        public string mainSkill { get; set; }
        public bool mainSkillAvailable { get; set; }
        public bool secondarySkillAvailable { get; set; }
        public int speed { get; set; }
        public bool Cursed { get; set; }
        public (int x, int y) positionIndex { get; set; }

        public ChampionClass(string name, string mainSkill, (int x, int y) positionIndex)
        {
            this.name = name;
            this.mainSkill = mainSkill;
            this.mainSkillAvailable = true;
            this.secondarySkillAvailable = true;
            this.speed = 3;    
            this.Cursed = false;
            this.positionIndex = positionIndex;
        }

        public override string ToString()
        {
            return $"Name: {name}\n" +
                   $"Main Skill: {mainSkill}\n" +
                   $"Main Skill Available: {mainSkillAvailable}\n" +
                   $"Secondary Skill Available: {secondarySkillAvailable}\n" +
                   $"Speed: {speed}\n" +  
                   $"Cursed: {Cursed}\n" +
                   $"Position: {positionIndex}";
        }


        public void PrintInfo()
        {
            var table = new Table();

            table.AddColumn("Property");
            table.AddColumn("Value");

            table.AddRow("Name", name);
            table.AddRow("Main Skill", mainSkill);
            table.AddRow("Main Skill Available", mainSkillAvailable.ToString());
            table.AddRow("Secondary Skill Available", secondarySkillAvailable.ToString());
            table.AddRow("Speed", speed.ToString());
            table.AddRow("Cursed", Cursed.ToString());
            table.AddRow("Position", positionIndex.ToString());

            AnsiConsole.Write(table);
        }

        public void SpeedCooldown(int i)
        {
            if (( i + 3) % 3 == 0)
            {
                speed = 3;
            }
            else if ((i + 3) % 3 == 1)
            {
                speed = 2;
            }
            else if((i + 3) % 3 == 2)
            {
                speed = 1;
            }
        }
    }
}
