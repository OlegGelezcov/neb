using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nebula.Engine;
using Nebula.Game.Components;
using Common;
using Space.Game;
using GameMath;
using System.Collections;

namespace Nebula.Game.Skills {
    public class Skill_0000042A : SkillExecutor  {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            if(!CheckForShotEnemy(source, skill)) { return false; }
            float dmgMult = skill.data.Inputs.Value<float>("dmg_mult");
            float effectProb = skill.data.Inputs.Value<float>("effect_prob");
            var sourceWeapon = source.GetComponent<BaseWeapon>();
            var targetBonuses = source.GetComponent<PlayerTarget>().targetObject.GetComponent<PlayerBonuses>();

            bool mastery = RollMastery(source);
            if(mastery) {
                dmgMult *= 2;
                effectProb *= 2;
            }
            WeaponHitInfo hit;
            var shotInfo = sourceWeapon.Fire(out hit, skill.data.Id, dmgMult);
            if (hit.hitAllowed) {
                if(Rand.Float01() < effectProb ) {
                    BuffParameter prm = CommonUtils.GetRandomEnumValue<BuffParameter>(new List<BuffParameter>());
                    BonusType[] debuffs = BuffUtils.GetBuffsForParameter(prm);
                    foreach(var dbf in debuffs ) {
                        targetBonuses.RemoveBuffs(dbf);
                    }
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
