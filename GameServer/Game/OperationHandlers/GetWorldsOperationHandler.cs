using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Photon.SocketServer;
using Space.Game;
using Nebula.Server.Operations;
using Common;
using System.Collections;

namespace Nebula.Game.OperationHandlers {
    public class GetWorldsOperationHandler : BasePlayerOperationHandler {

        public override OperationResponse Handle(MmoActor player, OperationRequest request, SendParameters sendParameters) {
            GetWorldsOperation operation = new GetWorldsOperation(player.Peer.Protocol, request);
            if(!operation.IsValid) {
                return new OperationResponse(request.OperationCode) { ReturnCode = (short)ReturnCode.InvalidOperationParameter, DebugMessage = "Invalid operation parameters" };
            }

            var dict = player.application.DatabaseManager.GetWorlds();

            Hashtable worldHash = new Hashtable();

            foreach(var p in dict ) {
                worldHash.Add(p.Key, p.Value.GetInfo());
            }

            GetWorldsOperationResponse responseObject = new GetWorldsOperationResponse { worlds = worldHash };
            return new OperationResponse(request.OperationCode, responseObject);
        }
    }
}
