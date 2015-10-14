using Common;
using Photon.SocketServer;
using SelectCharacter.Operations;

namespace SelectCharacter.OperationHandlers {
    public class InvokeMethodOperationHandler : BaseOperationHandler {

        public InvokeMethodOperationHandler(SelectCharacterApplication app, SelectCharacterClientPeer peer)
            : base(app, peer) { }

        public override OperationResponse Handle(OperationRequest request, SendParameters sendParameters) {
            InvokeMethodOperationRequest operation = new InvokeMethodOperationRequest(peer.Protocol, request);
            if (!operation.IsValid) {
                return InvalidParametersOperationResponse((SelectCharacterOperationCode)request.OperationCode);
            }

            var method = peer.invoker.GetType().GetMethod(operation.MethodName);
            if (method == null) {
                return new OperationResponse(request.OperationCode) { ReturnCode = (short)ReturnCode.Fatal, DebugMessage = string.Format("method = {0} not founded", operation.MethodName) };
            }
            object result = method.Invoke(peer.invoker, operation.Arguments);
            InvokeMethodOperationResponse response = new InvokeMethodOperationResponse { MethodName = operation.MethodName, ReturnValue = result };
            return new OperationResponse(request.OperationCode, response);
        }
    }
}
