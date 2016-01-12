using Nebula.Engine;
using Nebula.Game.Components;

namespace Nebula.Game.Pets.Conditions {
    public class PetOwnerInCombatCondition : Condition {

        public PetOwnerInCombatCondition(NebulaObject source) : base(source) {
        }

        public override bool Check() {
            if(pet && pet.owner) {
                var ownerTarget = pet.owner.GetComponent<PlayerTarget>();
                if(ownerTarget) {
                    return ownerTarget.inCombat;
                }
            }
            return false;
        }

        public override void Renew() {
            
        }
    }
}
