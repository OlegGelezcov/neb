using Common;
using ExitGames.Logging;
using Login.Operations;
using Nebula.Server.Login;
using Photon.SocketServer;
using ServerClientCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Login.OperationHandlers {
    public class RecoverUserOperationHandler : BaseOperationHandler {

        public const string SMTP_SERVER = "smtp.yandex.com";
        public const int SMTP_SERVER_PORT = 587;
        public const string SOURCE_USER = "support@depielco.com";
        public const string SOURCE_PASSWORD = "ks00ts14";

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private Task task;

        public RecoverUserOperationHandler(LoginApplication app, PeerBase peer)
            : base(app, peer ) { }

        public override OperationResponse Handle(OperationRequest request, SendParameters sendParameters) {
            RecoverUserRequest operation = new RecoverUserRequest(peer.Protocol, request);
            if(!operation.IsValid) {
                return new OperationResponse(request.OperationCode) {
                    ReturnCode = (short)ReturnCode.InvalidOperationParameter,
                    DebugMessage = operation.GetErrorMessage()
                };
            }

            RegexUtilities emailChecker = new RegexUtilities();
            if(!emailChecker.IsValidEmail(operation.email)) {
                RecoverUserResponse responseObject = new RecoverUserResponse {
                     returnCode = (int)LoginReturnCode.EmailInvalid
                };
                return new OperationResponse(request.OperationCode, responseObject);
            }

            var database = application.DbUserLogins;
            DbUserLogin dbUser = database.GetUser(new Email(operation.email));
            if(dbUser == null ) {
                RecoverUserResponse responseObject = new RecoverUserResponse {
                    returnCode = (int)LoginReturnCode.UserNotFound
                };
                return new OperationResponse(request.OperationCode, responseObject);
            }

            EmailSender sender = new EmailSender(SMTP_SERVER, SMTP_SERVER_PORT, SOURCE_USER, SOURCE_PASSWORD);
            string title = ComposeMailMessageTitle(operation.language);
            string body = ComposeMailMessageBody(dbUser, operation.language);


            task = Task.Factory.StartNew(() => {
                bool result = sender.SendMessage(dbUser.email, title, body);
                log.InfoFormat("message sended with status = {0}", result);
            });

            RecoverUserResponse successResponseObject = new RecoverUserResponse {
                returnCode = (int)LoginReturnCode.Ok
            };
            return new OperationResponse(request.OperationCode, successResponseObject);
        }

        private string ComposeMailMessageTitle(string language) {
            switch(language) {
                case "ru":
                    return "Восстановление аккаунта в Туманность Онлайн";
                default:
                    return "Accaunt recovering in Nebula Online";
            }
        }

        private string ComposeMailMessageBody(DbUserLogin user, string language) {
            switch(language) {
                case "ru":
                    return string.Format("Здесь данные аккаунта, привязанного к этой почте в игре Туманность Онлайн{0}Логин: {1}{0}Пароль: {2}{0}Ждем Вас в игре!",
                        Environment.NewLine, user.login, user.password);
                default:
                    return string.Format("Here accaunt data for this email at game Nebula Online{0}Login: {1}{0}Password: {2}{0}Wait you in game!",
                        Environment.NewLine, user.login, user.password);
            }
        }
    }
}
