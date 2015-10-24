using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Photon.SocketServer;
using SelectCharacter.Operations;
using Common;
using System.Collections;
using ServerClientCommon;
using SelectCharacter.Events;
using ExitGames.Logging;

namespace SelectCharacter.OperationHandlers {
    public class SendPushToPlayersOperationHandler : BaseOperationHandler {

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public SendPushToPlayersOperationHandler(SelectCharacterApplication app, SelectCharacterClientPeer peer)
            : base(app, peer) { }

        public override OperationResponse Handle(OperationRequest request, SendParameters sendParameters) {

            log.InfoFormat("push send operation received...");

            SendPushToPlayersOperationRequest operation = new SendPushToPlayersOperationRequest(peer.Protocol, request);
            if(!operation.IsValid) {
                return InvalidParametersOperationResponse(Common.SelectCharacterOperationCode.SendPushToPlayers);
            }

            Hashtable pushInfo = new Hashtable {
                { (int)SPC.Type, operation.pushType },
                { (int)SPC.Body, operation.body },
                { (int)SPC.Title, operation.title }
            };
            GenericEvent genEvt = new GenericEvent {
                subCode = (int)SelectCharacterGenericEventSubCode.Push,
                data = pushInfo
            };
            application.Clients.SendGenericEvent(genEvt);

            return new OperationResponse((byte)SelectCharacterOperationCode.SendPushToPlayers);
        }
    }
}
