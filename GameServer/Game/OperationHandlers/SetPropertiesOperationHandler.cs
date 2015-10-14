using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Photon.SocketServer;
using Space.Game;
using Space.Server.Operations;
using Space.Server;
using Common;
using Space.Server.Events;
using Space.Server.Messages;

namespace Nebula.Game.OperationHandlers {
    class SetPropertiesOperationHandler : BasePlayerOperationHandler {
        public override OperationResponse Handle(MmoActor actor, OperationRequest request, SendParameters sendParameters) {
            var operation = new SetProperties(actor.Peer.Protocol, request);
            if (!operation.IsValid) {
                return new OperationResponse(request.OperationCode) { ReturnCode = (int)ReturnCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage() };
            }

            operation.OnStart();
            Item item;
            if (false == operation.ItemType.HasValue || string.IsNullOrEmpty(operation.ItemId)) {
                item = actor.Avatar;

                // set return values
                operation.ItemId = item.Id;
                operation.ItemType = item.Type;
            } else if (actor.TryGetItem(operation.ItemType.Value, operation.ItemId, out item) == false) {
                return operation.GetOperationResponse((int)ReturnCode.ItemNotFound, "ItemNotFound");
            }

            return this.ItemOperationSetProperties(item, operation, sendParameters, actor);
        }

        private OperationResponse ItemOperationSetProperties(Item item, SetProperties operation, SendParameters sendParameters, MmoActor actor) {
            MethodReturnValue result = this.CheckAccess(item, actor);
            if (result) {
                item.properties.SetProperties(operation.PropertiesSet, operation.PropertiesUnset);
                var eventInstance = new ItemPropertiesSet {
                    ItemId = item.Id,
                    ItemType = item.Type,
                    PropertiesRevision = item.properties.propertiesRevision,
                    PropertiesSet = operation.PropertiesSet,
                    PropertiesUnset = operation.PropertiesUnset
                };

                var eventData = new EventData((byte)EventCode.ItemPropertiesSet, eventInstance);
                sendParameters.ChannelId = Settings.ItemEventChannel;
                var message = new ItemEventMessage(item, eventData, sendParameters);
                item.EventChannel.Publish(message);

                // no response sent
                operation.OnComplete();
                return null;
            }

            return operation.GetOperationResponse(result);
        }
    }
}
