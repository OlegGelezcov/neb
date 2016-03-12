using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nebula.Engine;
using Nebula.Game.Components;

namespace Nebula.Game.Skills {
    public class Skill_0000041C : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            float time = skill.GetFloatInput("time");
            float hpPc = skill.GetFloatInput("hp_pc");

            bool mastery = RollMastery(source);
            if(mastery) {
                time *= 2;
            }

            source.Skills().Set41C(time, hpPc);
            return true;
        }


        public void Make(NebulaObject source, PlayerSkill skill, float hpPc) {
            var items = GetHealTargets(source, source, skill.GetFloatInput("radius"));
            float restoredHp = hpPc * source.Weapon().GetDamage(false).totalDamage;

            foreach(var pItem in items) {
                pItem.Value.Damagable().RestoreHealth(source, restoredHp);
            }
        }
    }
}
