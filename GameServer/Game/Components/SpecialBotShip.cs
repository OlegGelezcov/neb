using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using Nebula.Server.Components;
using Space.Game.Drop;
using Space.Game.Ship;
using ExitGames.Logging;

namespace Nebula.Game.Components {
    public class SpecialBotShip : BotShip {

        private SpecialBotShipComponentData m_Data;

        private static readonly ILogger s_Log = LogManager.GetCurrentClassLogger();

        public override void Init(BotShipComponentData data) {
            mDifficulty = data.difficulty;
            m_Data = data as SpecialBotShipComponentData;

            if (!initialized) {
                Initialize();
                GenerateModel();
            }
            
            s_Log.Info("Init() on Special Ship".Magenta());
        }

        protected override void GenerateModule(DropManager dropManager, ShipModelSlotType slotType) {
            //base.GenerateModule(dropManager, slotType);

            bool success = false;

            if(m_Data != null ) {
                if(m_Data.moduleList.ContainsKey(slotType)) {
                    ModuleGenList gen;
                    if(m_Data.moduleList.TryGetValue(slotType, out gen)) {
                        ShipModule prevModule;
                        ShipModule module = new ShipModule(gen);
                        shipModel.SetModule(module, out prevModule);
                        success = true;
                        s_Log.InfoFormat("Special Ship {0} generate success".Magenta(), slotType);
                    }
                } else {
                    s_Log.InfoFormat("Special Ship data module list invalid".Magenta());
                }
            } else {
                s_Log.Info("Special Ship data is null".Magenta());
            }
            if(!success) {
                base.GenerateModule(dropManager, slotType);
            }

        }

        public override int behaviourId {
            get {
                return base.behaviourId;
            }
        }
    }
}
