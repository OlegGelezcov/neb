using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nebula.Engine;
using Nebula.Game.Components;
using Nebula.Game.Bonuses;

namespace Nebula.Game.Skills {
    public class Skill_0000041C : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            info.SetSkillUseState(Common.SkillUseState.normal);
            float time = skill.GetFloatInput("time");
            float hpPc = skill.GetFloatInput("hp_pc");
            

            bool mastery = RollMastery(source);
            if(mastery) {
                time *= 2;
                hpPc *= 2;
                info.SetMastery(true);
            } else {
                info.SetMastery(false);
            }

            //source.Skills().Set41C(time, hpPc);
            source.Bonuses().SetBuff(new Buff(skill.idInt.ToString(), null, Common.BonusType.buff_41c, time, hpPc), source);
            return true;
        }

        
        public void Make(NebulaObject source, PlayerSkill skill, float hpPc) {

            float radius = skill.GetFloatInput("radius");
            /*
            var items = GetHealTargets(source, source, skill.GetFloatInput("radius"));
            float restoredHp = hpPc * source.Weapon().GetDamage(false).totalDamage;

            foreach(var pItem in items) {
                pItem.Value.Damagable().RestoreHealth(source, restoredHp);
            }*/

            var weapon = source.Weapon();
            float hp = weapon.GetDamage().totalDamage * hpPc;

            foreach(var pFriend in GetNearestFriends(source, radius)) {
                weapon.Heal(pFriend.Value, hp, skill.idInt, generateCrit: false);
            }
        }
    }
}
