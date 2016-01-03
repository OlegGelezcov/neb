using Nebula.Game.Bonuses;
using Nebula.Pets;
using ServerClientCommon;
using System;
using System.Collections;

namespace Nebula.Game.Pets {
    public class AccelerationPetSkill : PetSkill {

        private PetSkillInfo m_Data;
        private float m_PercentValue;
        private float m_Time;
        private int m_Id;

        public AccelerationPetSkill(PetSkillInfo skill) {
            m_Data = skill;

            m_Id = m_Data.id;

            if(m_Data.prob < 1f ) {
                AddCondition(new ProbCondition(m_Data.prob));
            }    
            if(m_Data.cooldown > 0 ) {
                AddCondition(new CooldownCondition(m_Data.cooldown));
            }

            m_PercentValue = 0f;
            m_Time = 0f;
            object objPercentValue = null;
            object objTimeValue = null;
            if(m_Data.inputs.TryGetValue("value", out objPercentValue)) {
                m_PercentValue = (float)objPercentValue;
            }
            if(m_Data.inputs.TryGetValue("time", out objTimeValue)) {
                m_Time = (float)objTimeValue;
            }
        }

        public override bool DoUse(PetObject source) {
            if(source.owner) {
                var bonuses = source.owner.Bonuses();
                if(bonuses) {
                    Buff buff = new Buff("acceleration_pet_skill", null, Common.BonusType.increase_speed_on_pc, m_Time, m_PercentValue);
                    bonuses.SetBuff(buff);
                    return true;
                }
            }
            return false;
        }

        public override int id {
            get {
                return m_Id;
            }
        }

        protected override Hashtable GetProperties(PetObject source) {
            Hashtable hash =  base.GetProperties(source);
            if(source.owner) {
                hash.Add((int)SPC.Target, source.owner.Id);
                hash.Add((int)SPC.TargetType, source.owner.Type);
            }
            return hash;
        }
    }
}
