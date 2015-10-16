﻿using Nebula.Engine;
using Nebula.Game.Components;
using System.Collections;

namespace Nebula.Game.Skills {
    public class Skill_0000043B : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            var sourceTarget = source.Target();
            if(!sourceTarget.inCombat && sourceTarget.noSubscribers) {
                source.SetInvisibility(true);
                return true;
            }
            return false;
        }
    }
}