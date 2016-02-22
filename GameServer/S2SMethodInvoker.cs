using Common;
using ExitGames.Logging;
using Nebula.Game;
using Nebula.Game.Components;
using Nebula.Game.Utils;

namespace Nebula {
    public class S2SMethodInvoker {
        private static ILogger s_Log = LogManager.GetCurrentClassLogger();

        private GameApplication m_App;

        public S2SMethodInvoker(GameApplication app) {
            m_App = app;
        }

        public int AddAchievmentVariable(string gameRef, string variableName, int count ) {
            s_Log.InfoFormat("S2S: AddAchievmentVariable({0}, {1}, {2})".Color(LogColor.white), gameRef, variableName, count);
            var player = m_App.GetServerActor(gameRef);
            if(player != null ) {
                var achievments = player.GetComponent<AchievmentComponent>();
                if(achievments != null ) {
                    achievments.AddVariable(variableName, count);
                }
            }
            return (int)RPCErrorCode.Ok;
        }

        
    }
}
