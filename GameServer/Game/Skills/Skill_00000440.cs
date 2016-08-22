using Nebula.Engine;
using Nebula.Game.Bonuses;
using Nebula.Game.Components;
using System.Collections;

namespace Nebula.Game.Skills {

    /// <summary>
    /// Block resist on source and target
    /// </summary>
    public class Skill_00000440 : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            if(ShotToEnemyRestricted(source, skill)) {
                return false;
            }

            var targetObject = source.Target().targetObject;
            var sourceShip = source.GetComponent<BaseShip>();
            var targetShip = targetObject.GetComponent<BaseShip>();
            if((!sourceShip) || (!targetShip)) {
                return false;
            }

            var sourceBonuses = source.Bonuses();
            var targetBonuses = targetObject.Bonuses();

            float time = skill.GetFloatInput("time");

            bool mastery = RollMastery(source);
            if(mastery) {
                time *= 2;
            }

            //decrated skill do nothing (only for icons)
            Buff sourceDebuff = new Buff(skill.id, null, Common.BonusType.block_resist, time);
            sourceBonuses.SetBuff(sourceDebuff, source);
            sourceShip.BlockResist(time);

            //decorated skill do nothing (only for icons)
            Buff targetDebuff = new Buff(skill.id, null, Common.BonusType.block_resist, time);
            targetBonuses.SetBuff(targetDebuff, source);
            targetShip.BlockResist(time);
            return true;
        }
    }
}
