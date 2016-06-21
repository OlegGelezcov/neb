using Common;
using ExitGames.Client.Photon;
using ntool.Handlers;
using System.Collections.Generic;

namespace ntool.Listeners {
    public class SelectCharacterListener : BaseListener {

        private Dictionary<byte, BaseHandler> m_Handlers;

        public SelectCharacterListener(string name,  Application app, ServerInfo serverInfo)
            : base(name,  app, serverInfo) {
            m_Handlers = new Dictionary<byte, BaseHandler>();
            m_Handlers.Add((byte)SelectCharacterOperationCode.GetCharacters, new GetCharactersHandler((byte)SelectCharacterOperationCode.GetCharacters, app));
        }

        public override void OnEvent(EventData eventData) {
        }
        public override void OnOperationResponse(OperationResponse operationResponse) {
            if(m_Handlers.ContainsKey(operationResponse.OperationCode)) {
                m_Handlers[operationResponse.OperationCode].Handle(operationResponse);
            } else {
                LogMissingHandler(operationResponse);
            }
        }
    }
}
