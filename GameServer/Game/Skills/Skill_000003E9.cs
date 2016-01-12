using Common;
using ExitGames.Logging;
using GameMath;
using Nebula.Engine;
using Nebula.Game.Bonuses;
using Nebula.Game.Components;
using Space.Game;
using System.Collections;

namespace Nebula.Game.Skills {
    public class Skill_000003E9 : SkillExecutor {

        //private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {

            log.Info("USE 3E9");
            info = new Hashtable();
            float decreaseSpeedPercent = skill.data.Inputs.Value<float>("decrease_speed_pc");
            float dmgMult = skill.data.Inputs.Value<float>("dmg_mult");

            string id = source.Id + skill.data.Id;

            var sourceTarget = source.GetComponent<PlayerTarget>();
            if (!sourceTarget.hasTarget) {
                log.InfoFormat("Skill {0} error: source don't have target", skill.data.Id.ToString("X8"));
                return false;
            }
            if (!sourceTarget.targetObject) {
                log.InfoFormat("Skill {0} error: source target object invalid", skill.data.Id.ToString("X8"));
                return false;
            }
            var sourceWeapon = source.GetComponent<BaseWeapon>();
            if(!sourceWeapon) {
                log.InfoFormat("Skill {0} error: source don't has weapon", skill.data.Id.ToString("X8"));
                return false;
            }
            if( Mathf.Approximately(sourceWeapon.HitProbTo(sourceTarget.nebulaObject), 0f) ) {
                log.InfoFormat("Skill {0} error: hit prob is 0", skill.data.Id.ToString("X8"));
                return false;
            }

            var targetBonuses = sourceTarget.targetObject.GetComponent<PlayerBonuses>();
            if (!targetBonuses) {
                log.InfoFormat("Skill {0} error: target don't has Bonuses component", skill.data.Id.ToString("X8"));
                return false;
            }

            var sourceCharacter = source.GetComponent<CharacterObject>();
            var targetCharacter = sourceTarget.targetObject.GetComponent<CharacterObject>();
            if(!sourceTarget || !targetCharacter) {
                log.InfoFormat("Skill {0} error: source or target don't has character component", skill.data.Id.ToString("X8"));
                return false;
            }


            if (sourceCharacter.RelationTo(targetCharacter) == FractionRelation.Friend) {
                log.InfoFormat("Skill {0} error: source and target in friend fraction, source fraction = {1}, target fraction = {2}", 
                    skill.data.Id.ToString("X8"), (FractionType)(byte)sourceCharacter.fraction, (FractionType)(byte)targetCharacter.fraction);
                return false;
            }

            if(RollMastery(source)) {
                dmgMult *= 2;
            }

            WeaponHitInfo hit;
            var shotInfo = sourceWeapon.GetComponent<BaseWeapon>().Fire(out hit, skill.data.Id, dmgMult);
            if (hit.hitAllowed) {
                log.Info("HIT ALLOWED");
                float speedCount = source.GetComponent<MovableObject>().normalSpeed * decreaseSpeedPercent;
                Buff speedDebuff = new Buff(id, null, BonusType.decrease_speed_on_cnt, skill.data.Durability, speedCount);
                targetBonuses.SetBuff(speedDebuff);
                source.GetComponent<MmoMessageComponent>().SendShot(EventReceiver.OwnerAndSubscriber, shotInfo);
                return true;
            } else {
                log.Info("HIT NOT ALLOWED");
                source.GetComponent<MmoMessageComponent>().SendShot(EventReceiver.OwnerAndSubscriber, shotInfo);
                log.InfoFormat("Skill {0} error: hit to target not allowed", skill.data.Id.ToString("X8"));
                return false;
            }
        }
    }
}
