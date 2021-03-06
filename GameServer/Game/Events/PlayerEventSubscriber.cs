﻿using ExitGames.Logging;
//using Nebula.Game.Components.Quests;
using Nebula.Game.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Game.Events {
    public class PlayerEventSubscriber : EventSubscriber {
        private static ILogger s_Log = LogManager.GetCurrentClassLogger();

        private ContractManager m_ContractManager;
        //private QuestManager m_QuestManager;

        public override void Start() {
            base.Start();
            m_ContractManager = GetComponent<ContractManager>();
            //m_QuestManager = GetComponent<QuestManager>();
        }

        public override bool OnEvent(BaseEvent evt) {

            s_Log.Info("OnEvent Before()".Orange());
            //if (m_QuestManager != null) {
            //    s_Log.Info("OnEvent() Before 2".Orange());
            //    m_QuestManager.OnEvent(evt);
            //} else {
            //    s_Log.Info("QuestManager is null".Orange());
            //}

            s_Log.InfoFormat("OnEvent() handle: {0}", evt.ToString());
            if(m_ContractManager) {
                return m_ContractManager.OnEvent(evt);
            }

            return false;
        }
    }
}
