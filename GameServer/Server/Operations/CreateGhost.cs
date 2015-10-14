namespace Space.Server.Operations
{
    using System;
    using System.Collections;
    using Common;
    using Photon.SocketServer;
    using Photon.SocketServer.Rpc;
    using Space.Mmo.Server;

    public class CreateGhost : Operation
    {
        public CreateGhost(IRpcProtocol protocol, OperationRequest request )
            : base(protocol, request)
        {

        }


    }
}
