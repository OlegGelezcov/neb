using Nebula.Server.Login;
using Photon.SocketServer;

namespace Login {
    public class LoggedInUser {
        public readonly FullUserAuth auth;
        public readonly LoginClientPeer peer;

        private LoggedInUser() { }

        public LoggedInUser(FullUserAuth fAuth, LoginClientPeer peer) {
            this.auth = fAuth;
            this.peer = peer;
        }

        public void SendEvent(EventData eventData) {
            if(peer != null ) {
                peer.SendEvent(eventData, new SendParameters());
            }
        }
    }
}
