using Common;
using ExitGames.Client.Photon;
using System;

namespace ntool.Listeners {
    public abstract class BaseListener : IPhotonPeerListener {

        private Application m_Application;
        private string m_Name;
        private PhotonPeer m_Peer;
        private ServerInfo m_ServerInfo;

        public BaseListener(string listenerName,  Application application, ServerInfo serverInfo ) {
            m_Name = listenerName;
            m_Application = application;
            m_ServerInfo = serverInfo;
        }

        public void SetPeer(PhotonPeer peer) {
            m_Peer = peer;
        }

        public virtual void DebugReturn(DebugLevel level, string message) {
            app.logger.PushColor(ConsoleColor.Yellow);
            app.logger.Log("{0} debug: {1} -> {2}", name, level, message);
            app.logger.PopColor();
        }

        public abstract void OnEvent(EventData eventData);

        public abstract void OnOperationResponse(OperationResponse operationResponse);

        public virtual void OnStatusChanged(StatusCode statusCode) {
            app.logger.PushColor(ConsoleColor.Green);
            app.logger.Log("{0} listener: {1}", name, statusCode);
            app.logger.PopColor();
        }

        public Application app {
            get {
                return m_Application;
            }
        }

        public string name {
            get {
                return m_Name;
            }
        }

        public PhotonPeer peer {
            get {
                return m_Peer;
            }
        }

        public ServerInfo serverInfo {
            get {
                return m_ServerInfo;
            }
        }

        protected virtual void LogMissingHandler(OperationResponse response) {
            app.logger.PushColor(ConsoleColor.DarkRed);
            app.logger.Log("{0} has missing handler for operation: {1}", name, (OperationCode)response.OperationCode);
            app.logger.PopColor();
        }
    }
}
