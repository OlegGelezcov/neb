// GetShipModelOperationHandler.cs
// Nebula
//
// Created by Oleg Zheleztsov on Tuesday, November 3, 2015 1:50:27 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
using Common;
using Nebula.Game.Components;
using Nebula.Server.Operations;
using Photon.SocketServer;
using Space.Game;
using System;

namespace Nebula.Game.OperationHandlers {

    /// <summary>
    /// Operation handler for GetShipModel
    /// </summary>
    public class GetShipModelOperationHandler : BasePlayerOperationHandler {

        public override OperationResponse Handle(MmoActor player, OperationRequest request, SendParameters sendParameters) {
            GetShipModel operation = new GetShipModel(player.Peer.Protocol, request);
            if(!operation.IsValid ) {
                return new OperationResponse(request.OperationCode) {
                    ReturnCode = (short)ReturnCode.InvalidOperationParameter,
                    DebugMessage = operation.GetErrorMessage()
                };
            }

            var playerShip = player.GetComponent<PlayerShip>();
            if(playerShip == null || playerShip.shipModel == null) {
                return new OperationResponse(request.OperationCode) {
                    ReturnCode = (short)ReturnCode.Fatal,
                    DebugMessage = "Player ship or ship model don't exists"
                };
            }

            GetShipModelResponse responseObject = new GetShipModelResponse {
                model = playerShip.shipModel.GetExistingInfo()
            };
            return new OperationResponse(request.OperationCode, responseObject);
        }
    }
}
