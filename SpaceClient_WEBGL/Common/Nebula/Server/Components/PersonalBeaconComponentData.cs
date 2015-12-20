using Common;
using ServerClientCommon;
using ExitGames.Client.Photon;
#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif

namespace Nebula.Server.Components {
    public class PersonalBeaconComponentData : MultiComponentData, IDatabaseComponentData {

        public float time { get; private set; }

        public override ComponentSubType subType {
            get {
                return ComponentSubType.PersonalTeleport;
            }
        }

        public override ComponentID componentID {
            get {
                return ComponentID.Teleport;
            }
        }

#if UP
        public PersonalBeaconComponentData(UPXElement element) {
            time = element.GetFloat("time");
        }
#else
        public PersonalBeaconComponentData(XElement element) {
            time = element.GetFloat("time");
        }
#endif
        public PersonalBeaconComponentData(float time) {
            this.time = time;
        }

        public PersonalBeaconComponentData(Hashtable hash) {
            time = hash.GetValue<float>((int)SPC.Time, 0f);
        }

        public Hashtable AsHash() {
            return new Hashtable {
                { (int)SPC.Time, time }
            };
        }
    }
}
