using Nebula.Engine;
using Nebula.Game.Components;
using System;
using System.Collections;

namespace Nebula.Game.Skills {
    public class Skill_00000453 : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            return true;
        }
    }
}
