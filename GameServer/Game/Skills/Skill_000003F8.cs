using Nebula.Engine;
using Nebula.Game.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Game.Skills {
    public class Skill_000003F8 : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            float hpPc = skill.GetDataInput<float>("hp_pc", 0f);
            float hpTime = skill.GetDataInput<float>("hp_time", 0f);
            bool mastery = RollMastery(source);
            if(mastery) {
                hpTime *= 2;
            }
            source.Skills().Set3F8(hpTime, hpPc);
            return true;
        }
    }
}
