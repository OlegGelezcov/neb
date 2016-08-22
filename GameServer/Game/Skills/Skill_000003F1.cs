using Common;
using Nebula.Engine;
using Nebula.Game.Bonuses;
using Nebula.Game.Components;
using Space.Game;
using System.Collections;

namespace Nebula.Game.Skills {
    public class Skill_000003F1 : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            info.SetSkillUseState(SkillUseState.normal);
            if (!CheckForShotEnemy(source, skill)) {
                info.SetSkillUseState(SkillUseState.invalidTarget);
                return false;
            }
            if (NotCheckDistance(source)) {
                info.SetSkillUseState(SkillUseState.tooFar);
                return false;
            }
            float dmgMult = skill.data.Inputs.GetValue<float>("dmg_mult", 0f);
            float blockHealInterval = skill.data.Inputs.GetValue<float>("block_heal_interval", 0f);
            var weapon = source.GetComponent<BaseWeapon>();
            WeaponHitInfo hit;

            bool mastery = RollMastery(source);
            if(mastery) {
                dmgMult *= 2;
            }
            var shot = weapon.Fire(out hit, skill.data.Id, dmgMult);
            if (hit.normalOrMissed) {
                if(mastery) {
                    blockHealInterval *= 2;
                }
                Buff buff = new Buff(skill.data.Id.ToString(), null, BonusType.block_heal, blockHealInterval);
                source.GetComponent<PlayerTarget>().targetObject.GetComponent<PlayerBonuses>().SetBuff(buff, source);
                source.GetComponent<MmoMessageComponent>().SendShot(EventReceiver.OwnerAndSubscriber, shot);
                return true;
            } else {
                source.GetComponent<MmoMessageComponent>().SendShot(EventReceiver.OwnerAndSubscriber, shot);
                return false;
            }
        }
    }
}
