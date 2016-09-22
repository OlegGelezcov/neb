using Common;
using ExitGames.Logging;
using Login.Operations;
using Nebula.Server.Login;
using Photon.SocketServer;
using ServerClientCommon;

namespace Login.OperationHandlers {
    public class LoginOperationHandler : BaseOperationHandler  {

        private static readonly ILogger s_Log = LogManager.GetCurrentClassLogger();

        public LoginOperationHandler(LoginApplication app, PeerBase peer)
            : base(app, peer) { }


        private OperationResponse LoginViaFacebook(LoginOperationRequest operation) {

            FacebookId facebookId = new FacebookId(operation.facebookId);
            DbUserLogin user = application.GetUser(facebookId);

            if(user == null ) {
                user = application.DbUserLogins.CreateUser(facebookId);
            }

            string platform = GetPlatform(operation);

            LoginOperationResponse response = new LoginOperationResponse {
                facebookId = user.facebookId,
                GameRefId = user.gameRef,
                Login = user.login,
                returnCode = (int)LoginReturnCode.Ok,
                vkontakteId = user.vkontakteId,
                method = (byte)LoginMethod.facebook
            };

            AddUserToCollection(user, platform);

            return new OperationResponse(operation.OperationRequest.OperationCode, response);
        }

        private OperationResponse LoginViaSteam(LoginOperationRequest operation) {
            SteamId steamId = new SteamId(operation.login);
            DbUserLogin user = application.GetUser(steamId);
            if(user == null ) {
                s_Log.InfoFormat("Create new user for steam id: {0}", steamId);
                user = application.DbUserLogins.CreateUser(steamId);
            } else {
                s_Log.InfoFormat("User with steam id: {0} already exists in database, ok", steamId);
            }

            string platform = GetPlatform(operation);

            LoginOperationResponse response = new LoginOperationResponse {
                facebookId = user.facebookId,
                GameRefId = user.gameRef,
                Login = user.login,
                returnCode = (int)LoginReturnCode.Ok,
                vkontakteId = user.vkontakteId,
                method = (byte)LoginMethod.steam
            };
            AddUserToCollection(user, platform);
            return new OperationResponse(operation.OperationRequest.OperationCode, response);
        }

        private OperationResponse LoginViaVkontakte(LoginOperationRequest operation ) {
            VkontakteId vkontakteId = new VkontakteId(operation.vkontakteId);
            DbUserLogin user = application.GetUser(vkontakteId);
            if(user == null ) {
                user = application.DbUserLogins.CreateUser(vkontakteId);
            }

            string platform = GetPlatform(operation);

            LoginOperationResponse response = new LoginOperationResponse {
                facebookId = string.Empty,
                GameRefId = user.gameRef,
                Login = user.login,
                returnCode = (int)LoginReturnCode.Ok,
                vkontakteId = user.vkontakteId,
                method = (byte)LoginMethod.vkontakte
            };

            AddUserToCollection(user, platform);

            return new OperationResponse(operation.OperationRequest.OperationCode, response);
        }

        private OperationResponse LoginViaServer(LoginOperationRequest operation) {

            string password = StringChiper.Decrypt(operation.encryptedPassword);
            LoginAuth loginAuth = new LoginAuth(operation.login, password);
            DbUserLogin user = application.GetUser(loginAuth);

            string platform = GetPlatform(operation);

            LoginOperationResponse response = null;
            if(user == null ) {
                response = new LoginOperationResponse {
                    facebookId = operation.facebookId,
                    GameRefId = string.Empty,
                    Login = operation.login,
                    method = (byte)LoginMethod.server,
                    returnCode = (int)LoginReturnCode.UserNotFound,
                    vkontakteId = operation.vkontakteId
                };
            } else {
                response = new LoginOperationResponse {
                    facebookId = user.facebookId,
                    GameRefId = user.gameRef,
                    Login = user.login,
                    method = (byte)LoginMethod.server,
                    returnCode = (int)LoginReturnCode.Ok,
                    vkontakteId = user.vkontakteId
                };
                AddUserToCollection(user, platform);
            }
            return new OperationResponse(operation.OperationRequest.OperationCode, response);
        }

        private string GetPlatform(LoginOperationRequest operation) {
            string platform = string.Empty;
            if (operation.platform != null) {
                platform = operation.platform;
            }
            return platform;
        }

        public override OperationResponse Handle(OperationRequest request, SendParameters sendParameters) {
            LoginOperationRequest operation = new LoginOperationRequest(peer.Protocol, request);
            if(!operation.IsValid) {
                return new OperationResponse(request.OperationCode) {
                    ReturnCode = (short)ReturnCode.OperationInvalid,
                    DebugMessage = "Login operation parameters invalid"
                };
            }

            if(operation.login != null ) {
                operation.login = operation.login.ToLower();
            }

            OperationResponse response = null;
            if(operation.method == (byte)LoginMethod.server) {
                response = LoginViaServer(operation);
            } else if( operation.method == (byte)LoginMethod.facebook ) {
                response = LoginViaFacebook(operation);
            } else if( operation.method == (byte)LoginMethod.vkontakte ) {
                response = LoginViaVkontakte(operation);
            } else if(operation.method == (byte)LoginMethod.steam ) {
                response = LoginViaSteam(operation);
            }

            if(response != null ) {
                return response;
            } else {
                LoginOperationResponse data = new LoginOperationResponse {
                    facebookId = operation.facebookId,
                    GameRefId = string.Empty,
                    Login = operation.login,
                    method = operation.method,
                    returnCode = (int)LoginReturnCode.UnknownError,
                    vkontakteId = operation.vkontakteId
                };
                return new OperationResponse(operation.OperationRequest.OperationCode, data);
            }
        }

        private void AddUserToCollection(DbUserLogin user, string platform) {
            FullUserAuth fullAuth = new FullUserAuth(user.login, user.gameRef, user.facebookId, user.vkontakteId);

            (peer as LoginClientPeer).SetLogin(new LoginId(fullAuth.login));
            application.LogedInUsers.OnUserLoggedIn(fullAuth, peer as LoginClientPeer);
            application.stats.OnUserLoggedIn(fullAuth, platform);
            if (user != null) {
                user.IncrementSessionCount();
                user.UpdateLastSessionTime();
                application.SaveUser(user);
            }

        }
    }
}
