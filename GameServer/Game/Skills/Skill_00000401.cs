using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nebula.Engine;
using Nebula.Game.Components;
using Common;

namespace Nebula.Game.Skills {
    public class Skill_00000401 : SkillExecutor  {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            if (!source ) {
                return false;
            }

            if(!CheckForShotEnemy(source, skill)) {
                return false;
            }

            float speedTime = skill.data.Inputs.Value<float>("speed_time");

            var movable = source.GetComponent<PlayerTarget>().targetObject.GetComponent<MovableObject>();
            if(!movable) {
                return false;
            }

            movable.SetStopTimer(speedTime);
            return true;
        }
    }
}
