using Common;
using Nebula.Engine;
using Nebula.Game.Bonuses;
using Nebula.Game.Components;
using Space.Game;
using System.Collections;

namespace Nebula.Game.Skills {
    public class Skill_0000040C : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            if(!CheckForShotEnemy(source, skill)) {
                return false;
            }
            var sourceWeapon = source.GetComponent<BaseWeapon>();
            float dmgMult = skill.data.Inputs.Value<float>("dmg_mult");
            float speedPc = skill.data.Inputs.Value<float>("speed_pc");
            float speedTime = skill.data.Inputs.Value<float>("speed_time");

            bool mastery = RollMastery(source);
            if(mastery) {
                dmgMult *= 2;
                speedTime *= 2;
            }

            WeaponHitInfo hit;
            var shotInfo = sourceWeapon.Fire(out hit, skill.data.Id, dmgMult);
            if(hit.hitAllowed) {
                float speedDebuff = source.GetComponent<MovableObject>().normalSpeed * speedPc;
                Buff debuff = new Buff(skill.data.Id.ToString(), null, BonusType.decrease_speed_on_cnt, speedTime, speedDebuff);
                source.GetComponent<PlayerTarget>().targetObject.GetComponent<PlayerBonuses>().SetBuff(debuff);
                source.GetComponent<MmoMessageComponent>().SendShot(EventReceiver.OwnerAndSubscriber, shotInfo);
                return true;
            } else {
                source.GetComponent<MmoMessageComponent>().SendShot(EventReceiver.OwnerAndSubscriber, shotInfo);
                return false;
            }
        }
    }
}
