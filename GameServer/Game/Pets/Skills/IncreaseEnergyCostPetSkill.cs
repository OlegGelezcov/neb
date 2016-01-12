using Common;
using Nebula.Engine;
using Nebula.Game.Bonuses;
using Nebula.Game.Pets.Conditions;
using Nebula.Pets;
using ServerClientCommon;
using System.Collections;

namespace Nebula.Game.Pets.Skills {
    public class IncreaseEnergyCostPetSkill : PetSkill {

        private float m_EnergyPc;
        private float m_Time;
        private NebulaObject m_LastEnemy;
        private string m_BonusName;

        public IncreaseEnergyCostPetSkill(PetSkillInfo skill, NebulaObject source)
            : base(skill, source) {
            if (data.prob < 1f) {
                AddCondition(new ProbCondition(data.prob, source));
            }
            if (data.cooldown > 0) {
                AddCondition(new CooldownCondition(data.cooldown, source));
            }

            AddCondition(new PetOwnerEnergyLessThanTargetEnergyCondition(source));
            AddCondition(new PetOwnerInCombatCondition(source));

            object objEnergyPc;
            object objTime;
            if(skill.inputs.TryGetValue("energy_pc", out objEnergyPc)) {
                m_EnergyPc = (float)objEnergyPc;
            }
            if(skill.inputs.TryGetValue("time", out objTime)) {
                m_Time = (float)objTime;
            }
            m_BonusName = source.Id + "iecps";
        }

        public override bool DoUse() {
            if(pet && pet.owner) {
                var ownerTarget = pet.owner.Target();
                if(ownerTarget && ownerTarget.targetObject) {
                    var targetBonuses = ownerTarget.targetObject.Bonuses();
                    if(targetBonuses) {
                        Buff buff = new Buff(m_BonusName, null, Common.BonusType.increase_energy_cost_on_pc, m_Time, m_EnergyPc);
                        targetBonuses.SetBuff(buff);
                        m_LastEnemy = targetBonuses.nebulaObject;
                        return true;
                    }
                }
            }
            return false;
        }

        protected override Hashtable GetProperties() {
            Hashtable hash =  base.GetProperties();
            if (m_LastEnemy) {
                hash.Add((int)SPC.Target, m_LastEnemy.Id);
                hash.Add((int)SPC.TargetType, m_LastEnemy.Type);
            } else {
                hash.Add((int)SPC.Target, string.Empty);
                hash.Add((int)SPC.TargetType, (byte)ItemType.Bot);
            }
            return hash;
        }
    }
}
