using Common;
using ServerClientCommon;
using System.Collections;

namespace NebulaCommon.Group {
    public class GroupMember : IInfo {
        public string login { get; set; }
        public string gameRefID { get; set; }
        public string characterID { get; set; }
        public bool isLeader { get; set; }
        public string worldID { get; set; }
        public int workshop { get; set; }
        public int exp { get; set; }
        public string characterName { get; set; } = string.Empty;
        public int characterIcon { get; set; } = -1;

        public Hashtable GetInfo() {
            return new Hashtable {
                { (int)SPC.Login, login },
                { (int)SPC.GameRefId, gameRefID },
                { (int)SPC.CharacterId, characterID },
                { (int)SPC.IsLeader, isLeader },
                { (int)SPC.WorldId, worldID },
                { (int)SPC.Workshop, workshop },
                { (int)SPC.Exp, exp },
                { (int)SPC.CharacterName, characterName },
                { (int)SPC.Icon, characterIcon }
            };
        }

        public void ParseInfo(Hashtable info) {
            login = info.Value<string>((int)SPC.Login);
            gameRefID = info.Value<string>((int)SPC.GameRefId);
            characterID = info.Value<string>((int)SPC.CharacterId);
            isLeader = info.Value<bool>((int)SPC.IsLeader);
            worldID = info.Value<string>((int)SPC.WorldId);
            workshop = info.GetValue<int>((int)SPC.Workshop, (int)(byte)Workshop.Arlen);
            exp = info.GetValue<int>((int)SPC.Exp, 0);
            characterName = info.GetValue<string>((int)SPC.CharacterName, string.Empty);
            characterIcon = info.GetValue<int>((int)SPC.Icon, -1);
        }
    }
}
