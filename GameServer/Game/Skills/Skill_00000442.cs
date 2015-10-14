// Skill_00000442.cs
// Nebula
//
// Created by Oleg Zheleztsov on Tuesday, August 4, 2015 10:35:20 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
using Nebula.Engine;
using Nebula.Game.Components;
using System.Collections;

namespace Nebula.Game.Skills {
    /// <summary>
    /// Shot inflicts 100% damage (and spends less energy)
    /// </summary>
    public class Skill_00000442 : Skill_000003E8 {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            return base.TryCast(source, skill, out info);
        }
    }
}
