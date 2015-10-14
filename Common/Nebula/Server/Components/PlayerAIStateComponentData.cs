using Common;
using System.Xml.Linq;

namespace Nebula.Server.Components {
    public class PlayerAIStateComponentData : ComponentData {

        public PlayerAIStateComponentData(XElement e) { }

        public PlayerAIStateComponentData() { }

        public override ComponentID componentID {
            get {
                return ComponentID.PlayerAI;
            }
        }
    }
}
