using Nebula.Client;
using Nebula.Client.PassiveBonuses;
using Nebula.Client.Planets;
using Nebula.Login;
using ServerClientCommon;

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

        public static event System.Action<LoggedUser, LoginReturnCode> LoginResponse;

        public static event System.Action<LoginReturnCode, string, string> UserRegistered;

        public static event System.Action<LoginReturnCode> UserRecoverResponse;

        public static event System.Action<GameTimeInfo> GameTimeReceived;

        public static event System.Action<LoginReturnCode> UsePassResponse;

        public static event System.Action<string> GameServerIdReceived;

        public static event System.Action BankUpdated;



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

        public static void EvtLoginResponse(LoggedUser user, LoginReturnCode code) {
            if(LoginResponse != null ) {
                LoginResponse(user, code);
            }
        }

        public static void EvtUserRegistered(LoginReturnCode code, string login, string gameRef) {
            if(UserRegistered != null ) {
                UserRegistered(code, login, gameRef);
            }
        }

        public static void EvtUserRecoverResponse(LoginReturnCode code) {
            if(UserRecoverResponse != null ) {
                UserRecoverResponse(code);
            }
        }

        public static void EvtGameTimeInfoReceived(GameTimeInfo info) {
            if(GameTimeReceived != null ) {
                GameTimeReceived(info);
            }
        }

        public static void EvtUsePassResponse(LoginReturnCode code) {
            if(UsePassResponse != null ) {
                UsePassResponse(code);
            }
        }

        public static void EvtGameServerIdReceived(string serverID) {
            if(GameServerIdReceived != null) {
                GameServerIdReceived(serverID);
            }
        }

        public static void EvtBankUpdated() {
            if(BankUpdated != null ) {
                BankUpdated();
            }
        }
    }
}
