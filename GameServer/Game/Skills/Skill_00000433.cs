// Skill_00000433.cs
// Nebula
//
// Created by Oleg Zheleztsov on Monday, September 21, 2015 4:09:20 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
using Nebula.Engine;
using Nebula.Game.Bonuses;
using Nebula.Game.Components;
using System.Collections;

namespace Nebula.Game.Skills {
    public class Skill_00000433 : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            float hpPc = skill.GetFloatInput("hp_pc");
            float hpTime = skill.GetFloatInput("hp_time");
            Buff buff = new Buff(skill.data.Id.ToString(), null, Common.BonusType.increase_healing_speed_on_pc, hpTime, hpPc);
            source.Bonuses().SetBuff(buff);
            return true;
        }
    }
}


