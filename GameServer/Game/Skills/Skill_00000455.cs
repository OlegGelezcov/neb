using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nebula.Engine;
using Nebula.Game.Components;
using Common;
using Space.Game;
using GameMath;

namespace Nebula.Game.Skills {
    public class Skill_00000455 : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            if(!source) {
                return false;
            }

            int dronCount = skill.data.Inputs.Value<int>("dron_count");
            float dmgMult = skill.data.Inputs.Value<float>("dron_dmg_mult");

            var sourceCharacter = source.GetComponent<CharacterObject>();
            var sourceWeapon = source.GetComponent<BaseWeapon>();
            var sourceBonuses = source.Bonuses();


            var items = (source.world as MmoWorld).GetItems((item) => {
                var itemCharacter = item.GetComponent<CharacterObject>();
                var itemDamagable = item.GetComponent<DamagableObject>();
                if(itemCharacter && itemDamagable ) {
                    if(sourceCharacter.RelationTo(itemCharacter) == FractionRelation.Enemy ) {
                        if(sourceWeapon.HitProbTo(item) >= 0.01f ) {
                            return true;
                        }
                    }
                    
                }
                return false;
            });


            int itemCount = items.Count;
            if(itemCount == 0 ) {
                return false;
            }


            int counter = 0;

            foreach(var itemPair in items ) {
                if (counter >= Math.Min(dronCount, itemCount)) {
                    break;
                }

                WeaponHitInfo hit;
                dmgMult = dmgMult * (1f + sourceBonuses.dronStrengthPcBonus) + sourceBonuses.dronStrengthCntBonus;

                var shotInfo = sourceWeapon.Fire(itemPair.Value, out hit, skill.data.Id, dmgMult );
                if(hit.hitAllowed) {
                    source.GetComponent<MmoMessageComponent>().SendShot(EventReceiver.OwnerAndSubscriber, shotInfo);
                }
                counter++;
            }
            return true;
        }
    }
}
