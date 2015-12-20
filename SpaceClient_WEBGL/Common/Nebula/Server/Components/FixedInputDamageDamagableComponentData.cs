using Common;
using ServerClientCommon;
using ExitGames.Client.Photon;
#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif

namespace Nebula.Server.Components {
    public class FixedInputDamageDamagableComponentData : NotShipDamagableComponentData {

        public float fixedInputDamage { get; private set; }

#if UP
        public FixedInputDamageDamagableComponentData(UPXElement e) : base(e) {
            fixedInputDamage = e.GetFloat("fixed_input_damage");
        }
#else
        public FixedInputDamageDamagableComponentData(XElement e) : base(e) {
            fixedInputDamage = e.GetFloat("fixed_input_damage");
        }
#endif
        public FixedInputDamageDamagableComponentData(float maxHealth, bool ignoreDamageAtStart, float ignoreDamageInterval, bool createChestOnKilling, float fixedInputDamage)
            : base(maxHealth, ignoreDamageAtStart, ignoreDamageInterval, createChestOnKilling ) {
            this.fixedInputDamage = fixedInputDamage;
        }

        public FixedInputDamageDamagableComponentData(Hashtable hash)
            : base(hash) {
            fixedInputDamage = hash.GetValue<float>((int)SPC.FixedInputDamage, 0f);
        }

        public override ComponentSubType subType {
            get {
                return ComponentSubType.damagable_fixed_damage;
            }
        }
    }
}
