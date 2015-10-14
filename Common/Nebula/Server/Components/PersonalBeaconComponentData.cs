using Common;
using ServerClientCommon;
using System.Collections;
using System.Xml.Linq;

namespace Nebula.Server.Components {
    public class PersonalBeaconComponentData : TeleportComponentData, IDatabaseComponentData {

        public float time { get; private set; }

        public PersonalBeaconComponentData(XElement element)
            : base(element) {
            time = element.GetFloat("time");
        }

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
