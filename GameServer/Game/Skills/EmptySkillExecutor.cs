using Nebula.Engine;
using Nebula.Game.Components;
using Nebula.Game.Skills;
using System.Collections;

namespace Nebula.Game.Skills {
    public class EmptySkillExecutor : SkillExecutor
    {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            info.SetSkillUseState(Common.SkillUseState.normal);
            return true;
        }
    }
}
