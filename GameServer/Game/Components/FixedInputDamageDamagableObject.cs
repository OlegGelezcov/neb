using Nebula.Server.Components;

namespace Nebula.Game.Components {
    public class FixedInputDamageDamagableObject : NotShipDamagableObject {

        private float m_FixedDamage = 0f;


        #region Properties
        protected float fixedDamage {
            get {
                return m_FixedDamage;
            }
        } 
        #endregion

        public void Init(FixedInputDamageDamagableComponentData data) {
            base.Init(data);
            m_FixedDamage = data.fixedInputDamage;
        }

        protected override void ModifyDamage(InputDamage damage) {
            if (damage.ignoreFixedDamage) {
                //we ignore damage
            } else {
                damage.ClearAllDamages();
                damage.SetBaseDamage(m_FixedDamage);
            }
        }

        protected override void AbsorbDamage(InputDamage inputDamage) {
            //inputDamage.ClearAllDamages();
            //inputDamage.SetBaseDamage(mFixedDamage);
        }


    }
}
