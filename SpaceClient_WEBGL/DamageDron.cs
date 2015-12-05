using Common;
using ServerClientCommon;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;

namespace Nebula.Client {
    public class DamageDron {
        public string sourceID { get; private set; }
        public byte sourceType { get; private set; }
        public string targetID { get; private set; }
        public byte targetType { get; private set; }
        public float damage { get; private set; }

        public DamageDron(string inSourceID, byte inSourceType, Hashtable info) {
            sourceID = inSourceID;
            sourceType = inSourceType;
            targetID = info.GetValueString((int)SPC.Target);
            targetType = info.GetValueByte((int)SPC.TargetType);
            damage = info.GetValueFloat((int)SPC.Damage);
        }
    }
}
