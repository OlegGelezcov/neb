using Common;
using ExitGames.Logging;
using Photon.SocketServer;
using SelectCharacter.Operations;

namespace SelectCharacter.OperationHandlers {
    public class HandleNotificationOperationHandler : BaseOperationHandler {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public HandleNotificationOperationHandler(SelectCharacterApplication app, SelectCharacterClientPeer peer)
            : base(app, peer) { }

        public override OperationResponse Handle(OperationRequest request, SendParameters sendParameters) {
            HandleNotificationOperationRequest operation = new HandleNotificationOperationRequest(peer.Protocol, request);
            if (!operation.IsValid) {
                return InvalidParametersOperationResponse((SelectCharacterOperationCode)request.OperationCode);
            }

            application.Notifications.HandleNotification(operation.CharacterID, operation.NotificationID, operation.Respond);

            var notifications = application.Notifications.GetNotifications(operation.CharacterID);
            if (notifications == null) {
                log.ErrorFormat("notifications must be not null, character ID = {0}", operation.CharacterID);
                return new OperationResponse(request.OperationCode) { ReturnCode = (short)ReturnCode.Fatal, DebugMessage = "notifications is null when getting" };
            }
            HandleNotificationOperationResponse response = new HandleNotificationOperationResponse { Notifications = notifications.GetInfo() };
            return new OperationResponse(request.OperationCode, response);
        }
    }
}
