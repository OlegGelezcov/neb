﻿using System;
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


            info.SetSkillUseState(SkillUseState.normal);
            if (!CheckForShotEnemy(source, skill)) {
                info.SetSkillUseState(SkillUseState.invalidTarget);
                return false;
            }
            if (NotCheckDistance(source)) {
                info.SetSkillUseState(SkillUseState.tooFar);
                return false;
            }

            float speedTime = skill.data.Inputs.Value<float>("speed_time");
            bool mastery = RollMastery(source);
            if(mastery) {
                speedTime *= 2;
            }

            var movable = source.GetComponent<PlayerTarget>().targetObject.GetComponent<MovableObject>();
            if(!movable) {
                return false;
            }

            movable.SetStopTimer(speedTime);
            return true;
        }
    }
}
