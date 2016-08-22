using Common;
using Nebula.Engine;
using Nebula.Game.Bonuses;
using Nebula.Game.Components;
using Nebula.Pets;
using Space.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Game.Pets.PassiveBonuses {
    public class CreditsPetBonus : PassivePetBonus {

        private readonly string m_BonusName;

        public CreditsPetBonus(PetPassiveBonusInfo data, NebulaObject source)
            : base(data, source) {
            m_BonusName = source.Id + "cpb";
        }

        public override bool Check() {
            if(pet && pet.owner) {
                var bons = ownerBonuses;
                if(bons) {
                    if(false == bons.Contains(BonusType.increase_credits, m_BonusName)) {
                        Buff buff = new Buff(m_BonusName, null, BonusType.increase_credits, -1, data.value, () => {
                            return (pet) && (pet.owner);
                        }, -1, false);
                        bons.SetBuff(buff, pet.nebulaObject);
                        float creditsBonus = bons.creditsPcBonus;
                        var characterId = pet.owner.GetComponent<PlayerCharacterObject>().characterId;
                        pet.owner.GetComponent<MmoActor>().application.updater.SetCreditsBonus(characterId, creditsBonus);
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
