using Common;
using Nebula.Engine;
using Nebula.Game.Components;
using System.Collections;

namespace Nebula.Game.Skills {
    public class Skill_000003F6 : SkillExecutor  {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();

            float hpPc = skill.data.Inputs.GetValue<float>("hp_pc", 0f);
            float hpAlliesPc = skill.data.Inputs.GetValue<float>("hp_allies_pc", 0f);
            float hpRestoreTime = skill.data.Inputs.GetValue<float>("hp_pc_time", 0f);

            var selfDamagable = source.Damagable();
            var selfCharacter = source.Character();
            var selfWeapon = source.Weapon();

            float selfRestoreHP = selfDamagable.maximumHealth * hpPc;
            float alliesRestoreHP = selfDamagable.maximumHealth * hpAlliesPc;

            bool mastery = RollMastery(source);
            if(mastery) {
                selfRestoreHP *= 2;
                alliesRestoreHP *= 2;
            }
            selfDamagable.SetRestoreHPPerSec(selfRestoreHP / hpRestoreTime, hpRestoreTime, source.Id + skill.data.Id.ToString());

            var allies = source.mmoWorld().GetItems(ItemType.Avatar, (item) => {
                if (item.Id != source.Id) {
                    var character = item.Character();
                    var damagable = item.Damagable();
                    if (character && damagable) {
                        if (selfCharacter.RelationTo(character) == FractionRelation.Friend) {
                            if (item.transform.DistanceTo(source.transform) < selfWeapon.optimalDistance) {
                                return true;
                            }
                        }
                    }
                }
                return false;
            });

            foreach(var friend in allies) {
                friend.Value.Damagable().RestoreHealth(source, alliesRestoreHP);
            }
            return true;
        }
    }
}
