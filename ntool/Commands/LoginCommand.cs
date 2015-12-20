using Common;
using System;
using System.Collections.Generic;

namespace ntool.Commands {
    public class LoginCommand : BaseCommand  {
        public LoginCommand(string source, Application app)
            : base(source, app) { }

        public override void Execute() {
            try {
                string login, password, facebookId, vkontakteId;
                byte method;
                GetCommandArguments<string, string, string, string, byte>(out login, out password, out facebookId, out vkontakteId, out method);

                app.Operation(NebulaCommon.ServerType.Login, (byte)OperationCode.Login, new Dictionary<byte, object> {
                    { (byte)ParameterCode.Login, login },
                    { (byte)ParameterCode.Password, password },
                    { (byte)ParameterCode.FacebookId, facebookId },
                    { (byte)ParameterCode.VkontakteId, vkontakteId },
                    { (byte)ParameterCode.Method, method }
                });

            } catch (Exception exception) {
                app.logger.Log(exception.Message);
            }
        }
    }
}
