using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Photon.SocketServer;
using Space.Game;
using Space.Server.Operations;
using Common;
using Space.Server;
using Space.Server.Events;
using Space.Server.Messages;
using Nebula.Game.Components;

namespace Nebula.Game.OperationHandlers {
    public class MoveOperationHandler : BasePlayerOperationHandler {

        public override OperationResponse Handle(MmoActor actor, OperationRequest request, SendParameters sendParameters) {

            var operation = new Move(actor.Peer.Protocol, request);
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

            return this.ItemOperationMove((MmoItem)item, operation, sendParameters, actor);
        }

        private OperationResponse ItemOperationMove(MmoItem item, Move operation, SendParameters sendParameters, MmoActor actor) {
            // should always be OK
            MethodReturnValue result = this.CheckAccess(item, actor);

            if (result) {
                // save previous for event
                float[] oldPosition = item.transform.position.ToArray();
                float[] oldRotation = item.transform.rotation.ToArray();

                // move
                item.transform.SetRotation(operation.Rotation);
                item.Move(operation.Position);

                float speed = 0f;
                var ship = item.GetComponent<PlayerShip>();
                var movalble = item.GetComponent<MovableObject>();
                if(ship) {
                    speed = movalble.speed;
                }

                // send event
                var eventInstance = new ItemMoved {
                    ItemId = item.Id,
                    ItemType = item.Type,
                    OldPosition = oldPosition,
                    Position = operation.Position,
                    Rotation = operation.Rotation,
                    OldRotation = oldRotation,
                    Speed = speed
                };

                var eventData = new EventData((byte)EventCode.ItemMoved, eventInstance);
                sendParameters.ChannelId = Settings.ItemEventChannel;
                var message = new ItemEventMessage(item, eventData, sendParameters);
                item.EventChannel.Publish(message);

                //I ADDED toMOVE AT POSITION
                //item.ReceiveEvent(eventData, sendParameters);

                // no response sent
                operation.OnComplete();
                return null;
            }

            return operation.GetOperationResponse(result);
        }
    }
}
