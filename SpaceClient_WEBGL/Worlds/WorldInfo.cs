using Common;
using ServerClientCommon;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;

namespace Nebula.Client.Worlds {
    public class WorldInfo : IInfoParser {

        public string ID { get; private set; }
        public Race startRace { get; private set; }
        public Race currentRace { get; private set; }
        public WorldType worldType { get; private set; }
        public bool underAttack { get; private set; }
        public Race attackedRace { get; private set; }
        public int playerCount { get; private set; }

        public WorldInfo(Hashtable info) { ParseInfo(info); }

        public void ParseInfo(Hashtable info) {
            ID = info.GetValueString((int)SPC.WorldId);
            startRace = (Race)(byte)info.GetValueInt((int)SPC.StartRace);
            currentRace = (Race)(byte)info.GetValueInt((int)SPC.CurrentRace);
            worldType = (WorldType)(byte)info.GetValueInt((int)SPC.WorldType);
            underAttack = info.GetValueBool((int)SPC.UnderAttack);
            attackedRace = (Race)(byte)info.GetValueInt((int)SPC.AttackRace, (int)(byte)Race.None);
            playerCount = (int)info.GetValueInt((int)SPC.PlayerCount);
        }

        public override string ToString() {
            return string.Format("ID={0}, Start Race = {1}, Current Race = {2}, World Type = {3}, Under Attack = {4}, Attack Race = {5}, Player Count = {6}",
                ID, startRace, currentRace, worldType, underAttack, attackedRace, playerCount);
        }
    }
}
