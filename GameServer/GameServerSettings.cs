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
        public string TimedEffectsCollectionName;
        public string PetCollectionName { get; private set; }
        public string ContractCollectionName { get; private set; }
        public string AchievmentCollectionName { get; private set; }
        public string DatabaseQuestCollectionName { get; private set; }
        public string DatabaseDialogCollectionName { get; private set; }

        public static GameServerSettings Default {
            get {
#if LOCAL
                return new GameServerSettings {
                    ConnectRetryInterval = 1000,
                    MasterIPAddress = "192.168.1.4",
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
                    WorldStateCollectionName = "world_state_collection",
                    TimedEffectsCollectionName = "timed_effects",
                    PetCollectionName = "pets",
                    ContractCollectionName = "contracts",
                    AchievmentCollectionName = "achievments",
                    DatabaseQuestCollectionName = "quests",
                    DatabaseDialogCollectionName = "dialogs"
                };
#else
                return new GameServerSettings {
                    ConnectRetryInterval = 1000,
                    MasterIPAddress = "108.61.87.145",
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
                    WorldStateCollectionName = "world_state_collection",
                    TimedEffectsCollectionName = "timed_effects",
                    PetCollectionName = "pets",
                    ContractCollectionName = "contracts",
                    AchievmentCollectionName = "achievments",
                    DatabaseQuestCollectionName = "quests",
                    DatabaseDialogCollectionName = "dialogs"
                };
#endif
            }
        }
    }
}
