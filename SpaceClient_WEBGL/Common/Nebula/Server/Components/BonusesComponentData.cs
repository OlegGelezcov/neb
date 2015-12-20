using Common;
#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif
using System;
using ExitGames.Client.Photon;

namespace Nebula.Server.Components {
    public class BonusesComponentData : ComponentData, IDatabaseComponentData{
#if UP
        public BonusesComponentData(UPXElement e) { }
#else
        public BonusesComponentData(XElement e) { }
#endif
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
