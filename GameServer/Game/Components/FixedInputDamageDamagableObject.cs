using Nebula.Drop;
using Nebula.Server.Components;

namespace Nebula.Game.Components {
    public class FixedInputDamageDamagableObject : NotShipDamagableObject {

        private float mFixedDamage = 0f;

        public void Init(FixedInputDamageDamagableComponentData data) {
            base.Init(data);
            mFixedDamage = data.fixedInputDamage;
        }

        protected override WeaponDamage ModifyDamage(WeaponDamage damage) {
            damage.ClearAllDamages();
            damage.SetBaseTypeDamage(mFixedDamage);
            return damage;
        }

        protected override WeaponDamage AbsorbDamage(WeaponDamage inputDamage) {
            inputDamage.ClearAllDamages();
            inputDamage.SetBaseTypeDamage(mFixedDamage);
            return inputDamage;
        }
    }
}
