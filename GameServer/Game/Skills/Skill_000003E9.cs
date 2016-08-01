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

            //log.Info("USE 3E9");
            info = new Hashtable();
            float decreaseSpeedPercent = skill.data.Inputs.Value<float>("decrease_speed_pc");
            float dmgMult = skill.data.Inputs.Value<float>("dmg_mult");

            string id = source.Id + skill.data.Id;

            info.SetSkillUseState(SkillUseState.normal);
            if(!CheckForShotEnemy(source, skill)) {
                info.SetSkillUseState(SkillUseState.invalidTarget);
                return false;
            } 

            if(NotCheckDistance(source)) {
                info.SetSkillUseState(SkillUseState.tooFar);
                return false;
            }

            if(RollMastery(source)) {
                dmgMult *= 2;
            }

            WeaponHitInfo hit;
            var sourceWeapon = source.Weapon();
            var targetBonuses = source.Target().targetObject.Bonuses();

            var shotInfo = sourceWeapon.GetComponent<BaseWeapon>().Fire(out hit, skill.data.Id, dmgMult);
            if (hit.normalOrMissed) {
                //log.Info("HIT ALLOWED");
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
