using Common;
using GameMath;
using Nebula.Engine;
using Nebula.Game.Bonuses;
using Nebula.Game.Components;
using System.Collections;

namespace Nebula.Game.Skills {
    public class Skill_000007D0 : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {

            info = new Hashtable();

            if(!source) {
                return false;
            }

            var targ = source.GetComponent<PlayerTarget>();
            if(!targ) {
                return false;
            }

            if(!targ.targetObject) {
                return false;
            }

            var sourceWeapon = source.GetComponent<BaseWeapon>();

            if(!sourceWeapon) {
                return false;
            }

            float optimalDistance = sourceWeapon.optimalDistance;

            float realDist = source.transform.DistanceTo(targ.targetObject.transform);

            float startDistPc = skill.data.Inputs.Value<float>("start_dist_pc");
            float endDistPc = skill.data.Inputs.Value<float>("end_dist_pc");
            float dmgPc = skill.data.Inputs.Value<float>("dmg_pc");
            float dmgTime = skill.data.Inputs.Value<float>("dmg_time");


            if (realDist > startDistPc * optimalDistance) {
                return false;
            }

            if (realDist > optimalDistance * endDistPc) {
                Vector3 dir = (source.transform.position - targ.targetObject.transform.position).normalized;
                Vector3 offset = dir * optimalDistance * endDistPc;
                Vector3 finalPoint = targ.targetObject.transform.position + offset;
                source.GetComponent<MmoMessageComponent>().StartJumpToPosition(finalPoint);
            }

            Buff buff = new Buff(skill.data.Id.ToString(), null, BonusType.increase_damage_on_pc, dmgTime, dmgPc);
            source.GetComponent<PlayerBonuses>().SetBuff(buff);

            return true;
        }
    }
}
