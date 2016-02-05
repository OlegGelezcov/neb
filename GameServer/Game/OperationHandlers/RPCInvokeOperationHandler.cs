using Common;
using Nebula.Server.Operations;
using Photon.SocketServer;
using Space.Game;
using System.Collections;

namespace Nebula.Game.OperationHandlers {
    public class RPCInvokeOperationHandler : BasePlayerOperationHandler {

        public override OperationResponse Handle(MmoActor actor, OperationRequest request, SendParameters sendParameters) {
            var operation = new RPCInvokeOperation(actor.Peer.Protocol, request);
            if(!operation.IsValid) {
                return new OperationResponse(request.OperationCode) {
                    ReturnCode = (int)ReturnCode.InvalidOperationParameter,
                    DebugMessage = operation.GetErrorMessage()
                };
            }

            switch(operation.rpcId) {
                case RPCID.rpc_ProposeContract:
                    return CallProposeContract(actor, request, operation);
                case RPCID.rpc_RestartLoop:
                    return CallRestartLoop(actor, request, operation);
                case RPCID.rpc_AcceptContract:
                    return CallAcceptContract(actor, request, operation);
                case RPCID.rpc_DeclineContract:
                    return CallDeclineContract(actor, request, operation);
                case RPCID.rpc_CompleteContract:
                    return CallCompleteContract(actor, request, operation);
                default:
                    return new OperationResponse(request.OperationCode) {
                        ReturnCode = (int)ReturnCode.InvalidRPCID,
                        DebugMessage = string.Format("not found rpc with id = {0}", operation.rpcId)
                    };
            }
        }

        private OperationResponse CallCompleteContract(MmoActor player, OperationRequest request, RPCInvokeOperation op) {
            if(op.parameters != null && op.parameters.Length >= 1) {
                string contractId = op.parameters[0] as string;
                Hashtable ret = player.ActionExecutor.CompleteContract(contractId);
                RPCInvokeResponse responseInstance = new RPCInvokeResponse {
                    rpcId = op.rpcId,
                    result = ret
                };
                return new OperationResponse(request.OperationCode, responseInstance);
            }
            return InvalidOperationParameter(request);
        }

        private OperationResponse CallDeclineContract(MmoActor player, OperationRequest request, RPCInvokeOperation op) {
            if(op.parameters != null && op.parameters.Length >= 1) {
                string contractId = op.parameters[0] as string;
                Hashtable ret = player.ActionExecutor.DeclineContract(contractId);
                RPCInvokeResponse responseInstance = new RPCInvokeResponse {
                    rpcId = op.rpcId,
                    result = ret
                };
                return new OperationResponse(request.OperationCode, responseInstance);
            }
            return InvalidOperationParameter(request);
        }

        private OperationResponse CallAcceptContract(MmoActor player, OperationRequest request, RPCInvokeOperation op) {
            if(op.parameters != null && op.parameters.Length >= 1 ) {
                string contractId = op.parameters[0] as string;
                Hashtable ret = player.ActionExecutor.AcceptContract(contractId);
                RPCInvokeResponse responseInstance = new RPCInvokeResponse {
                    rpcId = op.rpcId,
                    result = ret
                };
                return new OperationResponse(request.OperationCode, responseInstance);
            }
            return InvalidOperationParameter(request);
        }

        private OperationResponse CallRestartLoop(MmoActor player, OperationRequest request, RPCInvokeOperation op) {
            int result = player.ActionExecutor.RestartLoop();
            RPCInvokeResponse r = new RPCInvokeResponse { rpcId = op.rpcId, result = result };
            return new OperationResponse(request.OperationCode, r);
        }

        private OperationResponse CallProposeContract(MmoActor player, OperationRequest request, RPCInvokeOperation op) {
            if(op.parameters != null && op.parameters.Length >= 1) {
                int category = (int)op.parameters[0];
                Hashtable result = player.ActionExecutor.ProposeContract(category);
                RPCInvokeResponse responseInstance = new RPCInvokeResponse {
                     rpcId = op.rpcId,
                     result = result
                };
                return new OperationResponse(request.OperationCode, responseInstance);
            }
            return InvalidOperationParameter(request);
        }

        private OperationResponse InvalidOperationParameter(OperationRequest request) {
            return new OperationResponse(request.OperationCode) {
                ReturnCode = (int)ReturnCode.InvalidOperationParameter,
                DebugMessage = "RPC parameters not valid"
            };
        }
    }
}
