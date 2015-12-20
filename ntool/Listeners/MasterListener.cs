using ExitGames.Client.Photon;

namespace ntool.Listeners {
    public class MasterListener : BaseListener {

        private Application m_Application;

        public MasterListener(string name,  Application app, ServerInfo serverInfo) 
            : base(name, app, serverInfo) { }

        public override  void OnEvent(EventData eventData) {
            
        }

        public override  void OnOperationResponse(OperationResponse operationResponse) {
            
        }
    }
}
