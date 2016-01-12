using Nebula.Engine;
using Nebula.Game.Components;
using System.Collections;

namespace Nebula.Game.Skills {
    public class Skill_00000406 : Skill_000003E8  {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            
            bool result =  base.TryCast(source, skill, out info);
            if(result ) {
                var damagable = source.Damagable();
                float hpPc = skill.GetFloatInput("hp_pc");
                bool mastery = RollMastery(source);
                if(mastery) {
                    hpPc *= 2;
                }
                damagable.RestoreHealth(source, damagable.maximumHealth * hpPc);
            }
            return result;
        }
    }
}
