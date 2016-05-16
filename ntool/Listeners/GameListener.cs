using ExitGames.Client.Photon;

namespace ntool.Listeners {
    public class GameListener : BaseListener {
        public GameListener(string name, Application app, ServerInfo serverInfo)
            : base(name,  app, serverInfo ){ }

        public override void OnEvent(EventData eventData) {
            app.logger.Log(eventData.ToStringFull());
        }

        public override void OnOperationResponse(OperationResponse operationResponse) {
            app.logger.Log(operationResponse.ToStringFull());
        }
    }
}
