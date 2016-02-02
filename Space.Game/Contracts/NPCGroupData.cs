using Common;
using Space.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Nebula.Contracts {
    public class NPCGroupData {
        public string name { get; private set; }
        public List<string> zones { get; private set; }
        public int count { get; private set; }
        public int minLevel { get; private set; }
        public List<Race> races { get; private set; }

        public NPCGroupData(XElement element) {
            name = element.GetString("name");
            zones = element.GetString("zones").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            count = element.GetInt("count");
            minLevel = element.GetInt("min_level");
            races = element.GetString("race").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => (Race)Enum.Parse(typeof(Race), s)).ToList();
        }

        public bool anyZone {
            get {
                return (zones == null) || (zones.Count == 0);
            }
        }

        public bool anyRace {
            get {
                return (races == null) || (races.Count == 0);
            }
        }

        public string RandomZone() {
            if(anyZone) {
                return string.Empty;
            }
            return zones.AnyElement();
        }

        public bool IsValidRace(Race race) {
            if(anyRace) {
                return true;
            }
            return races.Contains(race);
        }

        public bool IsValidLevel(int level) {
            return (level >= minLevel);
        }


    }
}
