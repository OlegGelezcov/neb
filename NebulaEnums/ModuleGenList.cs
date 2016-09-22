using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common {

    public class ModuleGenList {
        public string id;
        public ShipModelSlotType slot;
        public int level;
        public string name;
        public Workshop workshop;
        public string dataId;
        public List<DeconstructItem> deconstructOre;
        public Difficulty difficulty;
        public ObjectColor color;
        public string model;
        public float hp;
        public float speed;
        public int hold;
        public float critDamage;
        public float resist;
        public float damageBonus;
        public float energyBonus;
        public float critChance;
        public float speedBonus;
        public float holdBonus;
        public int skill;
        public string setId;
    }

    public class DeconstructItem {
        public string id;
        public int count;
    }
}
