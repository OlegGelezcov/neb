// Skill_0000040F.cs
// Nebula
//
// Created by Oleg Zheleztsov on Sunday, September 20, 2015 7:01:06 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
using Nebula.Engine;
using Nebula.Game.Bonuses;
using Nebula.Game.Components;
using System.Collections;

namespace Nebula.Game.Skills {
    public class Skill_0000040F : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();

            info.SetSkillUseState(Common.SkillUseState.normal);

            bool castOnTarget = true;
            if (source.Target().hasTarget) {
                if (FriendTargetInvalid(source)) {
                    info.SetSkillUseState(Common.SkillUseState.invalidTarget);
                    //return false;
                    castOnTarget = false;
                } else {
                    if (NotCheckDistance(source)) {
                        info.SetSkillUseState(Common.SkillUseState.tooFar);
                        //return false;
                        castOnTarget = false;
                    }
                }
            } else {
                castOnTarget = false;
            }

            float healMult = skill.GetFloatInput("heal_mult");
            float resistPc = skill.GetFloatInput("resist_pc");
            float resistTime = skill.GetFloatInput("resist_time");


            float damage = source.Weapon().GetDamage().totalDamage;
            float healing = damage * healMult;

            bool mastery = RollMastery(source);
            if(mastery) {
                healing *= 2;
                resistTime *= 2;
                info.SetMastery(true);
            } else {
                info.SetMastery(false);
            }

            NebulaObject targetObject = null;
            if(castOnTarget) {
                targetObject = source.Target().targetObject;
                var heal = source.Weapon().Heal(targetObject, healing, skill.data.Id);
                source.MmoMessage().SendHeal(Common.EventReceiver.OwnerAndSubscriber, heal);
                Buff resistBuff = new Buff(skill.data.Id.ToString(), null, Common.BonusType.increase_resist_on_cnt, resistTime, resistPc);
                targetObject.Bonuses().SetBuff(resistBuff, source);
            }

            targetObject = source;
            var heal2 = source.Weapon().Heal(targetObject, healing, skill.data.Id);
            source.MmoMessage().SendHeal(Common.EventReceiver.OwnerAndSubscriber, heal2);
            Buff resistBuff2 = new Buff(skill.data.Id.ToString(), null, Common.BonusType.increase_resist_on_cnt, resistTime, resistPc);
            targetObject.Bonuses().SetBuff(resistBuff2, source);


            return true;
        }
    }
}
