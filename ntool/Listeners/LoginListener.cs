using Common;
using ExitGames.Client.Photon;
using ntool.Handlers;
using System.Collections.Generic;

namespace ntool.Listeners {
    public class LoginListener : BaseListener {

        private Dictionary<byte, BaseHandler> m_Handlers;


        public LoginListener(string name,  Application app, ServerInfo serverInfo) 
            : base(name, app, serverInfo) {
            m_Handlers = new Dictionary<byte, BaseHandler>();
            m_Handlers.Add((byte)OperationCode.RegisterUser, new RegisterHandler((byte)OperationCode.RegisterUser, app));
            m_Handlers.Add((byte)OperationCode.Login, new LoginHandler((byte)OperationCode.Login, app));
            m_Handlers.Add((byte)OperationCode.GetUsersOnline, new GetUsersOnlineHandler((byte)OperationCode.GetUsersOnline, app));
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
