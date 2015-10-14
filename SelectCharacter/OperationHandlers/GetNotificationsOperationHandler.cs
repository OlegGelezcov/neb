using Common;
using ExitGames.Logging;
using Photon.SocketServer;
using SelectCharacter.Operations;

namespace SelectCharacter.OperationHandlers {
    public class GetNotificationsOperationHandler : BaseOperationHandler {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public GetNotificationsOperationHandler(SelectCharacterApplication app, SelectCharacterClientPeer peer)
            : base(app, peer) { }

        public override OperationResponse Handle(OperationRequest request, SendParameters sendParameters) {
            GetNotificationsOperationRequest operation = new GetNotificationsOperationRequest(peer.Protocol, request);
            if (!operation.IsValid) { return InvalidParametersOperationResponse((SelectCharacterOperationCode)request.OperationCode); }
            string characterID = operation.CharacterID;
            var notifications = application.Notifications.GetNotifications(characterID);
            if (notifications == null) {
                log.ErrorFormat("notifications must be not null, character ID = {0}", characterID);
                return new OperationResponse(request.OperationCode) { ReturnCode = (short)ReturnCode.Fatal, DebugMessage = "notifications is null when getting" };
            }
            GetNotificationsOperationResponse response = new GetNotificationsOperationResponse { Notifications = notifications.GetInfo() };
            return new OperationResponse(request.OperationCode, response);
        }
    }
}
