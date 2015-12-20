using Common;
#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif

namespace Nebula.Server.Components {
    public class ShipWeaponComponentData : MultiComponentData {

        public Difficulty difficulty { get; private set; }
        public float cooldown { get; private set; }
#if UP
        public ShipWeaponComponentData(UPXElement e) {
            if (e.HasAttribute("difficulty")) {
                difficulty = (Difficulty)System.Enum.Parse(typeof(Difficulty), e.GetString("difficulty"));
            } else {
                difficulty = Difficulty.none;
            }
            if (e.HasAttribute("cooldown")) {
                cooldown = e.GetFloat("cooldown");
            } else {
                cooldown = 2;
            }
        }
#else
        public ShipWeaponComponentData(XElement e) {
            if (e.HasAttribute("difficulty")) {
                difficulty = (Difficulty)System.Enum.Parse(typeof(Difficulty), e.GetString("difficulty"));
            } else {
                difficulty = Difficulty.none;
            }
            if(e.HasAttribute("cooldown")) {
                cooldown = e.GetFloat("cooldown");
            } else {
                cooldown = 2;
            }
        }
#endif
        public ShipWeaponComponentData(Difficulty difficulty, float cooldown) {
            this.difficulty = difficulty;
            this.cooldown = cooldown;
        }

        public override ComponentID componentID {
            get {
                return ComponentID.Weapon;
            }
        }
        public override ComponentSubType subType {
            get {
                return ComponentSubType.weapon_ship;
            }
        }
    }
}
