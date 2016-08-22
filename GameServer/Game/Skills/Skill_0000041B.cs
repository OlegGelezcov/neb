using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nebula.Engine;
using Nebula.Game.Components;

namespace Nebula.Game.Skills {
    public class Skill_0000041B : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            info.SetSkillUseState(Common.SkillUseState.normal);

            bool castOnTarget = true;
            if(source.Target().hasTarget ) {
                if(FriendTargetInvalid(source)) {
                    castOnTarget = false;
                } else {
                    if(NotCheckDistance(source)) {
                        info.SetSkillUseState(Common.SkillUseState.tooFar);
                        return false;
                    }
                }
            } else {
                castOnTarget = false;
            }


            float dmgMult = skill.GetFloatInput("dmg_mult");
            var weapon = source.Weapon();
            float damage = weapon.GetDamage().totalDamage;
            float absorbedDamage = dmgMult * damage;

            //NebulaObject target = null;
            //if(CheckForHealAlly(source)) {
            //    target = source.Target().targetObject;
            //} else {
            //    target = source;
            //}

            bool mastery = RollMastery(source);
            if(mastery) {
                absorbedDamage *= 2;
                info.SetMastery(true);
            } else {
                info.SetMastery(false);
            }

            if(castOnTarget ) {
                source.Target().targetObject.Bonuses().SetAbsrobBuff(absorbedDamage, source);
            } else {
                source.Bonuses().SetAbsrobBuff(absorbedDamage, source);
            }

            //target.Damagable().SetAbsorbDamage(absorbedDamage);
            return true;
        }
    }
}
