using Nebula.Engine;
using Nebula.Pets;

namespace Nebula.Game.Pets.PassiveBonuses {
    public class EmptyPetBonus : PassivePetBonus {
        public EmptyPetBonus(PetPassiveBonusInfo bonus, NebulaObject source)
            : base(bonus, source) { }
        public override bool Check() {
            return false;
        }
    }
}
