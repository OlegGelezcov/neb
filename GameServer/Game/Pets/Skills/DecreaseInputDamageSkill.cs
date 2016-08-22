using Nebula.Engine;
using Nebula.Game.Bonuses;
using Nebula.Game.Pets.Conditions;
using Nebula.Pets;

namespace Nebula.Game.Pets.Skills {
    public class DecreaseInputDamageSkill : OwnerTargetedPetSkill {

        private float m_BonusPercent;
        private float m_BonusTime;
        private string m_BonusName;

        public DecreaseInputDamageSkill(PetSkillInfo skill, NebulaObject source)
            : base(skill, source) {

            AddCondition(new PetOwnerHpPcLessThanCondition(data.prob, source));
            if(data.cooldown > 0.0f ) {
                AddCondition(new CooldownCondition(data.cooldown, source));
            }
            object objBonusPercent;
            object objBonusTime;
            if(data.inputs.TryGetValue("value", out objBonusPercent)) {
                m_BonusPercent = (float)objBonusPercent;
            }
            if(data.inputs.TryGetValue("time", out objBonusTime)) {
                m_BonusTime = (float)objBonusTime;
            }
            m_BonusName = source.Id + "dids";
        }

        public override bool DoUse() {
            if(false == pet.owner) {
                return false;
            }
            var bonuses = pet.owner.Bonuses();
            if(false == bonuses) {
                return false;
            }
            Buff buff = new Buff(m_BonusName, null, Common.BonusType.decrease_input_damage_on_pc, m_BonusTime, m_BonusPercent);
            bonuses.SetBuff(buff, pet.nebulaObject);
            return true;
        }

    }
}
