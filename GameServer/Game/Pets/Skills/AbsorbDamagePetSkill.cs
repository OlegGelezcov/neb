using Nebula.Engine;
using Nebula.Game.Bonuses;
using Nebula.Game.Pets.Conditions;
using Nebula.Pets;

namespace Nebula.Game.Pets.Skills {
    public class AbsorbDamagePetSkill : OwnerTargetedPetSkill {

        private float m_AbsorbDmgPc;
        private float m_ConvertDmgPc;
        private float m_Time;

        private string m_BonusName1;
        private string m_BonusName2;


        public AbsorbDamagePetSkill(PetSkillInfo skillInfo, NebulaObject source)
            : base(skillInfo, source) {
            if (data.prob < 1f) {
                AddCondition(new ProbCondition(data.prob, source));
            }
            if (data.cooldown > 0) {
                AddCondition(new CooldownCondition(data.cooldown, source));
            }
            AddCondition(new PetOwnerInCombatCondition(source));

            object objAbsrobDmg;
            object objConvertDmg;
            object objTime;

            if(data.inputs.TryGetValue("absorb_dmg_pc", out objAbsrobDmg)) {
                m_AbsorbDmgPc = (float)objAbsrobDmg;
            }
            if(data.inputs.TryGetValue("convert_dmg_pc", out objConvertDmg)) {
                m_ConvertDmgPc = (float)objConvertDmg;
            }
            if(data.inputs.TryGetValue("time", out objTime)) {
                m_Time = (float)objTime;
            }
            m_BonusName1 = source.Id + "adp1";
            m_BonusName2 = source.Id + "adp2";
        }

        public override bool DoUse() {
            if (pet && pet.owner) {
                var ownerBonuses = pet.owner.Bonuses();

                if (!ownerBonuses.Contains(m_BonusName1)) {
                    Buff buffAbsorb = new Buff(m_BonusName1, null, Common.BonusType.absorb_damage_pc, m_Time, m_AbsorbDmgPc);
                    Buff convertBuff = new Buff(m_BonusName2, null, Common.BonusType.convert_absorbed_damage_to_hp_pc, m_Time, m_ConvertDmgPc);
                    ownerBonuses.SetBuff(buffAbsorb);
                    ownerBonuses.SetBuff(convertBuff);
                    return true;
                }
            }
            return false;
        }
    }
}
