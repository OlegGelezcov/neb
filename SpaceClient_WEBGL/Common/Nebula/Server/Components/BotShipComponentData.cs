using Common;
#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif

namespace Nebula.Server.Components {
    public class BotShipComponentData : MultiComponentData  {

        public Difficulty difficulty { get; private set; }

#if UP
        public BotShipComponentData(UPXElement e) {
            if (e.HasAttribute("difficulty")) {
                difficulty = (Difficulty)System.Enum.Parse(typeof(Difficulty), e.GetString("difficulty"));
            } else {
                difficulty = Difficulty.none;
            }
        }
#else
        public BotShipComponentData(XElement e) {
            if(e.HasAttribute("difficulty")) {
                difficulty = (Difficulty)System.Enum.Parse(typeof(Difficulty), e.GetString("difficulty"));
            } else {
                difficulty = Difficulty.none;
            }
        }
#endif
        public BotShipComponentData(Difficulty difficulty) {
            this.difficulty = difficulty;
        }

        public override ComponentID componentID {
            get {
                return ComponentID.Ship;
            }
        }

        public override ComponentSubType subType {
            get {
                return ComponentSubType.ship_bot;
            }
        }
    }
}
