using Common;
using ServerClientCommon;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;

namespace Nebula.Client.Groups {
    public class FoundedMember : IInfoParser {
        public string characterID { get; private set; }
        public Workshop workshop { get; private set; }
        public int exp { get; private set; }

        public FoundedMember(string characterID, Hashtable info) {
            this.characterID = characterID;
            ParseInfo(info);
        }

        public void ParseInfo(Hashtable info) {
            workshop = (Workshop)(byte)info.GetValueInt((int)SPC.Workshop, (int)(byte)Workshop.Arlen);
            exp = info.GetValueInt((int)SPC.Exp);
        }
    }
}
