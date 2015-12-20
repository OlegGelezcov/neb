using ExitGames.Client.Photon;

namespace ntool.Listeners {
    public class SelectCharacterListener : BaseListener {
        public SelectCharacterListener(string name,  Application app, ServerInfo serverInfo)
            : base(name,  app, serverInfo) { }

        public override void OnEvent(EventData eventData) {
        }
        public override void OnOperationResponse(OperationResponse operationResponse) {
        }
    }
}
