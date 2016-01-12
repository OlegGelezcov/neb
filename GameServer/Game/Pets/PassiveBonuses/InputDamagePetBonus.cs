using Common;
using Nebula.Engine;
using Nebula.Game.Bonuses;
using Nebula.Pets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Game.Pets.PassiveBonuses {
    public class InputDamagePetBonus : PassivePetBonus  {

        private string m_BonusName;
        public InputDamagePetBonus(PetPassiveBonusInfo data, NebulaObject source)
            : base(data, source) {
            m_BonusName = source.Id + "idpb";
        }

        public override bool Check() {
            var bons = ownerBonuses;
            if(bons) {
                if(false == bons.Contains(BonusType.decrease_input_damage_on_pc, m_BonusName)) {
                    Buff buff = new Buff(m_BonusName, null, BonusType.decrease_input_damage_on_pc, -1, data.value, () => {
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
