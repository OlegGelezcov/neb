using Nebula.Server.Components;

namespace Nebula.Game.Components {
    public class FixedInputDamageDamagableObject : NotShipDamagableObject {

        private float mFixedDamage = 0f;

        public void Init(FixedInputDamageDamagableComponentData data) {
            base.Init(data);
            mFixedDamage = data.fixedInputDamage;
        }

        protected override void ModifyDamage(InputDamage damage) {
            if (damage.ignoreFixedDamage) {
                //we ignore damage
            } else {
                damage.ClearAllDamages();
                damage.SetBaseDamage(mFixedDamage);
            }
        }

        protected override void AbsorbDamage(InputDamage inputDamage) {
            //inputDamage.ClearAllDamages();
            //inputDamage.SetBaseDamage(mFixedDamage);
        }
    }
}
