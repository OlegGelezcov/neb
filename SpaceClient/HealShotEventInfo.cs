using Common;
using ServerClientCommon;
using System.Collections;

namespace Nebula.Client {
    public class HealShotEventInfo : IInfoParser {

        public string sourceID { get; private set; }
        public ItemType sourceType { get; private set; }
        public string targetID { get; private set; }
        public ItemType targetType { get; private set; }
        public Workshop sourceWorkshop { get; private set; }
        public int skillID { get; private set; }
        public float healValue { get; private set; }
        public int healID { get; private set; }
        public bool isCritical { get; private set; }

        public void ParseInfo(Hashtable info) {
            sourceID = info.GetValue<string>((int)SPC.Source, string.Empty);
            sourceType = (ItemType)info.GetValue<byte>((int)SPC.SourceType, (byte)ItemType.Avatar);
            targetID = info.GetValue<string>((int)SPC.Target, string.Empty);
            targetType = (ItemType)info.GetValue<byte>((int)SPC.TargetType, (byte)ItemType.Avatar);
            sourceWorkshop = (Workshop)info.GetValue<byte>((int)SPC.Workshop, (byte)Workshop.Arlen);
            skillID = info.GetValue<int>((int)SPC.Skill, -1);
            healValue = info.GetValue<float>((int)SPC.HealValue, 0f);
            healID = info.GetValue<int>((int)SPC.ShotID, 1);
            isCritical = info.GetValue<bool>((int)SPC.IsCritical, false);
        }

        public HealShotEventInfo(Hashtable info) {
            ParseInfo(info);
        }

        public HealShotEventInfo() { }
    }
}
