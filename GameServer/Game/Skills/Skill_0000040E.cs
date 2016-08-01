using Nebula.Engine;
using Nebula.Game.Components;
using System.Collections;

namespace Nebula.Game.Skills {
    //Heal target on 140% of damage
    public class Skill_0000040E : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            info.SetSkillUseState(Common.SkillUseState.normal);

            bool castOnTarget = true;
            if(source.Target().hasTarget ) {
                if (FriendTargetInvalid(source)) {
                    info.SetSkillUseState(Common.SkillUseState.invalidTarget);
                    //return false;
                    castOnTarget = false;
                } else {
                    if (NotCheckDistance(source)) {
                        info.SetSkillUseState(Common.SkillUseState.tooFar);
                        //return false;
                    }
                    castOnTarget = false;
                }
            } else {
                castOnTarget = false;
            }



            float healMult = skill.GetFloatInput("heal_mult");
            var weapon = source.Weapon();
            float damage = weapon.GetDamage().totalDamage;
            float healValue = damage * healMult;

            bool mastery = RollMastery(source);
            if(mastery) {
                healValue *= 2;
                info.SetMastery(true);
            } else {
                info.SetMastery(false);
            }

            if (castOnTarget) {
                //firtst heal friend
                var heal = source.Weapon().Heal(source.Target().targetObject, healValue, skill.data.Id);
                source.MmoMessage().SendHeal(Common.EventReceiver.OwnerAndSubscriber, heal);
            }

            //second heal self
            var selfHeal = source.Weapon().HealSelf(healValue, skill.idInt);
            source.MmoMessage().SendHeal(Common.EventReceiver.OwnerAndSubscriber, selfHeal);

            return true;
        }
    }
}
