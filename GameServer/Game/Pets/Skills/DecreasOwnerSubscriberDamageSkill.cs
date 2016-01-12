using System.Collections;
using Nebula.Engine;
using Nebula.Game.Bonuses;
using Nebula.Game.Components;
using Nebula.Game.Pets.Conditions;
using Nebula.Pets;
using ServerClientCommon;
using Common;
using ExitGames.Logging;
using Nebula.Game.Utils;

namespace Nebula.Game.Pets.Skills {
    public class DecreasOwnerSubscriberDamageSkill : PetSkill {
        private static readonly ILogger s_Log = LogManager.GetCurrentClassLogger();

        private float m_Value;
        private float m_Time;
        private NebulaObject m_LastEnemy;
        private string m_BonusName;


        public DecreasOwnerSubscriberDamageSkill(PetSkillInfo skill, NebulaObject source) 
            : base(skill, source) {
            if(data.prob < 1f ) {
                AddCondition(new ProbCondition(data.prob, source));
            }
            if(data.cooldown > 0 ) {
                AddCondition(new CooldownCondition(data.cooldown, source));
            }
            AddCondition(new PetOwnerInCombatCondition(source));
            AddCondition(new PetOwnerHasSubscriberCondition(source));

            object valObj = null;
            object timeObj = null;
            if(data.inputs.TryGetValue("value", out valObj)) {
                m_Value = (float)valObj;
            }
            if(data.inputs.TryGetValue("time", out timeObj)) {
                m_Time = (float)timeObj;
            }
            m_BonusName = source.Id + "dosds";
        }

        public override bool DoUse() {
            PlayerBonuses bonuses = null;
            if( pet ) {
                if(pet.owner) {
                    var ownerTarget = pet.owner.Target();
                    if(ownerTarget.targetIsEnemySubscriber) {
                        bonuses = ownerTarget.targetObject.Bonuses();
                    } else {
                        var anySubscriber = ownerTarget.anyEnemySubscriber;
                        if(anySubscriber) {
                            bonuses = anySubscriber.Bonuses();
                        }
                    }
                }
            }

            if(bonuses) {
                Buff buff = new Buff(m_BonusName, null, Common.BonusType.decrease_damage_on_pc, m_Time, m_Value);
                bonuses.SetBuff(buff);
                m_LastEnemy = bonuses.nebulaObject;
                s_Log.InfoFormat("used pet skill 3 and set buff (decrease_damage_on_pc) on object = {0}:{1}".Color(LogColor.white), (ItemType)m_LastEnemy.Type, m_LastEnemy.Id);
                return true;
            }
            return false;
        }


        protected override Hashtable GetProperties() {
            Hashtable hash = base.GetProperties();
            if(m_LastEnemy ) {
                hash.Add((int)SPC.Target, m_LastEnemy.Id);
                hash.Add((int)SPC.TargetType, m_LastEnemy.Type);
            } else {
                hash.Add((int)SPC.Target, string.Empty);
                hash.Add((int)SPC.TargetType, (byte)ItemType.Bot);
            }
            return hash;
        }
    }
}
