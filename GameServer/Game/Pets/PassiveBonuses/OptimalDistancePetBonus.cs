using Common;
using Nebula.Engine;
using Nebula.Game.Bonuses;
using Nebula.Pets;

namespace Nebula.Game.Pets.PassiveBonuses {
    public class OptimalDistancePetBonus : PassivePetBonus  {
        private string m_BonusName;
        public OptimalDistancePetBonus(PetPassiveBonusInfo data, NebulaObject source)
            : base(data, source) {
            m_BonusName = source.Id + "odpb";
        }

        public override bool Check() {
            var bons = ownerBonuses;
            if(bons) {
                if(false == bons.Contains(BonusType.increase_optimal_distance_on_cnt, m_BonusName)) {
                    Buff buff = new Buff(m_BonusName, null, BonusType.increase_optimal_distance_on_cnt, -1, data.value, () => {
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
