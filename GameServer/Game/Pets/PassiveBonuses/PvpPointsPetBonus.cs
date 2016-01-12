using Common;
using Nebula.Engine;
using Nebula.Game.Bonuses;
using Nebula.Pets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Game.Pets.PassiveBonuses {
    public class PvpPointsPetBonus : PassivePetBonus {

        private float m_Value;

        private readonly string m_BonusName;

        public PvpPointsPetBonus(PetPassiveBonusInfo data, NebulaObject source)
            : base(data, source) {
            m_Value = data.value;
            m_BonusName = source.Id + "pppb";
        }

        public override bool Check() {
            var bonuses = ownerBonuses;
            if(bonuses) {
                if( false == bonuses.Contains(BonusType.increase_pvp_points, m_BonusName)) {
                    Buff buff = new Buff(m_BonusName, null, BonusType.increase_pvp_points, -1, m_Value, () => {
                        return (pet) && (pet.owner);
                    }, -1, false);
                    bonuses.SetBuff(buff);
                    return true;
                }
            }
            return false;
        }
    }
}
