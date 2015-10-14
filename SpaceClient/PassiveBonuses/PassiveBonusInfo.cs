using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using ServerClientCommon;

namespace Nebula.Client.PassiveBonuses {
    public class PassiveBonusInfo : IInfoParser {

        public PassiveBonusType type { get; private set; }
        public int  tier { get; private set; }
        public bool learningStarted { get; private set; }
        public int learnStartTime { get; private set; }
        public int learnEndTime { get; private set; }
        public float progress { get; private set; }

        public PassiveBonusInfo(Hashtable info) {
            ParseInfo(info);
        }


        public void ParseInfo(Hashtable info) {
            type = (PassiveBonusType)info.GetValue<int>((int)SPC.Type, 1);
            tier = info.GetValue<int>((int)SPC.Tier, 0);
            learningStarted = info.GetValue<bool>((int)SPC.LearningStarted, false);
            learnStartTime = info.GetValue<int>((int)SPC.LearnStartTime, 0);
            learnEndTime = info.GetValue<int>((int)SPC.LearnEndTime, 0);

            if(learningStarted) {
                progress = info.GetValue<float>((int)SPC.LearnProgress, 0);
            }
        }
    }
}
