using Common;
using ExitGames.Client.Photon;
using ServerClientCommon;
#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif

namespace Nebula.Server.Components {
    public class SimpleMovableComponentData : MultiComponentData, IDatabaseComponentData {
        public float speed { get; private set; }

#if UP
        public SimpleMovableComponentData(UPXElement componentElement) {
            speed = componentElement.GetFloat("speed");
        }
#else
        public SimpleMovableComponentData(XElement componentElement) {
            speed = componentElement.GetFloat("speed");
        }
#endif
        public SimpleMovableComponentData(float speed) {
            this.speed = speed;
        }
        public SimpleMovableComponentData(Hashtable hash) {
            speed = hash.GetValue<float>((int)SPC.Speed, 0f);
        }

        public override ComponentID componentID {
            get {
                return ComponentID.Movable;
            }
        }

        public override ComponentSubType subType {
            get {
                return ComponentSubType.simple_movable;
            }
        }

        public Hashtable AsHash() {
            return new Hashtable {
                { (int)SPC.Speed, speed }
            };
        }
    }
}
