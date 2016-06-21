using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ntool.Commands {
    public class GetUsersOnlineCommand : BaseCommand {
        public GetUsersOnlineCommand(string source, Application app) 
            : base(source, app ) { }

        public override void Execute() {
            app.Operation(NebulaCommon.ServerType.Login, (byte)OperationCode.GetUsersOnline, new Dictionary<byte, object>(), true);
        }
    }
}
