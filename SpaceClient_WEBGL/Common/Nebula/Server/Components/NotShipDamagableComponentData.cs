using Common;
using ServerClientCommon;
using ExitGames.Client.Photon;
#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif

namespace Nebula.Server.Components {
    public class NotShipDamagableComponentData : MultiComponentData {

        public float maxHealth { get; private set; }
        public bool ignoreDamageAtStart { get; private set; }
        public float ignoreDamageInterval { get; private set; }
        public bool createChestOnKilling { get; private set; }

#if UP
        public NotShipDamagableComponentData(UPXElement e) {
            maxHealth = e.GetFloat("max_health");


            ignoreDamageAtStart = e.GetBool("ignore_damage_at_start");
            ignoreDamageInterval = e.GetFloat("ignore_damage_interval");

            if (e.HasAttribute("create_chest_when_killed"))
                createChestOnKilling = e.GetBool("create_chest_when_killed");
            else
                createChestOnKilling = true;
        }
#else
        public NotShipDamagableComponentData(XElement e) {
            maxHealth = e.GetFloat("max_health");


            ignoreDamageAtStart = e.GetBool("ignore_damage_at_start");
            ignoreDamageInterval = e.GetFloat("ignore_damage_interval");

            if (e.HasAttribute("create_chest_when_killed"))
                createChestOnKilling = e.GetBool("create_chest_when_killed");
            else
                createChestOnKilling = true;
        }
#endif
        public NotShipDamagableComponentData(float maxHealth, bool ignoreDamageAtStart, float ignoreDamageInterval, bool createChestOnKilling) {
            this.maxHealth = maxHealth;
            this.ignoreDamageAtStart = ignoreDamageAtStart;
            this.ignoreDamageInterval = ignoreDamageInterval;
            this.createChestOnKilling = createChestOnKilling;
        }

        public NotShipDamagableComponentData(Hashtable hash) {
            maxHealth = hash.GetValue<float>((int)SPC.MaxHealth, 0f);
            ignoreDamageAtStart = hash.GetValue<bool>((int)SPC.IgnoreDamagaeAtStart, false);
            ignoreDamageInterval = hash.GetValue<float>((int)SPC.IgnoreDamageInterval, 0f);
            createChestOnKilling = hash.GetValue<bool>((int)SPC.CreateChestOnKilling, false);
        } 

        public override ComponentID componentID {
            get {
                return ComponentID.Damagable;
            }
        }

        public override ComponentSubType subType {
            get {
                return ComponentSubType.damagable_not_ship;
            }
        }
    }
}
