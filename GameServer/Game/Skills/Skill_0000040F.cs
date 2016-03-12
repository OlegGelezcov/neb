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
            if(!CheckForHealAlly(source)) {
                return false;
            }
            float healMult = skill.GetFloatInput("heal_mult");
            float resistPc = skill.GetFloatInput("resist_pc");
            float resistTime = skill.GetFloatInput("resist_time");


            float damage = source.Weapon().GetDamage(false).totalDamage;
            float healing = damage * healMult;

            bool mastery = RollMastery(source);
            if(mastery) {
                healing *= 2;
                resistTime *= 2;
            }

            var heal = source.Weapon().Heal(source.Target().targetObject, healing, skill.data.Id);
            source.MmoMessage().SendHeal(Common.EventReceiver.OwnerAndSubscriber, heal);
            Buff resistBuff = new Buff(skill.data.Id.ToString(), null, Common.BonusType.increase_resist_on_pc, resistTime, resistPc);
            source.Bonuses().SetBuff(resistBuff);
            return true;
        }
    }
}
