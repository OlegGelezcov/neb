using Nebula.Engine;
using Nebula.Game.Components;
using System.Collections;

namespace Nebula.Game.Skills {
    //Heal target on 140% of damage
    public class Skill_0000040E : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            if(!CheckForHealAlly(source)) {
                return false;
            }

            float healMult = skill.GetFloatInput("heal_mult");
            var weapon = source.Weapon();
            float damage = weapon.GetDamage(false);
            float healValue = damage * healMult;

            var heal = source.Weapon().Heal(source.Target().targetObject, healValue, skill.data.Id);
            source.MmoMessage().SendHeal(Common.EventReceiver.OwnerAndSubscriber, heal);
            return true;
        }
    }
}
