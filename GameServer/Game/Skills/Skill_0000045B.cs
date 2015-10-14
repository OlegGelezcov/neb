using Nebula.Engine;
using Nebula.Game.Components;
using ServerClientCommon;
using System.Collections;

namespace Nebula.Game.Skills {
    public class Skill_0000045B : SkillExecutor  {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            if(!source) {
                return false;
            }
            var sourceWeapon = source.GetComponent<BaseWeapon>();
            info.Add((int)SPC.Distance, sourceWeapon.optimalDistance);
            return true;
        }
    }
}
