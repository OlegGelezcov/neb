﻿using Common;
using ExitGames.Logging;
using Photon.SocketServer;
using SelectCharacter.Operations;

namespace SelectCharacter.OperationHandlers {
    public class RegisterClientOperationHandler : BaseOperationHandler {

        private readonly static ILogger log = LogManager.GetCurrentClassLogger();

        public RegisterClientOperationHandler(SelectCharacterApplication app, SelectCharacterClientPeer peer)
            : base(app, peer) { }


        public override OperationResponse Handle(OperationRequest request, SendParameters sendParameters) {

            RegisterClientOperationRequest operation = new RegisterClientOperationRequest(peer.Protocol, request);
            if (!operation.IsValid) {
                return InvalidParametersOperationResponse((SelectCharacterOperationCode)request.OperationCode);
            }

            if (string.IsNullOrEmpty(operation.GameRefId)) {
                return new OperationResponse(request.OperationCode) { ReturnCode = (short)ReturnCode.InvalidOperationParameter, DebugMessage = "Invalid GameRefId" };
            }

            if(string.IsNullOrEmpty(operation.login)) {
                return new OperationResponse(request.OperationCode) {
                    ReturnCode = (short)ReturnCode.InvalidOperationParameter,
                    DebugMessage = "Invalid login parameter"
                };
            }

            peer.SetId(operation.GameRefId);
            peer.SetLogin(operation.login);

            if (!application.Clients.OnConnect(peer)) {
                return new OperationResponse(request.OperationCode) { ReturnCode = (short)ReturnCode.InvalidOperation, DebugMessage = "Error of registering client" };
            }

            log.InfoFormat("register client peer = {0}", peer.id);

            var bankSave = application.DB.LoadBank(operation.login);
            peer.SetBank(bankSave);

            return new OperationResponse(request.OperationCode) { ReturnCode = (short)ReturnCode.Ok };
        }
    }
}
