using Common;
using Nebula.Engine;
using Nebula.Game.Components;
using Space.Game;
using System.Collections;

namespace Nebula.Game.Skills {
    public class Skill_000003ED : SkillExecutor  {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            float dmgMult = skill.data.Inputs.Value<float>("dmg_mult");
            float dmgPerSecPC = skill.data.Inputs.Value<float>("dmg_per_sec_pc");
            float dmgPerSecTime = skill.data.Inputs.Value<float>("dmg_per_sec_time");

            string id = skill.data.Id.ToString();
            if (!CheckForShotEnemy(source, skill)) {
                return false;
            }
            
            var sourceWeapon = source.GetComponent<BaseWeapon>();

            WeaponHitInfo hit;
            var shotInfo = sourceWeapon.GetComponent<BaseWeapon>().Fire(out hit, skill.data.Id, dmgMult);
            if (hit.hitAllowed) {
                var targ = source.GetComponent<PlayerTarget>().targetObject;
                if(targ ) {
                    targ.GetComponent<DamagableObject>().SetTimedDamage(dmgPerSecTime, hit.ActualDamage * dmgPerSecPC);
                }
                source.GetComponent<MmoMessageComponent>().SendShot(EventReceiver.OwnerAndSubscriber, shotInfo);
                return true;
            } else {
                source.GetComponent<MmoMessageComponent>().SendShot(EventReceiver.OwnerAndSubscriber, shotInfo);
                return false;
            }
        }
    }
}
