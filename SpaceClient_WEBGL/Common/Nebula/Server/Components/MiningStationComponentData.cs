
using Common;
#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif

namespace Nebula.Server.Components {
    public class MiningStationComponentData : ComponentData {

        public string nebulaElementID { get; private set; }
        public int maxCount { get; private set; }
        public float timeToGetSingleElement { get; private set; }
        public string sourceID { get; private set; }
        public string ownedPlayerID { get; private set; }

        public int totalCount { get; private set; }

        public string characterID { get; private set; }

#if UP
        public MiningStationComponentData(UPXElement element) { }
#else
        public MiningStationComponentData(XElement element) { }
#endif

        public MiningStationComponentData(string inNebulaElementID, int inMaxCount, float inTimeToGetSingleElement, string inSourcePlanetID, string inOwnedPlayerID, int inTotalCount, string inCharacterID) {
            nebulaElementID = inNebulaElementID;
            maxCount = inMaxCount;
            timeToGetSingleElement = inTimeToGetSingleElement;
            sourceID = inSourcePlanetID;
            ownedPlayerID = inOwnedPlayerID;
            totalCount = inTotalCount;
            characterID = inCharacterID;
        }

        public override ComponentID componentID {
            get {
                return ComponentID.MiningStation;
            }
        }
    }
}
