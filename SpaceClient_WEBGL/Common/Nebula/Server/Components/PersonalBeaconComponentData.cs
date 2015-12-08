using Common;
using ServerClientCommon;
using ExitGames.Client.Photon;
using System.Xml.Linq;
using System;

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

        public PersonalBeaconComponentData(XElement element) {
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
