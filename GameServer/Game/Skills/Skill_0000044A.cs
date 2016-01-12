using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nebula.Engine;
using Nebula.Game.Components;
using GameMath;
using Space.Game;
using Nebula.Game.Bonuses;

namespace Nebula.Game.Skills {
    public class Skill_0000044A : SkillExecutor {

        private int mUseCounter = 0;

        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            if(ShotToEnemyRestricted(source, skill)) {
                ResetCounter();
                return false;
            }

            var dmgMult = skill.GetFloatInput("dmg_mult");
            var hpPc = skill.GetFloatInput("hp_pc");
            var hpTime = skill.GetFloatInput("hp_time");

            mUseCounter++;
            if(source.Skills().lastSkill != skill.data.Id) {
                mUseCounter = 0;
            }

            mUseCounter = Mathf.Clamp(mUseCounter, 0, 2);
            dmgMult = ModifyDamageMult(dmgMult);

            bool mastery = RollMastery(source);
            if(mastery) {
                dmgMult *= 2;
                hpTime *= 2;
            }

            var sourceWeapon = source.Weapon();
            var targetObject = source.Target().targetObject;
            WeaponHitInfo hit;
            var shot = sourceWeapon.Fire(out hit, skill.data.Id, dmgMult);
            if(hit.hitAllowed) {
                Buff buff = new Buff(skill.id, null, Common.BonusType.increase_healing_speed_on_pc, hpTime, hpPc);
                source.Bonuses().SetBuff(buff);
                source.MmoMessage().SendShot(Common.EventReceiver.OwnerAndSubscriber, shot);
                return true;
            } else {
                source.MmoMessage().SendShot(Common.EventReceiver.ItemOwner, shot);
                return false;
            }
        }

        private float ModifyDamageMult(float inputDamage) {
            switch(mUseCounter) {
                case 1:
                    return inputDamage + 0.1f;
                case 2:
                    return inputDamage + 0.2f;
                default:
                    return inputDamage;
            }
        } 

        private void ResetCounter() {
            mUseCounter = 0;
        }
    }
}
