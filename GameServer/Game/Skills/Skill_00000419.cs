using Nebula.Engine;
using Nebula.Game.Bonuses;
using Nebula.Game.Components;
using ServerClientCommon;
using System.Collections;

namespace Nebula.Game.Skills {
    public class Skill_00000419 : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            info.SetSkillUseState(Common.SkillUseState.normal);

            bool castOnTarget = true;
            if(source.Target().hasTarget) {
                if(FriendTargetInvalid(source)) {
                    castOnTarget = false;
                } else {
                    if(NotCheckDistance(source)) {
                        info.SetSkillUseState(Common.SkillUseState.tooFar);
                        return false;
                    }
                }
            } else {
                castOnTarget = false;
            }
            float time = skill.GetFloatInput("time");
            bool mastery = RollMastery(source);
            if(mastery ) {
                time *= 2;
                info.SetMastery(true);
            } else {
                info.SetMastery(false);
            }

            string id = GetBuffId(skill, source);
            if(castOnTarget) {
                var sourceTarget = source.Target();
                sourceTarget.targetObject.Bonuses().SetBuff(new Buff(id, null, Common.BonusType.block_debuff, time, 1), source);
                info.Add((int)SPC.Target, sourceTarget.targetId);
                info.Add((int)SPC.TargetType, sourceTarget.targetType);
            } else {
                source.Bonuses().SetBuff(new Buff(id, null, Common.BonusType.block_debuff, time, 1), source);
                info.Add((int)SPC.Target, source.Id);
                info.Add((int)SPC.TargetType, source.Type);
            }


            /*
            if(!CheckForShotFriend(source, skill)) {
                return false;
            }

            var targetBonuses = source.GetComponent<PlayerTarget>().targetObject.GetComponent<PlayerBonuses>();
            targetBonuses.RemoveBuffs(Common.BonusType.decrease_resist_on_cnt);
            targetBonuses.RemoveBuffs(Common.BonusType.decrease_resist_on_pc);*/

            return true;
        }
    }
}
