using Common;
using ServerClientCommon;
using System.Collections;

namespace Nebula.Client {
    public class DamageDron {
        public string sourceID { get; private set; }
        public byte sourceType { get; private set; }
        public string targetID { get; private set; }
        public byte targetType { get; private set; }
        public float damage { get; private set; }

        public DamageDron(string inSourceID, byte inSourceType, Hashtable info ) {
            sourceID = inSourceID;
            sourceType = inSourceType;
            targetID = info.GetValue<string>((int)SPC.Target, string.Empty);
            targetType = info.GetValue<byte>((int)SPC.TargetType, 0);
            damage = info.GetValue<float>((int)SPC.Damage, 0f);
        }
    }
}
