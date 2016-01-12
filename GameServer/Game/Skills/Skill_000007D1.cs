using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nebula.Engine;
using Nebula.Game.Components;
using Space.Game;
using Common;

namespace Nebula.Game.Skills {
    public class Skill_000007D1 : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            if(!CheckForShotEnemy(source, skill)) {
                return false;
            }

            var targ = source.Target().targetObject;
            var sourceCharacter = source.Character();
            var sourceWeapon = source.Weapon();
            var message = source.GetComponent<MmoMessageComponent>();

            float dmgMult = skill.GetFloatInput("dmg_mult");
            float radius = skill.GetFloatInput("radius");

            bool mastery = RollMastery(source);
            if(mastery) {
                dmgMult *= 2;
                radius *= 2;
            }

            WeaponHitInfo hit;
            var shot = sourceWeapon.Fire(out hit, skill.data.Id, dmgMult);

            BonusType[] speedDebuffs = BuffUtils.GetDebuffsForParameter(BuffParameter.speed);

            if(hit.hitAllowed) {
                message.SendShot(EventReceiver.OwnerAndSubscriber, shot);

                var items = source.mmoWorld().GetItems((item) => {
                    var itemBonuses = item.Bonuses();
                    var itemCharacter = item.Character();
                    var itemDamagable = item.Damagable();
                    bool allComponentPresent = itemBonuses && itemCharacter && itemDamagable;
                    if (allComponentPresent) {
                        if (item.Id != targ.Id) {
                            float distanceToTarg = targ.transform.DistanceTo(item.transform);
                            if (distanceToTarg <= radius) {
                                if (itemBonuses.ContainsAny(speedDebuffs)) {
                                    var relation = sourceCharacter.RelationTo(itemCharacter);
                                    if (relation == FractionRelation.Enemy || relation == FractionRelation.Neutral) {
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                    return false;
                });

                foreach(var pitem in items) {
                    WeaponHitInfo itemHit;
                    var itemShot = sourceWeapon.Fire(pitem.Value, out itemHit, skill.data.Id, dmgMult);
                    if(hit.hitAllowed) {
                        message.SendShot(EventReceiver.OwnerAndSubscriber, itemShot);
                    } else {
                        message.SendShot(EventReceiver.OwnerAndSubscriber, shot);
                    }
                }

                return true;

            } else {
                source.GetComponent<MmoMessageComponent>().SendShot(EventReceiver.OwnerAndSubscriber, shot);
                return false;
            }

        }
    }
}
