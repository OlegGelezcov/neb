using Common;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;
using ServerClientCommon;

namespace Nebula.Client.PassiveBonuses {
    public class PassiveBonusInfo : IInfoParser {

        public PassiveBonusType type { get; private set; }
        public int tier { get; private set; }
        public bool learningStarted { get; private set; }
        public int learnStartTime { get; private set; }
        public int learnEndTime { get; private set; }
        public float progress { get; private set; }

        public PassiveBonusInfo(Hashtable info) {
            ParseInfo(info);
        }


        public void ParseInfo(Hashtable info) {
            type = (PassiveBonusType)info.GetValueInt((int)SPC.Type, 1);
            tier = info.GetValueInt((int)SPC.Tier);
            learningStarted = info.GetValueBool((int)SPC.LearningStarted);
            learnStartTime = info.GetValueInt((int)SPC.LearnStartTime);
            learnEndTime = info.GetValueInt((int)SPC.LearnEndTime);

            if (learningStarted) {
                progress = info.GetValueFloat((int)SPC.LearnProgress);
            }
        }
    }
}
