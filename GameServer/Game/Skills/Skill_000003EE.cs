using Common;
using GameMath;
using Nebula.Engine;
using Nebula.Game.Bonuses;
using Nebula.Game.Components;
using Space.Game;
using System.Collections;

namespace Nebula.Game.Skills {
    public class Skill_000003EE : SkillExecutor{
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            if(!source) {
                log.InfoFormat("Skill {0} error: source invalid", skill.data.Id.ToString("X8"));
                return false;
            }

            float buffDamagePercent = skill.data.Inputs.Value<float>("b_dmg_pc");
            float buffDamageInterval = skill.data.Inputs.Value<float>("b_dmg_interval");
            float damageMult = skill.data.Inputs.Value<float>("dmg_mult");
            string id = source.Id + skill.data.Id;

            info.SetSkillUseState(SkillUseState.normal);
            if (!CheckForShotEnemy(source, skill)) {
                info.SetSkillUseState(SkillUseState.invalidTarget);
                return false;
            }
            if (NotCheckDistance(source)) {
                info.SetSkillUseState(SkillUseState.tooFar);
                return false;
            }

            bool mastery = RollMastery(source);
            if(mastery) {
                damageMult *= 2;
            }
            WeaponHitInfo hit;
            var shotInfo = source.Weapon().GetComponent<BaseWeapon>().Fire(out hit, skill.data.Id, damageMult);
            if(hit.normalOrMissed) {
                if(mastery) {
                    buffDamageInterval *= 2;
                }
                Buff damageBuff = new Buff(id, null, BonusType.increase_damage_on_pc, buffDamageInterval, buffDamagePercent);
                source.GetComponent<PlayerBonuses>().SetBuff(damageBuff);
                source.GetComponent<MmoMessageComponent>().SendShot(EventReceiver.OwnerAndSubscriber, shotInfo);
                return true;
            } else {
                source.GetComponent<MmoMessageComponent>().SendShot(EventReceiver.OwnerAndSubscriber, shotInfo);
                log.InfoFormat("Skill {0} error: hit to target not allowed", skill.data.Id.ToString("X8"));
                return false;
            }
        }
    }
}
