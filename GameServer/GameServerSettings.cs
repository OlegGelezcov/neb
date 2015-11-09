using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula {
    public class GameServerSettings {
        public int ConnectRetryInterval;
        public string MasterIPAddress;
        public int OutgoingMasterServerPeerPort;
       // public string DatabaseConnectionString;
        public int UpdateInterval;
        public string DatabaseWorkshopCollectionName;
        public string DatabaseWeaponCollectionName;
        public string DatabaseStatsCollectionName;
        public string DatabaseSkillsCollectionName;
        public string DatabaseShipModelCollectionName;
        public string DatabaseInventoryCollectionName;
        public string DatabaseName;
        public string WorldCollection;
        public string WorldStateCollectionName;
        public string PassiveBonusesCollectionName;


        public static GameServerSettings Default {
            get {
#if LOCAL
                return new GameServerSettings {
                    ConnectRetryInterval = 1000,
                    MasterIPAddress = "192.168.1.102",
                    OutgoingMasterServerPeerPort = 4520,
                    UpdateInterval = 6,
                    DatabaseWorkshopCollectionName = "workshops",
                    DatabaseWeaponCollectionName = "weapons",
                    DatabaseStatsCollectionName = "stats",
                    DatabaseSkillsCollectionName = "skills",
                    DatabaseShipModelCollectionName = "ship_models",
                    DatabaseInventoryCollectionName = "inventories",
                    PassiveBonusesCollectionName = "passive_bonuses",
                    DatabaseName = "nebula",
                    WorldCollection = "world",
                    WorldStateCollectionName = "world_state_collection"
                };
#else
                return new GameServerSettings {
                    ConnectRetryInterval = 1000,
                    MasterIPAddress = "45.63.0.198",
                    OutgoingMasterServerPeerPort = 4520,
                    UpdateInterval = 6,
                    DatabaseWorkshopCollectionName = "workshops",
                    DatabaseWeaponCollectionName = "weapons",
                    DatabaseStatsCollectionName = "stats",
                    DatabaseSkillsCollectionName = "skills",
                    DatabaseShipModelCollectionName = "ship_models",
                    DatabaseInventoryCollectionName = "inventories",
                    PassiveBonusesCollectionName = "passive_bonuses",
                    DatabaseName = "nebula",
                    WorldCollection = "world",
                    WorldStateCollectionName = "world_state_collection"
                };
#endif
            }
        }
    }
}
