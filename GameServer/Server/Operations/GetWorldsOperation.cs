using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Server.Operations {
    public class GetWorldsOperation : Operation {

        public GetWorldsOperation(IRpcProtocol protocol, OperationRequest request)
            : base(protocol, request ) { }

        
    }
}
