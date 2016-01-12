using Common;
using Nebula.Engine;
using Nebula.Game.Components;
using System.Collections;

namespace Nebula.Game.Skills {
    public class Skill_0000044E : SkillExecutor  {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            if(!source) {
                return false;
            }
            float hpPc = skill.data.Inputs.Value<float>("hp_pc");
            var damagable = source.GetComponent<DamagableObject>();
            float maxHP = damagable.maximumHealth;
            float restorHP = maxHP * hpPc;
            //damagable.SetHealth(damagable.health + restorHP);
            bool mastery = RollMastery(source);
            if(mastery) {
                restorHP *= 2;
            }

            damagable.RestoreHealth(source, restorHP);
            return true;
        }
    }
}
