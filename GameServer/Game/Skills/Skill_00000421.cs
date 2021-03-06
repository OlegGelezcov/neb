﻿// Skill_00000421.cs
// Nebula
//
// Created by Oleg Zheleztsov on Sunday, September 20, 2015 11:55:19 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
using Common;
using Nebula.Engine;
using Nebula.Game.Bonuses;
using Nebula.Game.Components;
using System.Collections;

namespace Nebula.Game.Skills {
    public class Skill_00000421 : SkillExecutor {

        //private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            info.SetSkillUseState(SkillUseState.normal);

            float pc = skill.GetFloatInput("hpdmg_pc");
            float time = skill.GetFloatInput("time");
            var bonuses = source.Bonuses();

            bool mastery = RollMastery(source);
            if (mastery) {
                time *= 2;
                pc *= 2;
                info.SetMastery(true);
            } else {
                info.SetMastery(false);
            }

            Buff damageBuff = new Buff(skill.data.Id.ToString(), null, Common.BonusType.increase_damage_on_pc, time, pc);
            Buff healingBuff = new Buff(skill.data.Id.ToString(), null, Common.BonusType.increase_healing_on_pc, time, pc);
            //log.InfoFormat("set damage and healing buff {0}:{1} green", pc, time);
            bonuses.SetBuff(damageBuff, source);
            bonuses.SetBuff(healingBuff, source);
            return true;
        }
    }
}
