using Common;
using Nebula.Engine;
using Nebula.Game.Components;
using Space.Game;
using Space.Server;
using System.Collections;

namespace Nebula.Game.Skills {
    public class Skill_0000042B : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();

            if (!CheckForShotEnemy(source, skill)) { return false; }
            float dmgMult = skill.data.Inputs.Value<float>("dmg_mult");
            float dmg2Mult = skill.data.Inputs.Value<float>("dmg2_mult");
            float radius = skill.data.Inputs.Value<float>("radius");

            var sourceWeapon = source.GetComponent<BaseWeapon>();
            var sourceChar = source.GetComponent<CharacterObject>();
            var targetBonuses = source.GetComponent<PlayerTarget>().targetObject.GetComponent<PlayerBonuses>();

            bool mastery = RollMastery(source);
            if(mastery) {
                dmgMult *= 2;
                dmg2Mult *= 2;
            }

            WeaponHitInfo hit;
            var shotInfo = sourceWeapon.Fire(out hit, skill.data.Id, dmgMult);
            if (hit.hitAllowed) {

                var items = (source.world as MmoWorld).GetItems((it) => {
                    if (it.GetComponent<DamagableObject>() && it.GetComponent<CharacterObject>()) {
                        if (it.Id != targetBonuses.nebulaObject.Id) {
                            if (sourceChar.RelationTo(it.GetComponent<CharacterObject>()) == FractionRelation.Enemy) {
                                return true;
                            }
                        }
                    }
                    return false;
                });

                float minDist = float.MaxValue;
                Item targetItem = null;
                foreach(var p in items) {
                    float cdist = p.Value.transform.DistanceTo(targetBonuses.transform);
                    if (cdist < minDist ) {
                        minDist = cdist;
                        targetItem = p.Value;
                    }
                }

                if(targetItem) {
                    float dmg = sourceWeapon.GetDamage(false) * dmg2Mult;
                    InputDamage inpDamage = new InputDamage(source, dmg);
                    targetItem.GetComponent<DamagableObject>().ReceiveDamage(inpDamage);
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
