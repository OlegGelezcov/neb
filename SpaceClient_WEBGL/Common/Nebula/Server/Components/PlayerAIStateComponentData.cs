using Common;
#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif

namespace Nebula.Server.Components {
    public class PlayerAIStateComponentData : ComponentData {
#if UP
        public PlayerAIStateComponentData(UPXElement e) { }
#else
        public PlayerAIStateComponentData(XElement e) { }
#endif
        public PlayerAIStateComponentData() { }

        public override ComponentID componentID {
            get {
                return ComponentID.PlayerAI;
            }
        }
    }
}
