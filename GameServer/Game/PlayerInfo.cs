using Common;
using Space.Game.Resources;
using System;
using System.Collections;

namespace Space.Game
{
    /*
    public class PlayerInfo : IInfo
    {
        private int exp;
        private int level;
        private string group;
        private Workshop homeWorkshop;
        private string name;
        private Race race;
        private readonly Leveling leveling;

        public PlayerInfo(Workshop homeWorkshop, int level, int exp, string name, Race race, string group, Leveling leveling)
        {
            this.leveling = leveling;
            this.Modify(homeWorkshop, level, exp, name, race, group);
        }

        public void Modify(Workshop homeWorkshop, int level, int exp, string name, Race race, string group)
        {
            this.homeWorkshop = homeWorkshop;
            this.level = level;
            this.exp = exp;
            this.name = name;
            this.race = race;
            this.group = group;
        }

        public int Level {
            get {
                return this.level;
            }
        }

        public string Group {
            get {
                return this.group;
            }
        }

        public int Exp
        {
            get
            {
                return this.exp;
            }
        }

        public Workshop HomeWorkshop
        {
            get
            {
                return this.homeWorkshop;
            }
        }

        public string Name
        {
            get { return this.name; }
        }

        public Race Race
        {
            get
            {
                return this.race;
            }
        }

        public void AddExp(int add)
        {
            this.exp += add;
            this.level = leveling.LevelForExp(this.exp);
        }

        public void SetExp(int exp)
        {
            this.exp = exp;
            this.level = this.leveling.LevelForExp(this.exp);
        }



        public void SetHomeWorkshop(int workshop)
        {
            this.homeWorkshop = (Workshop)(byte)workshop;
        }

        public void SetGroup(string group)
        {
            this.group = group;
        }

        public void SetName(string name)
        {
            this.name = name;
        }

        public void UnsetGroup()
        {
            this.group = string.Empty;
        }



        public Hashtable GetInfo()
        {
            Hashtable info = new Hashtable{
                {GenericEventProps.exp, this.exp},
                {GenericEventProps.level, this.level},
                {GenericEventProps.group, this.group},
                {GenericEventProps.workshop, (int)this.homeWorkshop.toByte()},
                {GenericEventProps.name, this.name },
                {GenericEventProps.race, (int)this.race.toByte()}
            };
            return info;
        }

        public void ParseInfo(Hashtable info)
        {
            this.level = info.GetValue<int>(GenericEventProps.level, 1);
            this.exp = info.GetValue<int>(GenericEventProps.exp, 0);
            this.group = info.GetValue<string>(GenericEventProps.group, string.Empty);
            this.homeWorkshop = (Workshop)(byte)info.GetValue<int>(GenericEventProps.workshop, (int)Workshop.DarthTribe.toByte());
            this.name = info.GetValue<string>(GenericEventProps.name, string.Empty);
            this.race = (Race)(byte)info.GetValue<int>(GenericEventProps.race, 0);
        }

        public void Replace(PlayerInfo other)
        {
            this.level = other.level;
            this.exp = other.exp;
            this.group = other.group;
            this.homeWorkshop = other.homeWorkshop;
            this.name = other.name;
            this.race = other.race;
        }


    }*/
}
