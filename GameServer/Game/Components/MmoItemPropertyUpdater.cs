using Common;
using Nebula.Engine;
using ServerClientCommon;
using Space.Game;
using Space.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;

namespace Nebula.Game.Components {
    public class MmoItemPropertyUpdater : NebulaBehaviour {

        private NebulaObjectProperties properties;

        public override void Start() {
            properties = nebulaObject.GetComponent<NebulaObjectProperties>();     
        }

        public override void Update(float deltaTime) {

        }

        public override int behaviourId {
            get {
                return (int)ComponentID.PropertyUpdater;
            }
        }
    }
}
