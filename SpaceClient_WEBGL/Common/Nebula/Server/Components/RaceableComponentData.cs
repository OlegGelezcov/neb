using Common;
using System;
using ExitGames.Client.Photon;
using ServerClientCommon;
#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif

namespace Nebula.Server.Components {
    public class RaceableComponentData : ComponentData, IDatabaseComponentData {

        public Race race { get; private set; }

#if UP
        public RaceableComponentData(UPXElement element) {
            race = (Race)Enum.Parse(typeof(Race), element.GetString("race"));
        }
#else
        public RaceableComponentData(XElement element) {
            race = (Race)Enum.Parse(typeof(Race), element.GetString("race"));
        }
#endif

        public RaceableComponentData(Race race) {
            this.race = race;
        }

        public RaceableComponentData(Hashtable hash) {
            race = (Race)(byte)hash.GetValue<int>((int)SPC.Race, (int)Race.None);
        }
        public override ComponentID componentID {
            get {
                return ComponentID.Raceable;
            }
        }

        public Hashtable AsHash() {
            return new Hashtable {
                {(int)SPC.Race, (int)race }
            };
        }
    }
}
