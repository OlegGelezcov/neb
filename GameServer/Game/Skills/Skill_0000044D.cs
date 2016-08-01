using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nebula.Engine;
using Nebula.Game.Components;
using System.Collections.Concurrent;
using Space.Server;
using Space.Game;
using Nebula.Drop;

namespace Nebula.Game.Skills {
    public class Skill_0000044D : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            var weapon = source.Weapon();

            var targets = GetTargets(source, source, weapon.optimalDistance);
            if(targets.Count == 0 ) {
                return false;
            }

            ConcurrentBag<Item> filteredTargets = new ConcurrentBag<Item>();
            foreach(var pItem in targets) {
                filteredTargets.Add(pItem.Value);
                if(filteredTargets.Count >= 3) {
                    break;
                }
            }

            float dmgMult = skill.GetFloatInput("dmg_mult");
            float dmgAreaMult = skill.GetFloatInput("dmg_area_mult");
            float radius = skill.GetFloatInput("radius");

            float damagePerTarget = dmgMult / filteredTargets.Count;
            var sourceMessage = source.MmoMessage();

            var sourceCharacter = source.Character();
            var sourceRace = source.Raceable();

            bool mastery = RollMastery(source);
            if(mastery) {
                damagePerTarget *= 2;
                dmgAreaMult *= 2;
            }

            foreach (var target in filteredTargets) {
                WeaponHitInfo hit;
                var shot = weapon.Fire(target, out hit, skill.data.Id, damagePerTarget);
                if(hit.normalOrMissed) {
                    sourceMessage.SendShot(Common.EventReceiver.OwnerAndSubscriber, shot);

                    var nearItems = GetTargets(source, target, radius);

                    WeaponDamage wd = weapon.GenerateDamage();
                    wd.Mult(dmgAreaMult);
                    InputDamage inpDamage = new InputDamage(source, wd);
                    foreach(var pNear in nearItems) {
                        if(NoId(filteredTargets, pNear.Key)) {
                            pNear.Value.Damagable().ReceiveDamage(inpDamage);
                        }
                    }

                } else {
                    sourceMessage.SendShot(Common.EventReceiver.ItemOwner, shot);
                }
            }
            return true;
        }

        private bool NoId(ConcurrentBag<Item> items, string id) {
            bool no = true;
            foreach(var it in items) {
                if(it.Id == id ) {
                    no = false;
                    break;
                }
            }
            return no;
        }
    }
}
