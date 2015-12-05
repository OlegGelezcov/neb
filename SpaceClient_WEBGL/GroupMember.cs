using Common;
using ServerClientCommon;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;

namespace Nebula.Client {
    public class GroupMember : IInfoParser {
        public string login { get; private set; }
        public string gameRefID { get; private set; }
        public string characterID { get; private set; }
        public bool isLeader { get; private set; }
        public string worldID { get; private set; }

        public void ParseInfo(Hashtable info) {
            login = info.GetValueString((int)SPC.Login);
            gameRefID = info.GetValueString((int)SPC.GameRefId);
            characterID = info.GetValueString((int)SPC.CharacterId);
            isLeader = info.GetValueBool((int)SPC.IsLeader);
            worldID = info.GetValueString((int)SPC.WorldId);
        }

        public void SetLogin(string inLogin) {
            login = inLogin;
        }

        public GroupMember() { }
        public GroupMember(Hashtable info) { ParseInfo(info); }

        public override string ToString() {
            return string.Format("{0}, leader = {1}, world = {2}", login, isLeader, worldID);
        }
    }
}
