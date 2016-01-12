using Nebula.Engine;
using Nebula.Game.Bonuses;
using Nebula.Game.Pets.Conditions;
using Nebula.Pets;

namespace Nebula.Game.Pets.Skills {
    public class ReflectionDamageSkill : OwnerTargetedPetSkill {

        private float m_Value;
        private float m_Time;

        private string m_BuffName;

        public ReflectionDamageSkill(PetSkillInfo skillInfo, NebulaObject source)
            : base(skillInfo, source) {
            if(data.prob < 1.0f ) {
                AddCondition(new ProbCondition(data.prob, source));
            }
            if(data.cooldown > 0.0f ) {
                AddCondition(new CooldownCondition(data.cooldown, source));
            }
            AddCondition(new PetOwnerInCombatCondition(source));

            object objVal;
            object objTime;
            if(data.inputs.TryGetValue("value", out objVal)) {
                m_Value = (float)objVal;
            }
            if(data.inputs.TryGetValue("time", out objTime)) {
                m_Time = (float)objTime;
            }

            m_BuffName = source.Id + "rds";
            if (id == 4) {
                m_BuffName += "4";
            } else if (id == 5) {
                m_BuffName += "5";
            }
        }

        public override bool DoUse() {
            var bonuses = pet.owner.Bonuses();
            if(bonuses && (false == bonuses.Contains(m_BuffName))) {

                Buff buff = new Buff(m_BuffName, null, Common.BonusType.reflection_pc, m_Time, m_Value);
                bonuses.SetBuff(buff);
                return true;
            }
            return false;
        }
    }
}
