using Common;
using ExitGames.Client.Photon;
using ServerClientCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ntool {
    public static class Extensions {
        public static LoginReturnCode GetLoginReturnCodeParameter(this OperationResponse response, ParameterCode code ) {
            return (LoginReturnCode)(int)response.Parameters[(byte)code];
        }
    }
}
