using Common;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;
using ServerClientCommon;

namespace Nebula.Client {
    public class ResistDetail : IInfoParser{
        public int blockedMult { get; private set; }
        public float modelValue { get; private set; }
        public float bonusesAdd { get; private set; }
        public float passiveAdd { get; private set; }

        public void ParseInfo(Hashtable info) {
            blockedMult = info.GetValueInt((int)SPC.RESIST_Blocked);
            modelValue = info.GetValueFloat((int)SPC.RESIST_ModelValue);
            bonusesAdd = info.GetValueFloat((int)SPC.RESIST_BonusesAdd);
            passiveAdd = info.GetValueFloat((int)SPC.RESIST_PassiveBonusesAdd);
        }

        public ResistDetail() { }
        public ResistDetail(Hashtable hash) {
            ParseInfo(hash);
        }
    }
}
