using Common;
using ExitGames.Logging;
using GameMath;
using Nebula.Contracts;
using Nebula.Game.Components;
using Nebula.Game.Utils;
using Space.Game;
using Space.Game.Drop;
using Space.Game.Inventory;
using Space.Game.Inventory.Objects;
using Space.Game.Resources;
using System.Collections.Generic;


namespace Nebula.Game.Contracts {

    public class ContractRewardExecutor {

        private static ILogger s_Log = LogManager.GetCurrentClassLogger();

        public List<ServerInventoryItem> GiveRewards(string contractId, MmoActor player) {

            List<ServerInventoryItem> items = new List<ServerInventoryItem>();

            ContractData contractData = player.resource.contracts.GetContract(contractId);
            if(contractData != null ) {
                foreach(var reward in contractData.rewards.rewards) {
                    switch (reward.type) {
                        case Common.ContractRewardType.credits:
                            GiveCreditsReward(player, reward as ContractCreditsDataReward);
                            break;
                        case Common.ContractRewardType.exp:
                            GiveExpReward(player, reward as ContractExpDataReward);
                            break;
                        case Common.ContractRewardType.ore: {
                                var item = GiveOreReward(player, reward as ContractOreDataReward);
                                if(item != null ) {
                                    items.Add(item);
                                }
                            }
                            break;
                        case Common.ContractRewardType.weapon: {
                                var item = GiveWeaponReward(player, reward as ContractWeaponDataReward);
                                if(item != null ) {
                                    items.Add(item);
                                }
                            }
                            break;

                    }
                }
            }

            if(items.Count > 0 ) {
                foreach(var item in items ) {
                    player.Station.StationInventory.Add(item.Object, item.Count);
                }
                player.EventOnStationHoldUpdated();
            }
            return items;
        }

        private void GiveCreditsReward(MmoActor player, ContractCreditsDataReward contractCreditsDataReward) {
            player.ActionExecutor.AddCredits(contractCreditsDataReward.count);
            s_Log.InfoFormat("credits reward complete: {0} added".Color(LogColor.orange), contractCreditsDataReward.count);
        }

        private void GiveExpReward(MmoActor player, ContractExpDataReward expReward ) {
            player.GetComponent<PlayerCharacterObject>().AddExp(expReward.count);
            s_Log.InfoFormat("exp reward complete: {0} added".Color(LogColor.orange), expReward.count);
        }

        private ServerInventoryItem GiveOreReward(MmoActor player, ContractOreDataReward oreReward) {
            int playerLevel = player.GetComponent<CharacterObject>().level;
            OreData data = null;
            foreach(var ore in player.resource.Materials.Ores ) {
                if(ore.Id.Contains(playerLevel.ToString())) {
                    data = ore;
                    break;
                }
            }

            if(data != null ) {
                MaterialObject materialObj = new MaterialObject(data.Id);
                int count = Rand.Int(oreReward.minCount, oreReward.maxCount);
                if(count > 0 ) {
                    ServerInventoryItem item = new ServerInventoryItem(materialObj, count);
                    return item;
                }
            }
            return null;
        }

        private ServerInventoryItem GiveWeaponReward(MmoActor player, ContractWeaponDataReward weaponReward ) {
            var playerCharacter = player.GetComponent<CharacterObject>();
            WeaponDropper.WeaponDropParams dropParams = new WeaponDropper.WeaponDropParams(
                player.resource,
                playerCharacter.level,
                (Workshop)playerCharacter.workshop,
                WeaponDamageType.damage,
                Difficulty.none
                );
            ColorInfo colorInfo = player.resource.ColorRes.Color(ColoredObjectType.Weapon, weaponReward.color);
            DropManager dropManager = DropManager.Get(player.resource);
            WeaponDropper weaponDropper = dropManager.GetWeaponDropper(dropParams);
            WeaponObject weapon = weaponDropper.DropWeapon(colorInfo);
            return new ServerInventoryItem(weapon, 1);
        }
    }
}
