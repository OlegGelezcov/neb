using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Nebula.Resources.PlayerConstructions {
    public abstract class PlanetConstructionData {
        public float maxHp { get; private set; }
        public float life { get; private set; }

        public PlanetConstructionData(XElement element ) {
            maxHp = element.GetFloat("max_hp");
            life = element.GetFloat("life");
        }
    }

    public class PlanetCommandCenterData : PlanetConstructionData {
        public PlanetCommandCenterData(XElement e)
            : base(e) { }
    }

    public class PlanetTurretData : PlanetConstructionData {
        public float damage { get; private set; }
        public float od { get; private set; }

        public PlanetTurretData(XElement e)
            : base(e) {
            damage = e.GetFloat("dmg");
            od = e.GetFloat("od");
        }
    }

    public class PlanetMiningStationData : PlanetConstructionData {
        public float workSpeed { get; private set; }
        public int maxSlots { get; private set; }

        public PlanetMiningStationData(XElement e)
            : base(e) {
            workSpeed = 1.0f / e.GetFloat("work_speed");
            maxSlots = e.GetInt("max_slots");
        }
    }

    public class PlanetResourceHangarData : PlanetConstructionData {
        public PlanetResourceHangarData(XElement e)
            : base(e) { }
    }

    public class PlanetResourceAcceleratorData : PlanetConstructionData {
        public PlanetResourceAcceleratorData(XElement e)
            : base(e) { }
    }

}
