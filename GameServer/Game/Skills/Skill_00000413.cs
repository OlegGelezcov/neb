using Common;
using Nebula.Engine;
using Nebula.Game.Components;
using System.Collections;

namespace Nebula.Game.Skills {
    public class Skill_00000413 : SkillExecutor  {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            info.SetSkillUseState(SkillUseState.normal);

            if(!source) {
                return false;
            }
            var damagable = source.GetComponent<DamagableObject>();
            if (!damagable) {
                return false;
            }

            float hpPc = skill.GetFloatInput("hp_pc");
            float hp = hpPc * damagable.maximumHealth;
            //damagable.SetHealth(damagable.health + hp);
            bool mastery = RollMastery(source);
            if(mastery) {
                hp *= 2;
                info.SetMastery(true);
            } else {
                info.SetMastery(false);
            }
            //damagable.RestoreHealth(source, hp);
            //source.Weapon().HealSelf(hp, skill.idInt);
            source.MmoMessage().SendHeal(EventReceiver.OwnerAndSubscriber, source.Weapon().HealSelf(hp, skill.idInt));
            return true;
        }
    }
}
