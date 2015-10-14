using Common;
using ExitGames.Logging;

namespace SelectCharacter {
    public class S2SMethodInvoker {

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private SelectCharacterApplication mApplication;

        public S2SMethodInvoker(SelectCharacterApplication application) {
            mApplication = application;
        }

        public object AddCredits(string login, string gameRefID, string characterID, int credits) {
            log.InfoFormat("{0}:{1}:{2} added credits {3}", login, gameRefID, characterID, credits);
            mApplication.Stores.AddCredits(login, gameRefID, characterID, credits);
            return (int)ReturnCode.Ok;
        }

        public object RequestRaceStatus(string gameRefID, string characterID) {
            var player = mApplication.Players.GetExistingPlayer(gameRefID);
            if(player != null ) {
                var character = player.Data.GetCharacter(characterID);

                
                if(character != null ) {
                    mApplication.SendRaceStatusChanged(gameRefID, characterID, character.raceStatus);
                    return (int)ReturnCode.Ok;
                }
            }
            return (int)ReturnCode.Fatal;
        }
    }
}
