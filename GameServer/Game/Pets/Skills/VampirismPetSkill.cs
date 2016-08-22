using Nebula.Engine;
using Nebula.Game.Bonuses;
using Nebula.Game.Pets.Conditions;
using Nebula.Pets;

namespace Nebula.Game.Pets.Skills {
    public class VampirismPetSkill : OwnerTargetedPetSkill {

        private float m_DmgPc;
        private float m_vampPc;
        private float m_Time;

        private string m_BonusName1;
        private string m_BonusName2;

        public VampirismPetSkill(PetSkillInfo skillInfo, NebulaObject source ) 
            : base(skillInfo, source ) {

            if (data.cooldown > 0) {
                AddCondition(new CooldownCondition(data.cooldown, source));
            }

            AddCondition(new PetOwnerHpPcLessThanCondition(data.prob, source));

            object objDmgPc;
            object objVampPc;
            object objTime;

            if(data.inputs.TryGetValue("dmg_pc", out objDmgPc)) {
                m_DmgPc = (float)objDmgPc;
            }
            if(data.inputs.TryGetValue("vamp_pc", out objVampPc)) {
                m_vampPc = (float)objVampPc;
            }
            if(data.inputs.TryGetValue("time", out objTime)) {
                m_Time = (float)objTime;
            }
            m_BonusName1 = source.Id + "vps1";
            m_BonusName2 = source.Id + "vps2";
        }

        public override bool DoUse() {
            if(pet && pet.owner) {
                var ownerBonuses = pet.owner.Bonuses();

                Buff buffDmg = new Buff(m_BonusName1, null, Common.BonusType.increase_damage_on_pc, m_Time, m_DmgPc);
                Buff buffVamp = new Buff(m_BonusName2, null, Common.BonusType.vampirism_pc, m_Time, m_vampPc);
                ownerBonuses.SetBuff(buffDmg, pet.nebulaObject);
                ownerBonuses.SetBuff(buffVamp, pet.nebulaObject);
                return true;
            }
            return false;
        }
    }
}
