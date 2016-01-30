using ExitGames.Logging;
using Nebula.Game.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Game.Events {
    public class PlayerEventSubscriber : EventSubscriber {
        private static ILogger s_Log = LogManager.GetCurrentClassLogger();

        private ContractManager m_ContractManager;

        public override void Start() {
            base.Start();
            m_ContractManager = GetComponent<ContractManager>();
        }

        public override bool OnEvent(BaseEvent evt) {
            s_Log.InfoFormat("OnEvent() handle: {0}", evt.ToString());
            if(m_ContractManager) {
                return m_ContractManager.OnEvent(evt);
            }
            return false;
        }
    }
}
