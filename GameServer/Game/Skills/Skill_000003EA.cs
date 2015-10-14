using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nebula.Engine;
using Nebula.Game.Components;
using Common;
using Space.Game;
using Nebula.Game.Bonuses;
using System.Collections;

namespace Nebula.Game.Skills {
    public class Skill_000003EA : SkillExecutor {

        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();

            float dmgMult = skill.data.Inputs.Value<float>("dmg_mult");
            float hpRestorPc = skill.data.Inputs.Value<float>("hp_restor_pc");
            float hpRestorDur = skill.data.Inputs.Value<float>("hp_restor_dur");

            string id = source.Id +  skill.data.Id.ToString();

            if(!CheckForShotEnemy(source, skill)) {
                return false;
            }

            var sourceWeapon = source.GetComponent<BaseWeapon>();

            WeaponHitInfo hit;
            var shotInfo = sourceWeapon.GetComponent<BaseWeapon>().Fire(out hit, skill.data.Id, dmgMult);
            if(hit.hitAllowed) {
                //Buff hpRestorBuff = new Buff(id, null, BonusType.restore_hp_on_pc_for_sec, hpRestorDur, hpRestorPc);
                //source.GetComponent<PlayerBonuses>().SetBuff(hpRestorBuff);
                var damagable = source.GetComponent<DamagableObject>();
                damagable.SetRestoreHPPerSec(hpRestorPc * damagable.maximumHealth, hpRestorDur);

                source.GetComponent<MmoMessageComponent>().SendShot(EventReceiver.OwnerAndSubscriber, shotInfo);
                return true;
            } else {
                source.GetComponent<MmoMessageComponent>().SendShot(EventReceiver.OwnerAndSubscriber, shotInfo);
                return false;
            }
        }
    }
}
