using ExitGames.Logging;
using Nebula.Engine;
using Nebula.Game.Bonuses;
using Nebula.Game.Pets.Conditions;
using Nebula.Pets;

namespace Nebula.Game.Pets.Skills {
    public class AdditionalDamagePetSkill : OwnerTargetedPetSkill {

        private static readonly ILogger s_Log = LogManager.GetCurrentClassLogger();
        private string m_BuffName;
        private float m_HpPc;

        public AdditionalDamagePetSkill(PetSkillInfo skillInfo, NebulaObject source)
            : base(skillInfo, source) {

            if (data.prob < 1f) {
                AddCondition(new ProbCondition(data.prob, source));
            }
            if (data.cooldown > 0) {
                AddCondition(new CooldownCondition(data.cooldown, source));
            }
            AddCondition(new PetOwnerInCombatCondition(source));

            object objHpPc;
            if(data.inputs.TryGetValue("value", out objHpPc)) {
                m_HpPc = (float)objHpPc;
            }
            m_BuffName = source.Id + "adps";
        }

        public override bool DoUse() {
            if(pet && pet.owner) {
                var ownerDamagable = pet.owner.Damagable();
                float dmg = m_HpPc * ownerDamagable.baseMaximumHealth;
                var ownerWeapon = pet.owner.Weapon();
                int shotCounter = ownerWeapon.notResettableShotCounter;
                var ownerBonuses = pet.owner.Bonuses();

                if (false == ownerBonuses.Contains(m_BuffName)) {
                    Buff buff = new Buff(m_BuffName, null, Common.BonusType.increase_damage_on_cnt, -1, dmg, () => {
                        if (ownerWeapon) {
                            if (ownerWeapon.notResettableShotCounter < shotCounter + 3) {
                                return true;
                            }
                        }
                        s_Log.InfoFormat("additional damage pet skill check made failed");
                        return false;
                    }, -1, false);
                    ownerBonuses.SetBuff(buff);
                    return true;
                }
            }
            return false;
        }
    }
}
