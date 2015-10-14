using Nebula.Engine;
using Nebula.Game.Components;
using System.Collections;

namespace Nebula.Game.Skills {
    public class Skill_00000419 : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            if(!CheckForShotFriend(source, skill)) {
                return false;
            }

            var targetBonuses = source.GetComponent<PlayerTarget>().targetObject.GetComponent<PlayerBonuses>();
            targetBonuses.RemoveBuffs(Common.BonusType.decrease_resist_on_cnt);
            targetBonuses.RemoveBuffs(Common.BonusType.decrease_resist_on_pc);
            return true;
        }
    }
}
