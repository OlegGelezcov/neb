using Common;
using ServerClientCommon;
using System.Collections;
using System.Xml.Linq;

namespace Nebula.Server.Components {
    public class OutpostDamagableComponentData : FixedInputDamageDamagableComponentData, IDatabaseComponentData {

        public float additionalHP { get; private set; }

        public OutpostDamagableComponentData(XElement e) 
            : base (e) {
            additionalHP = e.GetFloat("additional_hp");
        }

        public OutpostDamagableComponentData(float maxHealth, bool ignoreDamageAtStart, float ignoreDamageInterval, bool createChestOnKilling, float fixedInputDamage, float additionalHP)
            : base(maxHealth, ignoreDamageAtStart, ignoreDamageInterval, createChestOnKilling, fixedInputDamage) {
            this.additionalHP = additionalHP;
        }

        public OutpostDamagableComponentData(Hashtable hash)
            : base(hash) {
            additionalHP = hash.GetValue<float>((int)SPC.AdditionalHp, 0f);
        }

        public override ComponentSubType subType {
            get {
                return ComponentSubType.damagable_outpost;
            }
        }

        public Hashtable AsHash() {
            return new Hashtable {
                { (int)SPC.MaxHealth, maxHealth },
                { (int)SPC.IgnoreDamagaeAtStart, ignoreDamageAtStart },
                { (int)SPC.IgnoreDamageInterval, ignoreDamageInterval },
                { (int)SPC.CreateChestOnKilling, createChestOnKilling },
                { (int)SPC.FixedInputDamage, fixedInputDamage },
                { (int)SPC.AdditionalHp, additionalHP }
            };
        }
    }
}
