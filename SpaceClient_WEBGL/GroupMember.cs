using Common;
using ServerClientCommon;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;

namespace Nebula.Client {
    public class GroupMember : IInfoParser {

        private const int INVALID_ICON = -1;

        public string login { get; private set; }
        public string gameRefID { get; private set; }
        public string characterID { get; private set; }
        public bool isLeader { get; private set; }
        public string worldID { get; private set; }
        public string characterName { get; private set; } = string.Empty;
        public int characterIcon { get; private set; } = INVALID_ICON;
        public int exp { get; private set; } = 0;
        public int workshop { get; private set; } = 0;

        public void ParseInfo(Hashtable info) {
            login = info.GetValueString((int)SPC.Login);
            gameRefID = info.GetValueString((int)SPC.GameRefId);
            characterID = info.GetValueString((int)SPC.CharacterId);
            isLeader = info.GetValueBool((int)SPC.IsLeader);
            worldID = info.GetValueString((int)SPC.WorldId);
            characterName = info.GetValueString((int)SPC.CharacterName);
            characterIcon = info.GetValueInt((int)SPC.Icon, INVALID_ICON);
            exp = info.GetValueInt((int)SPC.Exp, 0);
            workshop = info.GetValueInt((int)SPC.Workshop, 0);
        }

        public void SetLogin(string inLogin) {
            login = inLogin;
        }

        public GroupMember() { }
        public GroupMember(Hashtable info) { ParseInfo(info); }

        public override string ToString() {
            return string.Format("{0}, leader = {1}, world = {2}", login, isLeader, worldID);
        }

        public bool hasIcon {
            get {
                return (characterIcon >= 0);
            }
        }
    }
}
