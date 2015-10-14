using Common;
using ServerClientCommon;
using System.Collections;

namespace Nebula.Client.Worlds {
    public class WorldInfo : IInfoParser {

        public string ID { get; private set; }
        public Race startRace { get; private set; }
        public Race currentRace { get; private set; }
        public WorldType worldType { get; private set; }
        public bool underAttack { get; private set; }
        public Race attackedRace { get; private set; }
        public int playerCount { get; private set; }

        public WorldInfo(Hashtable info) { ParseInfo(info);  }

        public void ParseInfo(Hashtable info) {
            ID = info.Value<string>((int)SPC.WorldId, string.Empty);
            startRace = (Race)(byte)info.Value<int>((int)SPC.StartRace, 0);
            currentRace = (Race)(byte)info.Value<int>((int)SPC.CurrentRace, 0);
            worldType = (WorldType)(byte)info.Value<int>((int)SPC.WorldType, 0);
            underAttack = info.Value<bool>((int)SPC.UnderAttack, false);
            attackedRace = (Race)(byte)info.Value<int>((int)SPC.AttackRace, (int)(byte)Race.None);
            playerCount = (int)info.Value<int>((int)SPC.PlayerCount, 0);
        }

        public override string ToString() {
            return string.Format("ID={0}, Start Race = {1}, Current Race = {2}, World Type = {3}, Under Attack = {4}, Attack Race = {5}, Player Count = {6}",
                ID, startRace, currentRace, worldType, underAttack, attackedRace, playerCount);
        }
    }
}
