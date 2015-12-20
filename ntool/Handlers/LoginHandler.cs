using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExitGames.Client.Photon;
using Common;
using ServerClientCommon;

namespace ntool.Handlers {
    public class LoginHandler : BaseHandler {

        public LoginHandler(byte code, Application context ) 
            : base(code, context ) { }

        public override void Handle(OperationResponse response) {
            LogResponse(response);
            if(response.ReturnCode == (short)ReturnCode.Ok ) {
                LoginReturnCode loginCode = (LoginReturnCode)(int)response.Parameters[(byte)ParameterCode.Status];
                if (loginCode == LoginReturnCode.Ok) {
                    string login = response.Parameters[(byte)ParameterCode.Login] as string;
                    string gameRef = response.Parameters[(byte)ParameterCode.GameRefId] as string;
                    app.player.SetLogin(login);
                    app.player.SetGameRef(gameRef);

                    app.logger.PushColor(ConsoleColor.Gray);
                    app.logger.Log("{0}->{1} successfully logged in", app.player.login, app.player.gameRef);
                    app.logger.PopColor();
                }

            }
        }
    }
}
