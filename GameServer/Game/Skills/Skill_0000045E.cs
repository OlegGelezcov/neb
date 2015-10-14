using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nebula.Engine;
using Nebula.Game.Components;
using Space.Game;
using GameMath;

namespace Nebula.Game.Skills {
    public class Skill_0000045E : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            if(ShotToEnemyRestricted(source, skill)) {
                return false;
            }

            var sourceWeapon = source.Weapon();
            var targetObject = source.Target().targetObject;

            float damageMult = skill.GetFloatInput("dmg_mult");
            WeaponHitInfo hit;
            var shot = sourceWeapon.Fire(out hit, skill.data.Id, damageMult);
            if(hit.hitAllowed) {
                Vector3 center = (source.transform.position + targetObject.transform.position) * 0.5f;
                float radius = source.transform.DistanceTo(targetObject.transform) * 0.5f;
                var skillComponent = source.GetComponent<Skill_0000045E_Component>();
                if(skillComponent == null) {
                    skillComponent = source.AddComponent<Skill_0000045E_Component>();
                }

                skillComponent.SetSkill(center, radius, skill.data);
                source.MmoMessage().SendShot(Common.EventReceiver.OwnerAndSubscriber, shot);
                return true;
            } else {
                source.MmoMessage().SendShot(Common.EventReceiver.ItemOwner, shot);
                return false;
            }
        }
    }
}
