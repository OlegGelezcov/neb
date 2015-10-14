using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nebula.Engine;
using Nebula.Game.Components;

namespace Nebula.Game.Skills {
    public class Skill_0000043A : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();

            hpPc = skill.GetFloatInput("hp_pc");
            dmgMult = skill.GetFloatInput("dmg_mult");

            return true;
        }

        public float hpPc { get; private set; }
        public float dmgMult { get; private set; }
    }
}
