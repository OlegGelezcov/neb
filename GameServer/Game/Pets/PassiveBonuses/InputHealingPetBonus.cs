using Common;
using Nebula.Engine;
using Nebula.Game.Bonuses;
using Nebula.Pets;

namespace Nebula.Game.Pets.PassiveBonuses {
    public class InputHealingPetBonus : PassivePetBonus {
        private string m_BonusName;

        public InputHealingPetBonus(PetPassiveBonusInfo bonus, NebulaObject source) 
            : base(bonus, source) {
            m_BonusName = source.Id + "ihpb";
        }

        public override bool Check() {
            var bons = ownerBonuses;
            if(bons) {
                if(false == bons.Contains(BonusType.increase_healing_speed_on_pc, m_BonusName)) {
                    Buff buff = new Buff(m_BonusName, null, BonusType.increase_healing_speed_on_pc, -1, data.value, () => {
                        return (pet) && (pet.owner);
                    }, -1, false);
                    if (pet != null) {
                        bons.SetBuff(buff, pet.nebulaObject);
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
