using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Nebula.Client.Res {
    public class WorkshopStatData {
        public Workshop workshop { get; private set; }
        public int damage { get; private set; }
        public int speed { get; private set; }
        public int hp { get; private set; }
        //public int cargo { get; private set; }
        public int optimalDistance { get; private set; }
        public int critChance { get; private set; }

        public WorkshopStatData(XElement e) {
            workshop = (Workshop)Enum.Parse(typeof(Workshop), e.GetString("name"));
            damage = e.GetInt("damage");
            speed = e.GetInt("speed");
            hp = e.GetInt("hp");
            //cargo = e.GetInt("cargo");
            optimalDistance = e.GetInt("optimal_distance");
            critChance = e.GetInt("crit_chance");
        }


    }
}
