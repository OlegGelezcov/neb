using Nebula.Engine;
using Nebula.Game.Components;
using ServerClientCommon;
using System.Collections;

namespace Nebula.Game.Skills {
    public class Skill_00000404 : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            var targetObject = source.Target().targetObject;

            if(!targetObject) {
                return false;
            }

            float moveDmgPc = skill.GetFloatInput("move_dmg_pc");
            float time = skill.GetFloatInput("time");

            bool mastery = RollMastery(source);
            if(mastery) {
                time *= 2;
            }

            if (targetObject.IsPlayer() && targetObject.Raceable().race == source.Raceable().race) {
                if(source.transform.DistanceTo(targetObject.transform) <= source.Weapon().optimalDistance ) {
                    source.Skills().Set404(time, moveDmgPc);
                    info.Add((int)SPC.Id, targetObject.Id);
                    return true;
                }
            }
            return false;
        }
    }
}
