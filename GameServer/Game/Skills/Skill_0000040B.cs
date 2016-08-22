using Nebula.Engine;
using Nebula.Game.Bonuses;
using Nebula.Game.Components;
using Space.Game;
using System.Collections;

namespace Nebula.Game.Skills {
    public class Skill_0000040B : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            info.SetSkillUseState(Common.SkillUseState.normal);

            if(ShotToEnemyRestricted(source, skill)) {
                info.SetSkillUseState(Common.SkillUseState.invalidTarget);
                return false;
            }

            if(NotCheckDistance(source)) {
                info.SetSkillUseState(Common.SkillUseState.tooFar);
                return false;
            }

            float dmgMult =     skill.GetFloatInput("dmg_mult");
            float hpPc =        skill.GetFloatInput("hp_pc");
            float radius =      skill.GetFloatInput("radius");

            bool mastery = RollMastery(source);
            if(mastery) {
                dmgMult *= 2;
                hpPc *= 2;
                info.SetMastery(true);
            } else {
                info.SetMastery(false);
            }

            float hpVal = source.Damagable().maximumHealth * hpPc;

            WeaponHitInfo hit;
            var shot = source.Weapon().Fire(out hit, skill.data.Id, dmgMult);
            if(hit.normalOrMissed) {
                //Buff speedBuff = new Buff(skill.data.Id.ToString(), null, Common.BonusType.increase_speed_on_pc, speedTime, speedPc);
                //source.Bonuses().SetBuff(speedBuff, source);
                source.MmoMessage().SendShot(Common.EventReceiver.OwnerAndSubscriber, shot);

                var sWeapon = source.Weapon();
                var mmoMsg = source.MmoMessage();

                foreach(var friend in GetNearestFriends(source, radius)) {
                    var heal = sWeapon.Heal(friend.Value, hpVal, skill.idInt);
                    mmoMsg.SendHeal(Common.EventReceiver.OwnerAndSubscriber, heal);
                }
                return true;
            }
            return false;
        }
    }
}
