﻿using Common;
using Nebula.Engine;
using Nebula.Game.Components;
using System.Collections;

namespace Nebula.Game.Skills {
    public class Skill_00000413 : SkillExecutor  {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            if(!source) {
                return false;
            }
            var damagable = source.GetComponent<DamagableObject>();
            if (!damagable) {
                return false;
            }

            float hpPc = skill.data.Inputs.Value<float>("hp_pc");
            float hp = hpPc * damagable.maximumHealth;
            //damagable.SetHealth(damagable.health + hp);
            bool mastery = RollMastery(source);
            if(mastery) {
                hp *= 2;
            }
            damagable.RestoreHealth(source, hp);
            return true;
        }
    }
}
