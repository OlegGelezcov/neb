using Nebula.Engine;
using Nebula.Game.Bonuses;
using Nebula.Game.Pets.Conditions;
using Nebula.Pets;
using ServerClientCommon;
using System;
using System.Collections;

namespace Nebula.Game.Pets.Skills {
    public class AccelerationPetSkill : OwnerTargetedPetSkill {

        private float m_PercentValue;
        private float m_Time;

        private string m_BonusName;

        public AccelerationPetSkill(PetSkillInfo skill, NebulaObject source) 
            : base(skill, source) {

            if(data.prob < 1f ) {
                AddCondition(new ProbCondition(data.prob, source));
            }    
            if(data.cooldown > 0 ) {
                AddCondition(new CooldownCondition(data.cooldown, source));
            }

            m_PercentValue = 0f;
            m_Time = 0f;
            object objPercentValue = null;
            object objTimeValue = null;
            if(data.inputs.TryGetValue("value", out objPercentValue)) {
                m_PercentValue = (float)objPercentValue;
            }
            if(data.inputs.TryGetValue("time", out objTimeValue)) {
                m_Time = (float)objTimeValue;
            }
            m_BonusName = source.Id + "aps";
        }

        public override bool DoUse() {
            if(pet.owner) {
                var bonuses = pet.owner.Bonuses();
                if(bonuses) {
                    Buff buff = new Buff(m_BonusName, null, Common.BonusType.increase_speed_on_pc, m_Time, m_PercentValue);
                    bonuses.SetBuff(buff, pet.nebulaObject);
                    return true;
                }
            }
            return false;
        }
    }
}
