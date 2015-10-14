// Skill_00000416.cs
// Nebula
//
// Created by Oleg Zheleztsov on Sunday, September 20, 2015 2:22:24 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
using Nebula.Engine;
using Nebula.Game.Components;
using System.Collections;

namespace Nebula.Game.Skills {

    //restore hp on me instant and on time
    public class Skill_00000416 : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            float hpPc = skill.GetFloatInput("hp_pc");
            float hpPcTimed = skill.GetFloatInput("hp_pc_timed");
            float hpTime = skill.GetFloatInput("hp_time");

            var damagable = source.Damagable();
            float maxHealth = damagable.maximumHealth;
            float hpInstance = hpPc * maxHealth;
            float hpRestorePerSec = hpPcTimed * maxHealth / hpTime;

            damagable.RestoreHealth(source, hpInstance);
            damagable.SetRestoreHPPerSec(hpRestorePerSec, hpTime);
            return true;
        }
    }
}
