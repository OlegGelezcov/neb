// Skill_00000434.cs
// Nebula
//
// Created by Oleg Zheleztsov on Monday, September 21, 2015 4:14:57 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
using Nebula.Engine;
using Nebula.Game.Components;
using System.Collections;

namespace Nebula.Game.Skills {
    public class Skill_00000434 : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            var damagable = source.Damagable();
            float hpPc = skill.GetFloatInput("hp_pc");

            float hpPcTimed = skill.GetFloatInput("hp_pc_timed");
            float hpTime = skill.GetFloatInput("hp_time");

            float restoreInstance = damagable.maximumHealth * hpPc;
            damagable.RestoreHealth(source, restoreInstance);

            float hpPerSec = damagable.maximumHealth * hpPcTimed / hpTime;
            damagable.SetRestoreHPPerSec(hpPerSec, hpTime);
            return true;
        }

    }
}
