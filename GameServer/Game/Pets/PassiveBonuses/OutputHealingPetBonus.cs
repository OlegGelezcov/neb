﻿using Common;
using Nebula.Engine;
using Nebula.Game.Bonuses;
using Nebula.Pets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Game.Pets.PassiveBonuses {
    public class OutputHealingPetBonus : PassivePetBonus {
        private string m_BonusName;

        public OutputHealingPetBonus(PetPassiveBonusInfo data, NebulaObject source)
            : base(data, source) {
            m_BonusName = source.Id + "ohpb";
        }

        public override bool Check() {
            var bons = ownerBonuses;
            if(bons) {
                if(false == bons.Contains(BonusType.increase_healing_on_pc, m_BonusName)) {
                    Buff buff = new Buff(m_BonusName, null, BonusType.increase_healing_on_pc, -1, data.value, () => {
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
