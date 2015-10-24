using Photon.SocketServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Login.OperationHandlers {
    public abstract class BaseOperationHandler {

        public PeerBase peer { get; }
        public LoginApplication application { get; }

        public BaseOperationHandler(LoginApplication app, PeerBase peer) {
            this.application = app;
            this.peer = peer;
        }

        public abstract OperationResponse Handle(OperationRequest request, SendParameters sendParameters);
    }
}
