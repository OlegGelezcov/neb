// Skill_00000431.cs
// Nebula
//
// Created by Oleg Zheleztsov on Tuesday, August 4, 2015 4:22:14 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
using Nebula.Engine;
using Nebula.Game.Components;
using Common;
using System.Collections;

namespace Nebula.Game.Skills {
    /// <summary>
    /// Recovers 20% of HP's 8 seconds
    /// </summary>
    public class Skill_00000431 : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            
            if(!source) { return false; }
            float hpPc = skill.data.Inputs.Value<float>("hp_pc");
            float hpTime = skill.data.Inputs.Value<float>("hp_time");
            bool mastery = RollMastery(source);
            if(mastery) {
                hpTime *= 2;
            }

            string id = source.Id + skill.data.Id.ToString();
            var damagable = source.GetComponent<DamagableObject>();
            float hpRestore = damagable.maximumHealth * hpPc;
            damagable.SetRestoreHPPerSec(hpRestore / hpTime, hpTime, id);
            return true;
        }
    }
}
