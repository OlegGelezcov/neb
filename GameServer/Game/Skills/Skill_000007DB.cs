using Nebula.Engine;
using Nebula.Game.Components;
using System.Collections;

namespace Nebula.Game.Skills {

    /// <summary>
    /// Set to object in area buffs while objects in area
    /// </summary>
    public class Skill_000007DB : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();

            var skillComponent = source.GetComponent<Skill_000007DB_Component>();
            if(skillComponent) {
                skillComponent.SetSkill(skill.data);
            } else {
                skillComponent = source.AddComponent<Skill_000007DB_Component>();
                skillComponent.SetSkill(skill.data);
            }
            return true;
        }
    }
}
