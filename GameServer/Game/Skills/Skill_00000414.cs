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
            info.SetSkillUseState(Common.SkillUseState.normal);

            float hpPc = skill.GetFloatInput("hp_pc");
            float hpTime = skill.GetFloatInput("hp_time");
            float radius = skill.GetFloatInput("radius");

            var damagable = source.Damagable();

            float restoredHp = hpPc * damagable.maximumHealth;

            float restoredHpPerSec = restoredHp / hpTime;

            string id = source.Id + skill.data.Id.ToString();

            bool mastery = RollMastery(source);
            if(mastery) {
                hpTime *= 2;
                restoredHpPerSec *= 2;
                info.SetMastery(true);
            } else {
                info.SetMastery(false);
            }

            //damagable.SetRestoreHPPerSec(restoredHpPerSec, hpTime, id);

            foreach(var item in GetNearestFriends(source, radius)) {
                item.Value.Damagable().SetRestoreHPPerSec(restoredHpPerSec, hpTime, id);
            }

            return true;
        }
    }
}
