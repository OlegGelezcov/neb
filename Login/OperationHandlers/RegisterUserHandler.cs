using Common;
using Login.Operations;
using Nebula.Server.Login;
using Photon.SocketServer;
using ServerClientCommon;

namespace Login.OperationHandlers {
    public class RegisterUserHandler : BaseOperationHandler {

        private readonly LoginTextUtilities mLoginUtils = new LoginTextUtilities();

        public RegisterUserHandler(LoginApplication app, PeerBase peer)
            : base(app, peer) { }

        public override OperationResponse Handle(OperationRequest request, SendParameters sendParameters) {

            RegisterUserRequest operation = new RegisterUserRequest(peer.Protocol, request);
            if(!operation.IsValid) {
                return new OperationResponse(request.OperationCode) {
                    ReturnCode = (short)ReturnCode.InvalidOperationParameter,
                    DebugMessage = operation.GetErrorMessage()
                };
            }
            operation.Prepare();

            if(!mLoginUtils.IsLoginLengthValid(operation.login)) {
                RegisterUserResponse responseObject = new RegisterUserResponse {
                    login = operation.login,
                    gameRef = string.Empty,
                    returnCode = (int)LoginReturnCode.LoginVeryShort
                };
                return new OperationResponse(request.OperationCode, responseObject);
            }

            string password = StringChiper.Decrypt(operation.encryptedPassword);

            if (!mLoginUtils.IsPasswordLengthValid(password)) {
                RegisterUserResponse responseObject = new RegisterUserResponse {
                    login = operation.login,
                    gameRef = string.Empty,
                    returnCode = (int)LoginReturnCode.PasswordVeryShort
                };
                return new OperationResponse(request.OperationCode, responseObject);
            }

            if(!mLoginUtils.IsLoginCharactersValid(operation.login)) {
                RegisterUserResponse responseObject = new RegisterUserResponse {
                    login = operation.login,
                    gameRef = string.Empty,
                    returnCode = (int)LoginReturnCode.LoginHasInvalidCharacters
                };
                return new OperationResponse(request.OperationCode, responseObject);
            }

            if(!mLoginUtils.IsPasswordCharactersValid(password)) {
                RegisterUserResponse responseObject = new RegisterUserResponse {
                    login = operation.login,
                    gameRef = string.Empty,
                    returnCode = (int)LoginReturnCode.PasswordHasInvalidCharacters
                };
                return new OperationResponse(request.OperationCode, responseObject);
            }

            var emailChecker = new RegexUtilities();
            if(!emailChecker.IsValidEmail(operation.email)) {
                RegisterUserResponse responseObject = new RegisterUserResponse {
                    login = operation.login,
                    gameRef = string.Empty,
                    returnCode = (int)LoginReturnCode.EmailInvalid
                };
                return new OperationResponse(request.OperationCode, responseObject);
            }



            var database = application.DbUserLogins;

            if(database.ExistsUser(new LoginId ( operation.login))) {
                RegisterUserResponse responseObject = new RegisterUserResponse {
                    login = operation.login,
                    gameRef = string.Empty,
                    returnCode = (int)LoginReturnCode.UserWithSameLoginAlreadyExists
                };
                return new OperationResponse(request.OperationCode, responseObject);
            }

            var emailUser = database.GetUser(new Email( operation.email));
            if(emailUser != null ) {
                RegisterUserResponse responseObject = new RegisterUserResponse {
                    login = operation.login,
                    gameRef = string.Empty,
                    returnCode = (int)LoginReturnCode.UserWithSuchEmailAlreadyExists
                };
                return new OperationResponse(request.OperationCode, responseObject);
            }


            LoginReturnCode code = LoginReturnCode.Ok;


            LoginAuth loginAuth = new LoginAuth(operation.login, password);
            Email email = new Email(operation.email);
            FacebookId fbId = new FacebookId(operation.facebookId);
            VkontakteId vkId = new VkontakteId(operation.vkontakteId);
           
            var dbUser = database.CreateUser(loginAuth, email, fbId, vkId);

            FullUserAuth fullAuth = new FullUserAuth(loginAuth.login, dbUser.gameRef, fbId.value, vkId.value);


            string platform = string.Empty;
            if(operation.platform != null ) {
                platform = operation.platform;
            }

            (peer as LoginClientPeer).SetLogin(new LoginId(loginAuth.login));
            application.LogedInUsers.OnUserLoggedIn(fullAuth, peer as LoginClientPeer);
            application.stats.OnUserLoggedIn(fullAuth, platform);

            if(code != LoginReturnCode.Ok) {
                RegisterUserResponse responseObject = new RegisterUserResponse {
                    login = operation.login,
                    gameRef = string.Empty,
                    returnCode = (int)code
                };
                return new OperationResponse(request.OperationCode, responseObject);
            }

            RegisterUserResponse successResponseObject = new RegisterUserResponse {
                gameRef = dbUser.gameRef,
                login = dbUser.login,
                returnCode = (int)code
            };
            return new OperationResponse(request.OperationCode, successResponseObject);
        }

    }
}
