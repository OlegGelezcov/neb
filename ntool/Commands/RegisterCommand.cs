using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ntool.Commands {
    public class RegisterCommand : BaseCommand {

        public RegisterCommand(string source, Application app)
            : base(source, app) { }

        public override void Execute() {
            try {
                app.logger.Log("executin RegisterCommand()");

                string login, password, email, facebookId, vkontakteId;
                GetCommandArguments<string, string, string, string, string>(out login, out password, out email, out facebookId, out vkontakteId);
                app.logger.Log("login = '{0}', password = '{1}', email = '{2}', facebook = '{3}', vkontakte = '{4}'", login, password, email, facebookId, vkontakteId);
                app.Operation(NebulaCommon.ServerType.Login, (byte)OperationCode.RegisterUser, new Dictionary<byte, object> {
                    { (byte)ParameterCode.Login, login },
                    { (byte)ParameterCode.Password, password },
                    { (byte)ParameterCode.Email, email },
                    { (byte)ParameterCode.FacebookId, facebookId },
                    { (byte)ParameterCode.VkontakteId, vkontakteId }
                });

            } catch(Exception exception) {
                app.logger.PushColor(ConsoleColor.Red);
                app.logger.Log(exception.Message);
                app.logger.Log(exception.StackTrace);
                app.logger.PopColor();
            }
        }
    }
}
