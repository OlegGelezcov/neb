using Common;
using Nebula.Engine;
using Nebula.Game.Bonuses;
using Nebula.Pets;

namespace Nebula.Game.Pets.PassiveBonuses {
    public class AutoLootPetBonus : PassivePetBonus {
        private string m_BonusName;

        public AutoLootPetBonus(PetPassiveBonusInfo data, NebulaObject source)
            : base(data, source) {
            m_BonusName = source.Id + "alb";
        }

        public override bool Check() {
            var bons = ownerBonuses;
            if(bons) {
                if(false == bons.Contains(BonusType.auto_loot_chest, m_BonusName)) {
                    Buff buff = new Buff(m_BonusName, null, BonusType.auto_loot_chest, -1, 1, () => {
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
