using Common;
using Photon.SocketServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectCharacter.OperationHandlers {
    public abstract class BaseOperationHandler {

        protected readonly SelectCharacterApplication application;
        protected readonly SelectCharacterClientPeer peer;

        public BaseOperationHandler(SelectCharacterApplication application, SelectCharacterClientPeer peer) {
            this.application = application;
            this.peer = peer;
        }


        public abstract OperationResponse Handle(OperationRequest request, SendParameters sendParameters);

        protected OperationResponse InvalidParametersOperationResponse(SelectCharacterOperationCode code) {
            return new OperationResponse((byte)code) {
                ReturnCode = (short)ReturnCode.InvalidOperationParameter,
                DebugMessage = string.Format("Operation {0}: invalid operation parameters", code)
            };
        }
    }
}
