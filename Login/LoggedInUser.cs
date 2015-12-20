using Nebula.Server.Login;

namespace Login {
    public class LoggedInUser {
        public readonly FullUserAuth auth;
        public readonly LoginClientPeer peer;

        private LoggedInUser() { }

        public LoggedInUser(FullUserAuth fAuth, LoginClientPeer peer) {
            this.auth = fAuth;
            this.peer = peer;
        }
    }
}
