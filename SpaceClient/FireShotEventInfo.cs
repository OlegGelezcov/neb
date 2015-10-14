using Common;
using ServerClientCommon;
using System.Collections;

namespace Nebula.Client {
    public class FireShotEventInfo : IInfoParser{
        public string sourceID { get; private set; }
        public ItemType sourceType { get; private set; }
        public string targetID { get; private set; }
        public ItemType targetType { get; private set; }
        public Workshop sourceWorkshop { get; private set; }
        public int skillID { get; private set; }
        public bool isHitAllowed { get; private set; }
        public bool isHitted { get; private set; }
        public float damage { get; private set; }
        public bool isCritical { get; private set; }
        public string errorMessage { get; private set; }

        public void ParseInfo(Hashtable info) {
            sourceID = info.GetValue<string>((int)SPC.Source, string.Empty);
            sourceType = (ItemType)info.GetValue<byte>((int)SPC.SourceType, (byte)ItemType.Avatar);
            targetID = info.GetValue<string>((int)SPC.Target, string.Empty);
            targetType = (ItemType)info.GetValue<byte>((int)SPC.TargetType, (byte)ItemType.Avatar);
            sourceWorkshop = (Workshop)info.GetValue<byte>((int)SPC.Workshop, (byte)Workshop.Arlen);
            skillID = info.GetValue<int>((int)SPC.Skill, -1);
            isHitAllowed = info.GetValue<bool>((int)SPC.FireAllowed, false);
            isHitted = info.GetValue<bool>((int)SPC.IsHitted, false);
            damage = info.GetValue<float>((int)SPC.ActualDamage, 0f);
            isCritical = info.GetValue<bool>((int)SPC.IsCritical, false);
            errorMessage = info.GetValue<string>((int)SPC.ErrorMessageId, string.Empty);
        }

        public FireShotEventInfo(Hashtable info) {
            ParseInfo(info);
        }
    }
}
