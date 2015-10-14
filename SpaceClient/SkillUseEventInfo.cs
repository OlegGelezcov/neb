using Common;
using ServerClientCommon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            isOn = info.Value<bool>((int)SPC.IsOn);
            data = new SkillData(info.Value<Hashtable>((int)SPC.Data));
            sourceID = info.Value<string>((int)SPC.Source);
            sourceType = (ItemType)info.Value<byte>((int)SPC.SourceType);
            targetID = info.Value<string>((int)SPC.Target);
            targetType = (ItemType)info.Value<byte>((int)SPC.TargetType);
            isSuccess = info.Value<bool>((int)SPC.IsSuccess);
            message = info.Value<string>((int)SPC.Message);
            useInfo = info.Value<Hashtable>((int)SPC.Info);
        }

        public object GetInfoValue(SPC key) {
            if(useInfo == null ) {
                return 0f;
            }
            if(!useInfo.ContainsKey((int)key)) {
                return 0f;
            }
            return useInfo[(int)key];
        }

        public SkillUseEventInfo(Hashtable info) {
            ParseInfo(info);
        }
    }
}
