using Nebula.Server.Components;

namespace Nebula.Game.Components {
    public class FixedInputDamageDamagableObject : NotShipDamagableObject {

        private float mFixedDamage = 0f;

        public void Init(FixedInputDamageDamagableComponentData data) {
            base.Init(data);
            mFixedDamage = data.fixedInputDamage;
        }

        protected override float ModifyDamage(float damage) {
            return mFixedDamage;
        }

        protected override float AbsorbDamage(float inputDamage) {
            return mFixedDamage;
        }
    }
}
