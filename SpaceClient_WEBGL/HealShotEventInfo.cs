using Common;
using ServerClientCommon;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;

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
            sourceID = info.GetValueString((int)SPC.Source);
            sourceType = (ItemType)info.GetValueByte((int)SPC.SourceType, (byte)ItemType.Avatar);
            targetID = info.GetValueString((int)SPC.Target);
            targetType = (ItemType)info.GetValueByte((int)SPC.TargetType, (byte)ItemType.Avatar);
            sourceWorkshop = (Workshop)info.GetValueByte((int)SPC.Workshop, (byte)Workshop.Arlen);
            skillID = info.GetValueInt((int)SPC.Skill, -1);
            healValue = info.GetValueFloat((int)SPC.HealValue);
            healID = info.GetValueInt((int)SPC.ShotID, 1);
            //isCritical = info.GetValueBool((int)SPC.IsCritical);
            if(info.ContainsKey((int)SPC.Critical)) {
                isCritical = info.GetValueBool((int)SPC.Critical);
            }
        }

        public HealShotEventInfo(Hashtable info) {
            ParseInfo(info);
        }

        public HealShotEventInfo() { }
    }
}
