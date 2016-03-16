using ExitGames.Logging;
using Nebula.Drop;
using Nebula.Game.Components;

namespace Nebula.Game.Pets {
    public class PetDamagable : NotShipDamagableObject {
        private static ILogger s_Log = LogManager.GetCurrentClassLogger();

        private PetObject m_Pet;

        public override void Awake() {
            base.Awake();
        }

        public override void Start() {
            base.Start();
            m_Pet = GetComponent<PetObject>();
            ForceSetHealth(maximumHealth);
            SetIgnoreDamageAtStart(false);
            SetIgnoreDamageInterval(0);
            SetCreateChestOnKilling(false);
        }

        public override float maximumHealth {
            get {
                return baseMaximumHealth;
            }
        }

        public override float baseMaximumHealth {
            get {
                if (m_Pet) {
                    if (m_Pet.info != null) {
                        return m_Pet.info.Hp(nebulaObject.resource.petParameters.hp, nebulaObject.resource.Leveling);
                    }
                }
                return 0f;
            }
        }

        public override void Update(float deltaTime) {
            base.Update(deltaTime);
        }

        protected override void ModifyDamage(InputDamage damage) {
            base.ModifyDamage(damage);
        }

        public override InputDamage ReceiveDamage(InputDamage inputDamage) {
            return base.ReceiveDamage(inputDamage);
        }

        protected override void AbsorbDamage(InputDamage inputDamage) {
            base.AbsorbDamage(inputDamage);
        }

        public override void Death() {

            s_Log.InfoFormat("Called Death() on Pet [red]");

            if (m_Pet.owner) {
                var petManager = m_Pet.owner.GetComponent<PetManager>();
                if (petManager) {
                    s_Log.InfoFormat("Remove pet from pet manager [red]");
                    petManager.RemovePet(nebulaObject.Id);
                }
            }

            base.Death();
        }
    }

}
