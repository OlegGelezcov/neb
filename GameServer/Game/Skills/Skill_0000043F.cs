using ExitGames.Logging;
using Nebula.Engine;
using Nebula.Game.Components;
using Space.Server;
using System.Collections;

namespace Nebula.Game.Skills {
    public class Skill_0000043F : SkillExecutor {
        private static ILogger log = LogManager.GetCurrentClassLogger();

        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();

            log.InfoFormat("43F used..");
            var sourceWeapon = source.Weapon();
            var sourceCharacter = source.Character();

            var targetObject = source.Target().targetObject;
            if(!targetObject) {
                log.InfoFormat("no target...");
                return false;
            }

            var targetBonuses = targetObject.Bonuses();
            if(!targetBonuses) {
                log.InfoFormat("no bonuses on target...");
                return false;
            }

            var targetCharacter = targetObject.Character();
            if(!targetCharacter) {
                log.InfoFormat("no character on target...");
                return false;
            }

            var targetDamagable = targetObject.Damagable();
            if(!targetDamagable) {
                log.InfoFormat("no damagable on target...");
                return false;
            }

            var relation = sourceCharacter.RelationTo(targetCharacter);
            if(!(relation == Common.FractionRelation.Enemy || relation == Common.FractionRelation.Neutral )) {
                log.InfoFormat("relation invalid = {0}", relation);
                return false;
            }

            float odMult = skill.GetFloatInput("od_mult");

            float distanceToTarget = source.transform.DistanceTo(targetObject.transform);
            float limitDistance = sourceWeapon.optimalDistance * odMult;

            log.InfoFormat("distance to target = {0}, limit distance = {1}", distanceToTarget, limitDistance);
            if ( distanceToTarget > limitDistance ) {
                return false;
            }



            var direction = source.transform.DirectionTo(targetObject.transform) * sourceWeapon.optimalDistance * 0.05f;

            var newPos = source.transform.position + direction;

            if(targetObject.IsPlayer()) {
                targetObject.MmoMessage().SetPos(newPos);
            } else {
                (targetObject as Item).Move(new GameMath.Vector { X = newPos.X, Y = newPos.Y, Z = newPos.Z });
            }
            return true;
        }
    }
}
