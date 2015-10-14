using Common;
using ServerClientCommon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Client {
    public class HealDron {
        public string sourceID { get; private set; }
        public byte sourceType { get; private set; }
        public string targetID { get; private set; }
        public byte targetType { get; private set; }
        public float heal { get; private set; }

        public HealDron(string inSourceID, byte inSourceType, Hashtable info) {
            sourceID = inSourceID;
            sourceType = inSourceType;
            targetID = info.GetValue<string>((int)SPC.Target, string.Empty);
            targetType = info.GetValue<byte>((int)SPC.TargetType, 0);
            heal = info.GetValue<float>((int)SPC.HealValue, 0f);
        }
    }
}
