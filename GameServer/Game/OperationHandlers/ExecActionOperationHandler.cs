using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Photon.SocketServer;
using Space.Game;
using Space.Server.Operations;
using Common;
using Space.Server;
using System.Collections;

namespace Nebula.Game.OperationHandlers {
    public class ExecActionOperationHandler : BasePlayerOperationHandler {
        public override OperationResponse Handle(MmoActor actor, OperationRequest request, SendParameters sendParameters) {
            var operation = new ExecAction(actor.Peer.Protocol, request);
            if (!operation.IsValid) {
                return new OperationResponse(request.OperationCode) { ReturnCode = (int)ReturnCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage() };
            }

            operation.OnStart();
            //CL.Out("exec action called: " + operation.Action);

            Item item;
            bool actorItem = actor.TryGetItem((byte)ItemType.Avatar, operation.ItemId, out item);
            if (actorItem == false) {
                if (actor.World.ItemCache.TryGetItem((byte)ItemType.Avatar, operation.ItemId, out item) == false) {
                    //return new OperationResponse((byte)OperationCode.ExecAction) { ReturnCode = (short)ReturnCode.ItemNotFound, DebugMessage = "ItemNotFound" };
                    Hashtable methodResult = null;
                    var method = actor.ActionExecutor.GetType().GetMethod(operation.Action);

                    if (method != null) {
                        object respObject = method.Invoke(actor.ActionExecutor, operation.Parameters);
                        if (respObject != null && respObject is Hashtable) {
                            methodResult = respObject as Hashtable;
                            ExecActionResponse response = new ExecActionResponse { Result = methodResult, Action = operation.Action, ItemId = string.Empty };
                            return new OperationResponse(OperationCode.ExecAction.toByte(), response) {
                                ReturnCode = (int)ReturnCode.Ok,
                                DebugMessage = string.Format("Action completed when no avatar")
                            };
                        }
                    }
                    return new OperationResponse(OperationCode.ExecAction.toByte(), new ExecActionResponse { Result = new Hashtable(),
                    Action = operation.Action, ItemId = string.Empty}) {
                        ReturnCode = (int)ReturnCode.Fatal,
                        DebugMessage = "Method not found",

                    };
                }
            }
            if (actorItem) {
                if (item is MmoItem) {
                    return this.ItemOperationExecAction(item as MmoItem, operation, actor);
                } else {
                    return new OperationResponse((byte)OperationCode.ExecAction, new ExecActionResponse {  Result = new Hashtable(), Action = operation.Action, ItemId = string.Empty}) {
                        ReturnCode = (short)ReturnCode.Fatal,
                        DebugMessage = "Actor item Item not MmoItem"
                    };
                }
            }

            if (item is MmoItem) {
                MmoItem mmoItem = item as MmoItem;
                item.Fiber.Enqueue(() => actor.ExecItemOperation(() => this.ItemOperationExecAction(mmoItem, operation, actor), sendParameters));
                return null;
            } else {
                return new OperationResponse((byte)OperationCode.ExecAction, new ExecActionResponse {  Action = operation.Action, ItemId = string.Empty, Result = new Hashtable()}) {
                    ReturnCode = (short)ReturnCode.Fatal,
                    DebugMessage = "Foreign Actor item Item not MmoItem"
                };
            }
        }

        public OperationResponse ItemOperationExecAction(MmoItem item, ExecAction operation, MmoActor actor) {
            if (item.Owner == null)
                return new OperationResponse((byte)OperationCode.ExecAction) { ReturnCode = (int)ReturnCode.Fatal, DebugMessage = "Item owner is null in ExecAction" };
            object[] parameters = operation.Parameters;
            if (operation.Action == "CreateRaider" || operation.Action == "DestroyAnyRaider") {
                ArrayList arrLst = new ArrayList(parameters);
                arrLst.Insert(0, actor.World);
                parameters = arrLst.ToArray();
            }
            var method = item.Owner.ActionExecutor.GetType().GetMethod(operation.Action);
            Hashtable result = null;
            if (method != null) {
                object respObject = method.Invoke(item.Owner.ActionExecutor, parameters);
                if (respObject != null && respObject is Hashtable) {
                    result = respObject as Hashtable;
                }
            } else {
                return new OperationResponse((byte)OperationCode.ExecAction, new ExecActionResponse {  Action = operation.Action, ItemId = string.Empty, Result = new Hashtable()}) {
                    ReturnCode = (short)ReturnCode.Fatal,
                    DebugMessage = string.Format("Method: {0} not founded in ExecAction", operation.Action)
                };
            }
            operation.OnComplete();
            if (result == null) {
                result = new Hashtable();
            }
            ExecActionResponse response = new ExecActionResponse { Result = result, Action = operation.Action, ItemId = item.Id };
            return new OperationResponse((byte)OperationCode.ExecAction, response) {
                ReturnCode = (int)ReturnCode.Ok,
                DebugMessage = string.Format("action: {0} completed", operation.Action)
            };
        }
    }
}
