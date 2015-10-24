using Common;
using ExitGames.Logging;
using Login.OperationHandlers;
using Login.Operations;
using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;
using ServerClientCommon;
using System.Collections.Generic;

namespace Login {
    public class LoginClientPeer : PeerBase {

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        private LoginApplication application;
        public string login { get; private set; } = string.Empty;

        private readonly Dictionary<OperationCode, BaseOperationHandler> mOperationHandlers;

        public MethodInvoker invoker { get; }

        public LoginClientPeer(InitRequest initRequest, LoginApplication application)
            : base(initRequest.Protocol, initRequest.PhotonPeer) {
            
            this.application = application;
            invoker = new MethodInvoker(application, this);

            mOperationHandlers = new Dictionary<OperationCode, BaseOperationHandler>();
            mOperationHandlers.Add(OperationCode.Login, new LoginOperationHandler(application, this));
            mOperationHandlers.Add(OperationCode.GetUsersOnline, new GetUsersOnlineHandler(application, this));
            mOperationHandlers.Add(OperationCode.RegisterUser, new RegisterUserHandler(application, this));
            mOperationHandlers.Add(OperationCode.GetUserPasses, new GetUserPassesOperationHandler(application, this));
            mOperationHandlers.Add(OperationCode.RecoverUser, new RecoverUserOperationHandler(application, this));
            mOperationHandlers.Add(OperationCode.UsePass, new UsePassOperationHandler(application, this));
            mOperationHandlers.Add(OperationCode.AddPass, new AddPassOperationHandler(application, this));
            mOperationHandlers.Add(OperationCode.ExecAction, new InvokeMethodOperationHandler(application, this));
        }

        protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail) {
            if (log.IsDebugEnabled) {
                log.DebugFormat("LoginClientPeer Disconnect: pid={0}: reason={1}, detail={2}", this.ConnectionId, reasonCode, reasonDetail);
            }
            application.LogedInUsers.OnLogOut(login);
        }

        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters) {
            OperationResponse response;
            if (mOperationHandlers.ContainsKey((OperationCode)operationRequest.OperationCode)) {
                response = mOperationHandlers[(OperationCode)operationRequest.OperationCode].Handle(operationRequest, sendParameters);
            } else {
                response = new OperationResponse(operationRequest.OperationCode) {
                    ReturnCode = (short)ReturnCode.OperationInvalid,
                    DebugMessage = "Unknown operation code"
                };
            }
          
            if(response != null ) {
                this.SendOperationResponse(response, sendParameters);
            }
        }
    }
}
