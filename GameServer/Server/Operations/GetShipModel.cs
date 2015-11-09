// GetShipModel.cs
// Nebula
//
// Created by Oleg Zheleztsov on Tuesday, November 3, 2015 1:45:34 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
using Common;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace Nebula.Server.Operations {

    /// <summary>
    /// Get ship model of player data contrack
    /// </summary>
    public class GetShipModel : Operation {

        public GetShipModel(IRpcProtocol protocol, OperationRequest request)
            : base(protocol, request ) { }

        /// <summary>
        /// id of player item
        /// </summary>
        [DataMember(Code =(byte)ParameterCode.ItemId, IsOptional = false)]
        public string itemId { get; set; }
    }
}
