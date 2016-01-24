using Common;
using ServerClientCommon;
using System.Collections;
using System.Xml.Linq;

namespace Nebula.Server.Components {
    public class MiningStationComponentData : ComponentData, IDatabaseComponentData  {

        public string nebulaElementID { get; private set; }
        public int maxCount { get; private set; }
        public float timeToGetSingleElement { get; private set; }
        public string sourceID { get; private set; }
        public string ownedPlayerID { get; private set; }

        public int totalCount { get; private set; }

        public string characterID { get; private set; }

        public int currentCount { get; private set; } = 0;
        public int currentTotatlCount { get; private set; } = 0;

        public MiningStationComponentData(XElement element) { }

        public MiningStationComponentData(string inNebulaElementID, 
            int inMaxCount, 
            float inTimeToGetSingleElement, 
            string inSourcePlanetID, 
            string inOwnedPlayerID, 
            int inTotalCount, 
            string inCharacterID) {

            nebulaElementID = inNebulaElementID;
            maxCount = inMaxCount;
            timeToGetSingleElement = inTimeToGetSingleElement;
            sourceID = inSourcePlanetID;
            ownedPlayerID = inOwnedPlayerID;
            totalCount = inTotalCount;
            characterID = inCharacterID;
        }

        public MiningStationComponentData(Hashtable hash) {
            nebulaElementID = hash.GetValue<string>((int)SPC.NebulaElementId, string.Empty);
            maxCount = hash.GetValue<int>((int)SPC.MaxCount, 0);
            timeToGetSingleElement = hash.GetValue<float>((int)SPC.Time, 0f);
            sourceID = hash.GetValue<string>((int)SPC.Source, string.Empty);
            ownedPlayerID = hash.GetValue<string>((int)SPC.OwnerGameRef, string.Empty);
            totalCount = hash.GetValue<int>((int)SPC.Total, 0);
            characterID = hash.GetValue<string>((int)SPC.CharacterId, string.Empty);
            currentCount = hash.GetValue<int>((int)SPC.CurrentCount, 0);
            currentTotatlCount = hash.GetValue<int>((int)SPC.CurrentTotal, 0);
        }

        public override ComponentID componentID {
            get {
                return ComponentID.MiningStation;
            }
        }

        public void SetCurrentCount(int cnt) {
            currentCount = cnt;
        }

        public void SetCurrentTotalCount(int cnt) {
            currentTotatlCount = cnt;
        }

        public Hashtable AsHash() {
            return new Hashtable {
                { (int)SPC.NebulaElementId, nebulaElementID },
                { (int)SPC.MaxCount, maxCount },
                { (int)SPC.Time, timeToGetSingleElement },
                { (int)SPC.Source, sourceID },
                { (int)SPC.OwnerGameRef, ownedPlayerID },
                { (int)SPC.Total, totalCount },
                { (int)SPC.CharacterId, characterID },
                { (int)SPC.CurrentCount, currentCount },
                { (int)SPC.CurrentTotal, currentTotatlCount }
            };
        }
    }
}
