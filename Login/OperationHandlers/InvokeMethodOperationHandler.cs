using Common;
using Login.Operations;
using Photon.SocketServer;

namespace Login.OperationHandlers {
    public class InvokeMethodOperationHandler : BaseOperationHandler {

        public InvokeMethodOperationHandler(LoginApplication app, PeerBase peer)
            : base(app, peer) { }

        public override OperationResponse Handle(OperationRequest request, SendParameters sendParameters) {
            InvokeMethodOperationRequest operation = new InvokeMethodOperationRequest(peer.Protocol, request);
            if(!operation.IsValid) {
                return new OperationResponse(request.OperationCode) {
                    ReturnCode = (short)ReturnCode.InvalidOperationParameter,
                    DebugMessage = operation.GetErrorMessage()
                };
            }

            var lpeer = peer as LoginClientPeer;
            var method = lpeer.invoker.GetType().GetMethod(operation.method);
            if(method == null ) {
                return new OperationResponse(request.OperationCode) {
                    ReturnCode = (short)ReturnCode.Fatal,
                    DebugMessage = string.Format("method = {0} not founded", operation.method)
                };
            }

            object result = method.Invoke(lpeer.invoker, operation.arguments);
            InvokeMethodOperationResponse responseObject = new InvokeMethodOperationResponse {
                method = operation.method,
                returnValue = result
            };
            return new OperationResponse(request.OperationCode, responseObject);
        }
    }
}
