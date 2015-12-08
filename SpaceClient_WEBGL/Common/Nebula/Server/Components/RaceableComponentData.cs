using Common;
using System;
using System.Xml.Linq;
using ExitGames.Client.Photon;
using ServerClientCommon;

namespace Nebula.Server.Components {
    public class RaceableComponentData : ComponentData, IDatabaseComponentData {

        public Race race { get; private set; }

        public RaceableComponentData(XElement element) {
            race = (Race)Enum.Parse(typeof(Race), element.GetString("race"));
        }

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
