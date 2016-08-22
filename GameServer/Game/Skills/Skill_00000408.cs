using Common;
using Nebula.Engine;
using Nebula.Game.Bonuses;
using Nebula.Game.Components;
using Space.Game;
using System.Collections;

namespace Nebula.Game.Skills {
    public class Skill_00000408 : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            info.SetSkillUseState(SkillUseState.normal);
            if (ShotToEnemyRestricted(source, skill)) {
                info.SetSkillUseState(SkillUseState.invalidTarget);
                return false;
            }
            if (NotCheckDistance(source)) {
                info.SetSkillUseState(SkillUseState.tooFar);
                return false;
            }

            float dmgMult = skill.GetFloatInput("dmg_mult");
            float dmgPc = skill.GetFloatInput("dmg_pc");
            float dmgTime = skill.GetFloatInput("time");

            bool mastery = RollMastery(source);
            if(mastery) {
                dmgPc *= 2;
                dmgTime *= 2;
                info.SetMastery(true);
            } else {
                info.SetMastery(false);
            }

            WeaponHitInfo hit;
            var shot = source.Weapon().Fire(out hit, skill.data.Id, dmgMult);
            if(hit.normalOrMissed) {
                Buff critChanceDebuff = new Buff(skill.data.Id.ToString(), null, Common.BonusType.decrease_damage_on_pc, dmgTime, dmgPc);
                source.Target().targetObject.Bonuses().SetBuff(critChanceDebuff, source);
                source.MmoMessage().SendShot(Common.EventReceiver.OwnerAndSubscriber, shot);
                return true;
            }
            return false;
        }
    }
}
