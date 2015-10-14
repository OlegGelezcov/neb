// Skill_00000430.cs
// Nebula
//
// Created by Oleg Zheleztsov on Tuesday, August 4, 2015 4:12:07 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
using Common;
using Nebula.Engine;
using Nebula.Game.Components;
using System.Collections;

namespace Nebula.Game.Skills {

    /// <summary>
    /// Instantly restores 12% HP
    /// </summary>
    public class Skill_00000430 : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();

            if(!source) { return false; }

            var damagable = source.GetComponent<DamagableObject>();
            float hpPc = skill.data.Inputs.Value<float>("hp_pc");
            float hpRestore = damagable.maximumHealth * hpPc;
            //damagable.SetHealth(damagable.health + hpRestore);
            damagable.RestoreHealth(source, hpRestore);
            return true;
        }
    }
}
