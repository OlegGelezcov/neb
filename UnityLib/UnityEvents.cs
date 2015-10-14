using Nebula.Client;
using Nebula.Client.PassiveBonuses;
using Nebula.Client.Planets;

namespace Nebula {
    public static class UnityEvents {

        /// <summary>
        /// Fire when event with custom code GetPlanetInfo received from server
        /// </summary>
        public static event System.Action<PlanetObjectInfo> PlanetObjectInfoReceived;

        /// <summary>
        /// Fire when event with custom code GetMiningStationInfo received from server
        /// </summary>
        public static event System.Action<MiningStationInfo> MiningStationInfoReceived;

        public static event System.Action<PlayerPassiveBonuses> PlayerPassiveBonusesReceived;

        public static event System.Action<PassiveBonusInfo> PassiveBonusComplete;



        public static void EvtPlanetObjectInfoReceived(PlanetObjectInfo planet) {
            if(PlanetObjectInfoReceived != null) {
                PlanetObjectInfoReceived(planet);
            }
        }

        public static void EvtMiningStationInfoReceived(MiningStationInfo miningStation) {
            if(MiningStationInfoReceived != null) {
                MiningStationInfoReceived(miningStation);
            }
        }

        public static void EvtPlayerPassiveBonusesReceived(PlayerPassiveBonuses bonuses) {
            if(PlayerPassiveBonusesReceived != null ) {
                PlayerPassiveBonusesReceived(bonuses);
            }
        }

        public static void EvtPassiveBonusComplete(PassiveBonusInfo passiveBonus) {
            if(PassiveBonusComplete != null ) {
                PassiveBonusComplete(passiveBonus);
            }
        }
    }
}
