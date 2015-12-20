using Common;
using System;
#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif

namespace Nebula.Server.Components {
    public class PlayerCharacterComponentData : MultiComponentData {
        public Workshop workshop { get; private set; }
        public int level { get; private set; }
        public FractionType fraction { get; private set; }

#if UP
        public PlayerCharacterComponentData(UPXElement e) {
            workshop = (Workshop)Enum.Parse(typeof(Workshop), e.GetString("workshop"));
            level = e.GetInt("level");
            fraction = (FractionType)Enum.Parse(typeof(FractionType), e.GetString("fraction"));
        }
#else
        public PlayerCharacterComponentData(XElement e) {
            workshop = (Workshop)Enum.Parse(typeof(Workshop), e.GetString("workshop"));
            level = e.GetInt("level");
            fraction = (FractionType)Enum.Parse(typeof(FractionType), e.GetString("fraction"));
        }
#endif
        public PlayerCharacterComponentData(Workshop workshop, int level, FractionType fraction) {
            this.workshop = workshop;
            this.level = level;
            this.fraction = fraction;
        }


        public override ComponentID componentID {
            get {
                return ComponentID.Character;
            }
        }

        public override ComponentSubType subType {
            get {
                return ComponentSubType.character_player;
            }
        }
    }
}
