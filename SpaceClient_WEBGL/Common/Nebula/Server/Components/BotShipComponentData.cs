using Common;
using System.Xml.Linq;

namespace Nebula.Server.Components {
    public class BotShipComponentData : MultiComponentData  {

        public Difficulty difficulty { get; private set; }

        public BotShipComponentData(XElement e) {
            if(e.HasAttribute("difficulty")) {
                difficulty = (Difficulty)System.Enum.Parse(typeof(Difficulty), e.GetString("difficulty"));
            } else {
                difficulty = Difficulty.none;
            }
        }

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
