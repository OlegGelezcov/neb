// Skill_00000414.cs
// Nebula
//
// Created by Oleg Zheleztsov on Sunday, September 20, 2015 12:17:39 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
using Nebula.Engine;
using Nebula.Game.Components;
using System.Collections;

namespace Nebula.Game.Skills {

    //Restore hp self and allies in radius for time
    public class Skill_00000414 : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();

            float hpPc = skill.GetFloatInput("hp_pc");
            float hpTime = skill.GetFloatInput("hp_time");
            float radius = skill.GetFloatInput("radius");
            var damagable = source.Damagable();
            float restoredHp = hpPc * damagable.maximumHealth;
            float restoredHpPerSec = restoredHp / hpTime;
            damagable.SetRestoreHPPerSec(restoredHpPerSec, hpTime);

            var items = GetHealTargets(source, source, radius);
            foreach(var pItem in items) {
                pItem.Value.Damagable().SetRestoreHPPerSec(restoredHpPerSec, hpTime);
            }
            return true;
        }
    }
}
