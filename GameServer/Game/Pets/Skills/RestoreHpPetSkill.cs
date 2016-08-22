using Nebula.Engine;
using Nebula.Game.Bonuses;
using Nebula.Game.Pets.Conditions;
using Nebula.Pets;

namespace Nebula.Game.Pets.Skills {
    public class RestoreHpPetSkill : OwnerTargetedPetSkill {

        private float m_MaxHpPc;
        private float m_HpAtSecPc;
        private float m_Time;

        private string m_BonusName1;
        private string m_BonusName2;

        public RestoreHpPetSkill(PetSkillInfo skillInfo, NebulaObject source ) 
            : base(skillInfo, source ) {
            AddCondition(new PetOwnerHpPcLessThanCondition(data.prob, source));
            if (data.cooldown > 0.0f) {
                AddCondition(new CooldownCondition(data.cooldown, source));
            }

            object objMaxHpPc;
            object objHpAtSecPc;
            object objTime;
            if(data.inputs.TryGetValue("max_hp_pc", out objMaxHpPc)) {
                m_MaxHpPc = (float)objMaxHpPc;
            }
            if(data.inputs.TryGetValue("hp_at_sec_pc", out objHpAtSecPc)) {
                m_HpAtSecPc = (float)objHpAtSecPc;
            }
            if(data.inputs.TryGetValue("time", out objTime)) {
                m_Time = (float)objTime;
            }
            m_BonusName1 = source.Id + "rhps1";
            m_BonusName2 = source.Id + "rhps2";
        }

        public override bool DoUse() {
            if (pet && pet.owner) {
                var bonuses = pet.owner.Bonuses();

                Buff buff = new Buff(m_BonusName1, null, Common.BonusType.increase_max_hp_on_pc, m_Time, m_MaxHpPc);
                Buff buff2 = new Buff(m_BonusName2, null, Common.BonusType.restore_hp_at_sec_on_pc, m_Time, m_HpAtSecPc);
                bonuses.SetBuff(buff, pet.nebulaObject);
                bonuses.SetBuff(buff2, pet.nebulaObject);
                return true;
            }
            return false;
        }
    }
}
