using Common;
using Login.Operations;
using Photon.SocketServer;
using ServerClientCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

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

            if(!mLoginUtils.IsLoginLengthValid(operation.login)) {
                RegisterUserResponse responseObject = new RegisterUserResponse {
                    login = operation.login,
                    gameRef = string.Empty,
                    returnCode = (int)LoginReturnCode.LoginVeryShort
                };
                return new OperationResponse(request.OperationCode, responseObject);
            }

            if (!mLoginUtils.IsPasswordLengthValid(operation.password)) {
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

            if(!mLoginUtils.IsPasswordCharactersValid(operation.password)) {
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

            if(database.ExistLogin(operation.login)) {
                RegisterUserResponse responseObject = new RegisterUserResponse {
                    login = operation.login,
                    gameRef = string.Empty,
                    returnCode = (int)LoginReturnCode.UserWithSameLoginAlreadyExists
                };
                return new OperationResponse(request.OperationCode, responseObject);
            }

            var emailUser = database.GetExistingUserForEmail(operation.email);
            if(emailUser != null ) {
                RegisterUserResponse responseObject = new RegisterUserResponse {
                    login = operation.login,
                    gameRef = string.Empty,
                    returnCode = (int)LoginReturnCode.UserWithSuchEmailAlreadyExists
                };
                return new OperationResponse(request.OperationCode, responseObject);
            }


            LoginReturnCode code = LoginReturnCode.Ok;
            var dbUser = database.CreateUser(operation.login, operation.password, operation.email, out code);
            application.LogedInUsers.OnUserLoggedIn(dbUser.login, dbUser.gameRef, peer as LoginClientPeer);

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
