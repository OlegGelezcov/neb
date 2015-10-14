using Common;
using Photon.SocketServer;
using Space.Game;
using Space.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Game.OperationHandlers {
    public abstract class BasePlayerOperationHandler {

        public abstract OperationResponse Handle(MmoActor actor, OperationRequest request, SendParameters sendParameters);

        protected MethodReturnValue CheckAccess(Item item, MmoActor actor) {
            if (item.Disposed) {
                return MethodReturnValue.Fail((int)ReturnCode.ItemNotFound, "ItemNotFound");
            }

            if (((IMmoItem)item).GrantWriteAccess(actor)) {
                return MethodReturnValue.Ok;
            }

            return MethodReturnValue.Fail((int)ReturnCode.ItemAccessDenied, "ItemAccessDenied");
        }
    }
}
