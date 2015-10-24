using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Server.Operations {
    public class RequestServerID : Operation {
        public RequestServerID(IRpcProtocol protocol, OperationRequest request)
            : base(protocol, request) {

        }
    }
}
