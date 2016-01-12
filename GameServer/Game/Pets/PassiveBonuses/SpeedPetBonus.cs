using Common;
using Nebula.Engine;
using Nebula.Game.Bonuses;
using Nebula.Pets;

namespace Nebula.Game.Pets.PassiveBonuses {
    public class SpeedPetBonus : PassivePetBonus {
        private string m_BonusName;
        public SpeedPetBonus(PetPassiveBonusInfo data, NebulaObject source) 
            : base(data, source) {
            m_BonusName = source.Id + "spb";
        }

        public override bool Check() {
            var bons = ownerBonuses;
            if(bons) {
                if(false == bons.Contains(BonusType.increase_speed_on_pc, m_BonusName)) {
                    Buff buff = new Buff(m_BonusName, null, BonusType.increase_speed_on_pc, -1, data.value, () => {
                        return (pet) && (pet.owner);
                    }, -1, false);
                    bons.SetBuff(buff);
                    return true;
                }
            }
            return false;
        }
    }
}
