using Common;
using Photon.SocketServer;
using SelectCharacter.Operations;

namespace SelectCharacter.OperationHandlers {
    public class RPCInvokeMethodOperationHandler : BaseOperationHandler {
        public RPCInvokeMethodOperationHandler(SelectCharacterApplication app, SelectCharacterClientPeer peer)
            : base(app, peer) { }

        public override OperationResponse Handle(OperationRequest request, SendParameters sendParameters) {
            RPCInvokeOperation operation = new RPCInvokeOperation(peer.Protocol, request);
            if(!operation.IsValid) {
                return InvalidParametersOperationResponse(Common.SelectCharacterOperationCode.RPCInvokeMethod);
            }

            OperationResponse response = null;
            switch(operation.rpcId) {
                case RPCID.rpc_sc_SetRaceStatus: {
                        response = CallSetRaceStatus(request, operation);
                    }
                    break;
                case RPCID.rpc_sc_DepositCreditsToCoalition: {
                        response = DepositCreditsToCoalition(request, operation);
                    }
                    break;
                case RPCID.rpc_sc_DepositPvpPointsToCoalition: {
                        response = DepositPvpPointsToCoalition(request, operation);
                    }
                    break;
                case RPCID.rpc_sc_WithdrawCreditsFromCoalition: {
                        response = WithdrawCreditsFromCoalition(request, operation);
                    }
                    break;
                case RPCID.rpc_sc_WithdrawPvpPointsFromCoalition: {
                        response = WithdrawPvpPointsFromCoalition(request, operation);
                    }
                    break;
                default:
                    response = new OperationResponse(request.OperationCode) {
                        ReturnCode = (short)ReturnCode.InvalidRPCID,
                        DebugMessage = string.Format("not found rpc: {0} at SelectCharacterApplication", operation.rpcId)
                    };
                    break;
            }
            return response;
        }

        private OperationResponse CallSetRaceStatus(OperationRequest request, RPCInvokeOperation op ) {
            if(op.parameters != null && op.parameters.Length >= 3) {
                string gameRef = (string)op.parameters[0];
                string charId = (string)op.parameters[1];
                int raceStatus = (int)op.parameters[2];
                bool success = application.Players.SetRaceStatus(gameRef, charId, raceStatus);

                RPCInvokeResponse responseInstance = new RPCInvokeResponse {
                    rpcId = op.rpcId,
                    result = success
                };
                return new OperationResponse(request.OperationCode, responseInstance);
            }
            return DefaultError();
        }

        private OperationResponse DepositCreditsToCoalition(OperationRequest request, RPCInvokeOperation op) {
            if(op.parameters != null && op.parameters.Length >= 3 ) {
                string cid = (string)op.parameters[0];
                string gid = (string)op.parameters[1];
                int count = (int)op.parameters[2];
                ActionResult actionResult = application.Guilds.DepositCredits(cid, gid, count);
                RPCInvokeResponse responseInstance = new RPCInvokeResponse {
                    rpcId = op.rpcId,
                    result = actionResult.GetInfo()
                };
                return new OperationResponse(request.OperationCode, responseInstance);
            }
            return DefaultError();
        }

        private OperationResponse DepositPvpPointsToCoalition(OperationRequest request, RPCInvokeOperation op) {
            if (op.parameters != null && op.parameters.Length >= 3) {
                string cid = (string)op.parameters[0];
                string gid = (string)op.parameters[1];
                int count = (int)op.parameters[2];
                ActionResult actionResult = application.Guilds.DepositPvpPoints(cid, gid, count);
                RPCInvokeResponse responseInstance = new RPCInvokeResponse {
                    rpcId = op.rpcId,
                    result = actionResult.GetInfo()
                };
                return new OperationResponse(request.OperationCode, responseInstance);
            }
            return DefaultError();
        }

        private OperationResponse WithdrawCreditsFromCoalition(OperationRequest request, RPCInvokeOperation op) {
            if (op.parameters != null && op.parameters.Length >= 3) {
                string cid = (string)op.parameters[0];
                string gid = (string)op.parameters[1];
                int count = (int)op.parameters[2];
                ActionResult actionResult = application.Guilds.WithdrawCredits(cid, gid, count);
                RPCInvokeResponse responseInstance = new RPCInvokeResponse {
                    rpcId = op.rpcId,
                    result = actionResult.GetInfo()
                };
                return new OperationResponse(request.OperationCode, responseInstance);
            }
            return DefaultError();
        }

        private OperationResponse WithdrawPvpPointsFromCoalition(OperationRequest request, RPCInvokeOperation op) {
            if (op.parameters != null && op.parameters.Length >= 3) {
                string cid = (string)op.parameters[0];
                string gid = (string)op.parameters[1];
                int count = (int)op.parameters[2];
                ActionResult actionResult = application.Guilds.WithdrawPvpPoints(cid, gid, count);
                RPCInvokeResponse responseInstance = new RPCInvokeResponse {
                    rpcId = op.rpcId,
                    result = actionResult.GetInfo()
                };
                return new OperationResponse(request.OperationCode, responseInstance);
            }
            return DefaultError();
        }

        private OperationResponse DefaultError() {
            return InvalidParametersOperationResponse(SelectCharacterOperationCode.RPCInvokeMethod);
        }
    }
}
