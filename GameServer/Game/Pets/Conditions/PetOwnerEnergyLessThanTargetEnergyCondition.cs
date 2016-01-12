using Nebula.Engine;
using Nebula.Game.Components;

namespace Nebula.Game.Pets.Conditions {
    public class PetOwnerEnergyLessThanTargetEnergyCondition : Condition {

        public PetOwnerEnergyLessThanTargetEnergyCondition(NebulaObject source)
            : base(source) {

        }

        public override bool Check() {
            if(pet && pet.owner) {
                var ownerTarget = pet.owner.Target();
                var ownerEnergy = pet.owner.GetComponent<ShipEnergyBlock>();
                if(ownerTarget.hasTarget && ownerTarget.targetObject) {
                    var targetEnergy = ownerTarget.targetObject.GetComponent<ShipEnergyBlock>();
                    if(targetEnergy) {
                        if(ownerEnergy.currentEnergy < targetEnergy.currentEnergy ) {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public override void Renew() {
            
        }
    }
}
