using Common;
using ServerClientCommon;
using ExitGames.Client.Photon;
using System.Xml.Linq;
using Nebula.Client.Utils;

namespace Nebula.Server.Components {
    public class NotShipDamagableComponentData : MultiComponentData {

        public float maxHealth { get; private set; }
        public bool ignoreDamageAtStart { get; private set; }
        public float ignoreDamageInterval { get; private set; }
        public bool createChestOnKilling { get; private set; }

        public NotShipDamagableComponentData(XElement e) {
            maxHealth = e.GetFloat("max_health");


            ignoreDamageAtStart = e.GetBool("ignore_damage_at_start");
            ignoreDamageInterval = e.GetFloat("ignore_damage_interval");

            if (e.HasAttribute("create_chest_when_killed"))
                createChestOnKilling = e.GetBool("create_chest_when_killed");
            else
                createChestOnKilling = true;
        }

        public NotShipDamagableComponentData(float maxHealth, bool ignoreDamageAtStart, float ignoreDamageInterval, bool createChestOnKilling) {
            this.maxHealth = maxHealth;
            this.ignoreDamageAtStart = ignoreDamageAtStart;
            this.ignoreDamageInterval = ignoreDamageInterval;
            this.createChestOnKilling = createChestOnKilling;
        }

        public NotShipDamagableComponentData(Hashtable hash) {
            maxHealth = hash.GetValueFloat((int)SPC.MaxHealth);
            ignoreDamageAtStart = hash.GetValueBool((int)SPC.IgnoreDamagaeAtStart);
            ignoreDamageInterval = hash.GetValueFloat((int)SPC.IgnoreDamageInterval);
            createChestOnKilling = hash.GetValueBool((int)SPC.CreateChestOnKilling);
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
