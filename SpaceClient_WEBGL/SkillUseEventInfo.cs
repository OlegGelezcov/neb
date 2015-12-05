using Common;
using ServerClientCommon;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;

namespace Nebula.Client {
    public class SkillUseEventInfo : IInfoParser {

        public bool isOn { get; private set; }
        public SkillData data { get; private set; }
        public string sourceID { get; private set; }
        public ItemType sourceType { get; private set; }
        public string targetID { get; private set; }
        public ItemType targetType { get; private set; }
        public bool isSuccess { get; private set; }
        public string message { get; private set; }
        public Hashtable useInfo { get; private set; }

        public void ParseInfo(Hashtable info) {
            isOn = info.GetValueBool((int)SPC.IsOn);
            data = new SkillData(info.Value<Hashtable>((int)SPC.Data));
            sourceID = info.GetValueString((int)SPC.Source);
            sourceType = (ItemType)info.GetValueByte((int)SPC.SourceType);
            targetID = info.GetValueString((int)SPC.Target);
            targetType = (ItemType)info.GetValueByte((int)SPC.TargetType);
            isSuccess = info.GetValueBool((int)SPC.IsSuccess);
            message = info.GetValueString((int)SPC.Message);
            useInfo = info.GetValueHash((int)SPC.Info);
        }

        public object GetInfoValue(SPC key) {
            if (useInfo == null) {
                return 0f;
            }
            if (!useInfo.ContainsKey((int)key)) {
                return 0f;
            }
            return useInfo[(int)key];
        }

        public SkillUseEventInfo(Hashtable info) {
            ParseInfo(info);
        }
    }
}
