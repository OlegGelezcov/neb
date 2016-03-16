using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nebula.Engine;
using Nebula.Game.Components;
using GameMath;
using Nebula.Game.Bonuses;

namespace Nebula.Game.Skills {
    public class Skill_000003FE : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            var ship = source.PlayerShip();
            if(!ship) {
                return false;
            }

            float hpPc = skill.GetFloatInput("hp_pc");
            float maxResist = skill.GetFloatInput("max_resist");
            float speedPc = skill.GetFloatInput("speed_pc");
            float time = skill.GetFloatInput("time");

            var damagable = source.Damagable();

            var bonuses = source.Bonuses();

            bool mastery = RollMastery(source);
            if(mastery) {
                time *= 2;
            }

            if (damagable.health < damagable.maximumHealth * hpPc) {
                float currentResistance = ship.commonResist;
                float resistanceDifference = Mathf.ClampLess(maxResist - currentResistance, 0f);
                Buff buff = new Buff(skill.data.Id.ToString(), null, Common.BonusType.increase_resist_on_cnt, time, resistanceDifference);
                bonuses.SetBuff(buff);
            }

            Buff speedBuff = new Buff(skill.data.Id.ToString(), null, Common.BonusType.increase_speed_on_pc, time, speedPc);
            bonuses.SetBuff(speedBuff);
            return true;
        }
    }
}
