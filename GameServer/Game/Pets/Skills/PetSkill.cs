using Common;
using ExitGames.Logging;
using Nebula.Engine;
using Nebula.Game.Utils;
using Nebula.Pets;
using ServerClientCommon;
using System.Collections;
using System.Collections.Generic;

namespace Nebula.Game.Pets.Skills {
    public abstract class PetSkill  {

        private static readonly ILogger s_Log = LogManager.GetCurrentClassLogger();
        private readonly List<Condition> m_Conditions = new List<Condition>();
        private PetSkillInfo m_SkillInfo;
        private NebulaObject m_Source;
        private PetObject m_Pet;


        public PetSkill(PetSkillInfo skillInfo, NebulaObject source) {
            m_SkillInfo = skillInfo;
            m_Source = source;
            m_Pet = source.GetComponent<PetObject>();
        }

        protected void AddCondition(Condition condition) {
            m_Conditions.Add(condition);
        }

        public bool ConditionsValid() {
            bool valid = true;
            foreach(var condition in m_Conditions) {
                if(!condition.Check()) {
                    valid = false;
                    break;
                }
            }
            return valid;
        }

        private void ConditionsRenew() {
            foreach(var condition in m_Conditions) {
                condition.Renew();
            }
        }
        public bool Use() {
            if(ConditionsValid()) {
                bool success =  DoUse();
                if(success) {
                    ConditionsRenew();
                    pet.SendPetSkill(GetProperties());
                    s_Log.InfoFormat("pet skill: {0} use success".Color(LogColor.orange), id);
                }
                return success;
            }
            return false;
        }

        public virtual bool UseExplicit(NebulaObject source) {
            return Use();
        }

        public abstract bool DoUse();

        public int id {
            get {
                if(m_SkillInfo != null ) {
                    return m_SkillInfo.id;
                }
                return 0;
            }
        }

        public PetSkillInfo data {
            get {
                return m_SkillInfo;
            }
        }

        protected NebulaObject source {
            get {
                return m_Source;
            }
        }

        protected PetObject pet {
            get {
                return m_Pet;
            }
        }

        protected virtual Hashtable GetProperties() {
            return new Hashtable {
                {(int)SPC.Source,  source.Id },
                {(int)SPC.SourceType, source.Type },
                {(int)SPC.Skill, id }
            };
        }
    }
}
