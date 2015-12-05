using Common;
using ServerClientCommon;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;

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
            targetID = info.GetValueString((int)SPC.Target);
            targetType = info.GetValueByte((int)SPC.TargetType);
            heal = info.GetValueFloat((int)SPC.HealValue);
        }
    }
}
