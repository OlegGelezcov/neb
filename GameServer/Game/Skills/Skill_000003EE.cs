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

            var sourceTarget = source.GetComponent<PlayerTarget>();
            var sourceCharacter = source.GetComponent<CharacterObject>();

            if(!sourceCharacter) {
                log.InfoFormat("Skill {0} error: source don;t has character", skill.data.Id.ToString("X8"));
                return false;
            }
            if(!sourceTarget.hasTarget) {
                log.InfoFormat("Skill {0} error: source don't have target", skill.data.Id.ToString("X8"));
                return false;
            }
            if(!sourceTarget.targetObject) {
                log.InfoFormat("Skill {0} error: source target object invalid", skill.data.Id.ToString("X8"));
                return false;
            }

            var sourceWeapon = source.GetComponent<BaseWeapon>();
            if (!sourceWeapon) {
                log.InfoFormat("Skill {0} error: source don't has weapon", skill.data.Id.ToString("X8"));
                return false;
            }

            var targetCharacter = sourceTarget.targetObject.GetComponent<CharacterObject>();
            if(!targetCharacter) {
                log.InfoFormat("Skill {0} error: target don;t has character", skill.data.Id.ToString("X8"));
                return false;
            }
            if(sourceCharacter.RelationTo(targetCharacter) == FractionRelation.Friend) {
                log.InfoFormat("Skill {0} error: source and target in friend fraction, source fraction = {1}, target fraction = {2}",
                    skill.data.Id.ToString("X8"), (FractionType)(byte)sourceCharacter.fraction, (FractionType)(byte)targetCharacter.fraction);
                return false;
            }

            if (Mathf.Approximately(sourceWeapon.HitProbTo(sourceTarget.nebulaObject), 0f)) {
                log.InfoFormat("Skill {0} error: hit prob is 0", skill.data.Id.ToString("X8"));
                return false;
            }

            bool mastery = RollMastery(source);
            if(mastery) {
                damageMult *= 2;
            }
            WeaponHitInfo hit;
            var shotInfo = sourceWeapon.GetComponent<BaseWeapon>().Fire(out hit, skill.data.Id, damageMult);
            if(hit.hitAllowed) {
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
