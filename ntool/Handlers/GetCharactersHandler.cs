using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExitGames.Client.Photon;
using Common;
using Nebula.Client;

namespace ntool.Handlers {
    public class GetCharactersHandler : BaseHandler {
        public GetCharactersHandler(byte code, Application context)
            : base(code, context ) { }

        public override void Handle(OperationResponse response) {
            LogResponse(response);
            if(response.ReturnCode == (short)ReturnCode.Ok ) {
                Hashtable characterHash = response.Parameters[(byte)ParameterCode.Characters] as Hashtable;
                ClientPlayerCharactersContainer playerCharacterContainer = new ClientPlayerCharactersContainer(characterHash);
                Events.EventCharactersReceived(playerCharacterContainer);
            } else {
                Events.EventCharacterReceiveFail();
            }
        }
    }
}
