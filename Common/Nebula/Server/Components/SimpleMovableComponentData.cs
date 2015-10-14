using Common;
using System.Xml.Linq;
using System;
using System.Collections;
using ServerClientCommon;

namespace Nebula.Server.Components {
    public class SimpleMovableComponentData : MultiComponentData, IDatabaseComponentData {
        public float speed { get; private set; }
        public SimpleMovableComponentData(XElement componentElement) {
            speed = componentElement.GetFloat("speed");
        }

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
