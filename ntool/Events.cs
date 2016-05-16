using ServerClientCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ntool {
    public static class Events {
        public static event System.Action<LoginReturnCode> e_LoginFailed;
        public static event System.Action<string, string> e_LoginSuccess;

        public static void EventLoginFailed(LoginReturnCode code) {
            if(e_LoginFailed != null ) {
                e_LoginFailed(code);
            }
        }

        public static void EventLoginSuccess(string login, string gameRef ) {
            if(e_LoginSuccess != null ) {
                e_LoginSuccess(login, gameRef);
            }
        }
    }
}
