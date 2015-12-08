using Common;
using System.Xml.Linq;
using System;
using ExitGames.Client.Photon;

namespace Nebula.Server.Components {
    public class BonusesComponentData : ComponentData, IDatabaseComponentData{

        public BonusesComponentData(XElement e) { }

        public BonusesComponentData() { }

        public BonusesComponentData(Hashtable hash) { }

        public override ComponentID componentID {
            get {
                return ComponentID.Bonuses;
            }
        }

        public Hashtable AsHash() {
            return new Hashtable();
        }
    }
}
