using Common;
using Nebula.Engine;
using Nebula.Game.Bonuses;
using Nebula.Game.Components;
using Space.Game;
using System.Collections;

namespace Nebula.Game.Skills {
    public class Skill_0000040C : SkillExecutor {
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

            var sourceWeapon = source.Weapon();

            float dmgMult = skill.GetFloatInput("dmg_mult");
            float blockTime = skill.GetFloatInput("block_time");


            bool mastery = RollMastery(source);
            if(mastery) {
                dmgMult *= 2;
                blockTime *= 2;
                info.SetMastery(true);
            } else {
                info.SetMastery(false);
            }

            WeaponHitInfo hit;
            var shotInfo = sourceWeapon.Fire(out hit, skill.data.Id, dmgMult);
            if(hit.normalOrMissed) {
                var targetObject = source.Target().targetObject;         
                Buff debuff = new Buff(skill.data.Id.ToString(), null, BonusType.block_skills, blockTime, 1.0f);
                targetObject.Bonuses().SetBuff(debuff);
                targetObject.Skills().Block(blockTime);
                source.GetComponent<MmoMessageComponent>().SendShot(EventReceiver.OwnerAndSubscriber, shotInfo);
                return true;
            }
            return false;
        }
    }
}
