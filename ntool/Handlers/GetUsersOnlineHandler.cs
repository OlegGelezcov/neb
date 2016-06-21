using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExitGames.Client.Photon;
using Common;

namespace ntool.Handlers {
    public class GetUsersOnlineHandler : BaseHandler {
        public GetUsersOnlineHandler(byte code, Application context)
            : base(code, context) { }
        public override void Handle(OperationResponse response) {
            int count = (int)response.Parameters[(byte)ParameterCode.Count];
            Events.EventUsersOnlineReceived(count);
        }
    }
}
