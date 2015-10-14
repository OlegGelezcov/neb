// GetServerListRequest.cs
// Nebula
//
// Created by Oleg Zheleztsov on Sunday, September 13, 2015 5:53:32 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
namespace Master.Operations {
    using Photon.SocketServer;
    using Photon.SocketServer.Rpc;
    public class GetServerListRequest : Operation {

        public GetServerListRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest) {

        }

        public GetServerListRequest() { }


    }
}
