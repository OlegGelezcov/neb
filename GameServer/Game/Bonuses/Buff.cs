using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using Nebula.Engine;
using Nebula.Game.Components;
using ExitGames.Logging;

namespace Nebula.Game.Bonuses
{
    /// <summary>
    /// Abstract class for buffs
    /// </summary>
    public class Buff
    {
        private static readonly ILogger s_Log = LogManager.GetCurrentClassLogger();

        private float mValue;
        public string id { get; private set; }
        public Func<bool> customCheck { get; private set; }
        public BonusType buffType { get; private set; }
        
        public float interval { get; private set; }
        public bool valid { get; private set; }
        public NebulaObject owner { get; private set; }
        public int sourceSkillId { get; private set; }

        public float value { get; private set; }

        private float mTimer;
        private PlayerSkills cachedSkills;
        private Action mExpireAction;
        private Action mUpdateAction;

        //additional info at buff
        public int tag { get; private set; }

        private bool m_RequireSkill = true;

        //private int mStackCount;

        public Buff(string id, NebulaObject inOwner, BonusType inBuffType, 
            float inInterval = -1.0f, float inValue = 1.0f, 
            Func<bool> inCustomCheck = null, int inSourceSkillId = -1, 
            bool requireSkill = true)
        {
            this.id = id;
            buffType = inBuffType;
            value = inValue;
            interval = inInterval;
            customCheck = inCustomCheck;
            mTimer = inInterval;
            owner = inOwner;
            sourceSkillId = inSourceSkillId;

            //set valid at moment creation
            valid = true;
            // mStackCount = 1;
            m_RequireSkill = requireSkill;

            //if buff act indefinetly and object and skill not setted this error, impossible make check validity
            //indefinte buffs act only on self
            if (requireSkill) {
                if (interval <= 0) {
                    if (owner == null || (sourceSkillId == -1)) {
                        valid = false;
                        throw new Exception("For unlimited buffs must be setted owner and source skill");
                    }
                }

                if (!(sourceSkillId == -1) && owner != null) {
                    cachedSkills = owner.GetComponent<PlayerSkills>();
                    if (cachedSkills == null) {
                        //throw new Exception("Owner of buff must have component PlayerSKills");
                        valid = false;
                        s_Log.ErrorFormat("owner os this buff mus have component PlayerSkills: {0}", owner.ObjectString());
                    }
                }
            }
        }

        /// <summary>
        /// Set value of sum buffs
        /// </summary>
        /// <param name="count"></param>
        //public void SetStackCount(int count) {
        //    mStackCount = count;
        //}



        public void SetTag(int inTag) {
            tag = inTag;
        }
        public void SetExpireAction(Action eAction) {
            mExpireAction = eAction;
        }

        public void SetUpdateAction(Action uAction) {
            mUpdateAction = uAction;
        }

        public void SetValue(float newVal) {
            value = newVal;
        }

        public void ScaleInterval(float mult) {

            if(interval > 0 && valid ) {
                interval *= mult;
                mTimer *= mult;
            } 
        }

        public void MultInterval(float mult) {
            if(interval > 0 && valid ) {
                interval *= mult;
            }
        }

        public virtual void Update(float deltaTime) {
            if(!valid) {
                return;
            }

            if (m_RequireSkill) {
                //for timed buff update timer
                if (interval > 0) {
                    mTimer -= deltaTime;
                    if (mTimer <= 0f) {
                        valid = false;
                        if (mExpireAction != null) {
                            mExpireAction();
                        }
                        return;
                    }
                } else {
                    //for non timed persistent buff check owner and state of owner skill
                    if (!owner) {
                        valid = false;
                        return;
                    }
                    //check that skill present at owner
                    var skill = cachedSkills.GetSkillById(sourceSkillId);
                    if (skill == null) {
                        valid = false;
                        return;
                    }

                    //chect skill type
                    if (skill.data.Type != SkillType.Persistent) {
                        valid = false;
                        return;
                    }

                    //check skill is enabled
                    if (skill.isOn == false) {
                        valid = false;
                        return;
                    }

                }

                if (customCheck != null) {
                    if (!customCheck()) {
                        valid = false;
                        return;
                    }
                }

            } else {
                if(customCheck != null) {
                    if(!customCheck()) {
                        valid = false;
                        return;
                    }
                } else {
                    throw new Exception("Not required skill buff must have custom check function!!!");
                }
            }

            if(mUpdateAction != null) {
                mUpdateAction();
            }

        }
    }

    
}
