using Space.Game.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nebula.Engine;
using Nebula.Game.Components;
using Common;
using GameMath;
using System.Collections;

namespace Nebula.Game.Skills {
    public class Skill_000003F5 : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            float hpPercent = skill.data.Inputs.Value<float>("hp_pc");
            string id = source.Id + skill.data.Id;

            if(!source) { return false; }

            var sourceDamagable = source.GetComponent<DamagableObject>();
            if(!sourceDamagable) { return false; }

            //sourceDamagable.SetHealth(Mathf.Clamp( sourceDamagable.health + sourceDamagable.baseMaximumHealth * hpPercent, 0f, sourceDamagable.maximumHealth));
            sourceDamagable.RestoreHealth(source, sourceDamagable.baseMaximumHealth * hpPercent);
            return true;
        }
    }
}
