using Nebula.Engine;

namespace Nebula.Game.Pets.Conditions {
    public class PetOwnerHasSubscriberCondition : Condition{
        public PetOwnerHasSubscriberCondition(NebulaObject source)
            : base(source) { }

        public override bool Check() {
            if(pet && pet.owner) {
                var ownerTarget = pet.owner.Target();
                if(ownerTarget.anyEnemySubscriber) {
                    return true;
                }
            }
            return false;
        }

        public override void Renew() {
            
        }
    }
}
