﻿// Skill_0000042F.cs
// Nebula
//
// Created by Oleg Zheleztsov on Monday, September 21, 2015 3:05:06 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
using Nebula.Engine;
using Nebula.Game.Bonuses;
using Nebula.Game.Components;
using Space.Game;
using System.Collections;

namespace Nebula.Game.Skills {
    public class Skill_0000042F : SkillExecutor {
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

            float dmgMult = skill.GetFloatInput("dmg_mult");
            float speedPc = skill.GetFloatInput("speed_pc");
            float speedTime = skill.GetFloatInput("speed_time");
            float radius = skill.GetFloatInput("radius");

            bool mastery = RollMastery(source);
            if(mastery) {
                dmgMult *= 2;
                speedTime *= 2;
            }
            var sourceWeapon = source.Weapon();
            var sourceBonuses = source.Bonuses();
            WeaponHitInfo hit;
            var shot = sourceWeapon.Fire(out hit, skill.data.Id, dmgMult);
            if(hit.normalOrMissed) {
                source.MmoMessage().SendShot(Common.EventReceiver.OwnerAndSubscriber, shot);
                Buff buff = new Buff(skill.data.Id.ToString(), null, Common.BonusType.increase_speed_on_pc, speedTime, speedPc);
                sourceBonuses.SetBuff(buff, source);

                foreach(var ally in GetHealTargets(source, source, radius)) {
                    Buff buff2 = new Buff(skill.data.Id.ToString(), null, Common.BonusType.increase_speed_on_pc, speedTime, speedPc);
                    ally.Value.Bonuses().SetBuff(buff2, source);
                }
                return true;
            } else {
                source.MmoMessage().SendShot(Common.EventReceiver.ItemOwner, shot);
                return false;
            }
        }
    }
}
