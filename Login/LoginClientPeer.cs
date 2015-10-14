using Common;
using ExitGames.Logging;
using Login.Operations;
using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;
using ServerClientCommon;

namespace Login {
    public class LoginClientPeer : PeerBase {

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        private LoginApplication application;
        public string login { get; private set; } = string.Empty;

        public LoginClientPeer(InitRequest initRequest, LoginApplication application)
            : base(initRequest.Protocol, initRequest.PhotonPeer) {
            this.application = application;
        }

        protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail) {
            if (log.IsDebugEnabled) {
                log.DebugFormat("LoginClientPeer Disconnect: pid={0}: reason={1}, detail={2}", this.ConnectionId, reasonCode, reasonDetail);
            }
            application.LogedInUsers.OnLogOut(login);
        }

        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters) {

            OperationResponse response;
            switch((OperationCode)operationRequest.OperationCode) {
                default:
                    {
                        response = new OperationResponse(operationRequest.OperationCode) {
                            ReturnCode = (short)ReturnCode.OperationInvalid,
                            DebugMessage = "Unknown operation code"
                        };
                        break;
                    }
                case OperationCode.Login:
                    {
                        LoginOperationRequest operation = new LoginOperationRequest(this.Protocol, operationRequest);

                        if(!operation.IsValid ) {
                            response = new OperationResponse(operationRequest.OperationCode) {
                                ReturnCode = (short)ReturnCode.OperationInvalid,
                                DebugMessage = "Login operation parameters invalid"
                            };
                        } else {
                            if(false == this.application.CheckAccessToken(operation.LoginId, operation.AccessToken)) {
                                response = new OperationResponse(operationRequest.OperationCode) {
                                    ReturnCode = (short)ReturnCode.AccessTokenInvalid,
                                    DebugMessage = "Access token invalid"
                                };
                            } else {
                                LoginReturnCode code = LoginReturnCode.Ok;

                                var userLogin = this.application.GetUserLogin(operation.LoginId, operation.AccessToken, out code);
                                if(code != LoginReturnCode.Ok ) {
                                    var rObj = new LoginOperationResponse {
                                        GameRefId = (userLogin != null) ? userLogin.GameRefId : operation.AccessToken,
                                        Login = (userLogin != null ) ? userLogin.LoginId : operation.LoginId,
                                        returnCode = (int)code
                                    };
                                    response = new OperationResponse(operationRequest.OperationCode, rObj);
                                    application.LogedInUsers.OnUserLoggedIn(rObj.Login, rObj.GameRefId, rObj.GameRefId, rObj.Login);
                                    login = rObj.Login;
                                } else {
                                    var rObj = new LoginOperationResponse {
                                        GameRefId = userLogin.GameRefId,
                                        Login = userLogin.LoginId,
                                        returnCode = (int)code
                                    };
                                    response = new OperationResponse(operationRequest.OperationCode, rObj);
                                    application.LogedInUsers.OnUserLoggedIn(rObj.Login, rObj.GameRefId, rObj.GameRefId, rObj.Login);
                                    login = rObj.Login;
                                }
                                
                            }
                        }
                        break;
                    }
                case OperationCode.GetUsersOnline:
                    {
                        var responseObject = new GetUserOnlineResponse { count = application.LogedInUsers.Count };
                        response = new OperationResponse((byte)OperationCode.GetUsersOnline, responseObject);
                        break;
                    }
            }

            if(response != null ) {
                this.SendOperationResponse(response, sendParameters);
            }
        }
    }
}
