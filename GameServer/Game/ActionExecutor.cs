
namespace Space.Game {
    using Common;
    using ExitGames.Concurrency.Fibers;
    using ExitGames.Logging;
    using GameMath;
    using Nebula.Drop;
    using Nebula.Engine;
    using Nebula.Game;
    using Nebula.Game.Bonuses;
    using Nebula.Game.Components;
    using Nebula.Game.Components.PlanetObjects;
    using Nebula.Game.Events;
    using Nebula.Inventory;
    using Nebula.Inventory.Objects;
    using Nebula.Server.Components;
    using Nebula.Server.Nebula.Server.Components.AI;
    using Nebula.Server.Nebula.Server.Components.PlanetObjects;
    using ServerClientCommon;
    using Space.Game.Drop;
    using Space.Game.Inventory;
    using Space.Game.Inventory.Objects;
    using Space.Game.Ship;
    using Space.Server;
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    public class ActionExecutor 
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private MmoActor _actor;
        private object syncObject = new object();
        private PetOperations m_PetOps;
        private ContractOperations m_ContractOps;

        public ActionExecutor(MmoActor actor) 
        {
            _actor = actor;
            m_PetOps = new PetOperations(_actor);
            m_ContractOps = new ContractOperations(_actor);
        }

        public Hashtable ChangeControlState(byte state)
        {
            //log.InfoFormat("control state try change to = {0}", (PlayerState)state);

            switch ((PlayerState)state)
            {
                case PlayerState.Idle:
                    {
                        //if we setup shift - remove it first
                        var ai = Player.GetComponent<AIState>();
                        if(ai.shiftState.keyPressed) {
                            ai.shiftState.OnKeyUp();
                        }
                        //and set idle state after this
                        ai.SetControlState((PlayerState)state);
                    }
                    break;
                case PlayerState.JumpToTarget:
                    {
                        if(Player.GetComponent<PlayerTarget>().hasTarget) {
                            Player.GetComponent<AIState>().SetControlState(PlayerState.JumpToTarget);
                        } else {
                            Player.GetComponent<AIState>().SetControlState(PlayerState.Idle);
                        }
                    }
                    break;
                case PlayerState.MoveDirection:
                    {
                        Player.GetComponent<AIState>().SetControlState(PlayerState.MoveDirection);
                    }
                    break;
                case PlayerState.OrbitToTarget:
                    {
                        if(Player.GetComponent<PlayerTarget>().hasTarget) {
                            Player.GetComponent<AIState>().SetControlState(PlayerState.OrbitToTarget);
                        } else {
                            Player.GetComponent<AIState>().SetControlState(PlayerState.Idle);
                        }
                    }
                    break;
            }
            return new Hashtable {
                { (int)SPC.ControlState, (byte)Player.GetComponent<AIState>().controlState},
                { (int)SPC.MaxHitSpeed, Player.GetComponent<MovableObject>().maximumSpeed },
                { (int)SPC.Speed, Player.GetComponent<MovableObject>().speed }
            };
        }

        public Hashtable SetTarget(bool hasTarget, string targetId, byte targetType)
        {
            var target = Player.GetComponent<PlayerTarget>();

            bool oldHasTarget = target.hasTarget;
            string oldTargetId = target.targetId;

            target.SetTarget(targetId, targetType);

            return new Hashtable{
                {ACTION_RESULT.RESULT, ACTION_RESULT.SUCCESS },
                {ACTION_RESULT.RETURN, new Hashtable {
                    {(byte)PS.HasTarget, target.hasTarget },
                    {(byte)PS.TargetId, target.targetId },
                    {(byte)PS.TargetType, target.targetType }
                }}
            };
        }

        public Hashtable UseSkill(int index)
        {
            bool success = Player.GetComponent<PlayerSkills>().UseSkill(index, Player.GetComponent<PlayerTarget>().hasTarget ? Player.GetComponent<PlayerTarget>().targetObject : Player.nebulaObject);
            return new Hashtable { { (int)SPC.IsSuccess, success }, { (int)SPC.Index, index } };
        }



        /// <summary>
        /// Return all bonuses on owner
        /// </summary>
        /// <returns></returns>
        public Hashtable GetBonuses() {
            //get current bonuses
            var bonuses = this.Player.GetComponent<PlayerBonuses>().bonuses;
            Hashtable result = new Hashtable();

            var bons = Player.GetComponent<PlayerBonuses>();
            float time = Time.curtime();
            //go through all bonuses and add bonus type and total value to result table
            foreach (var pair in bonuses) {
                result.Add((byte)pair.Key, bons.Value(pair.Key));
            }
            return result;
        }

        /// <summary>
        /// Return all bonuses on owner target item
        /// </summary>
        /// <returns></returns>
        public Hashtable GetBonusesOnTarget()
        {
            Hashtable result = new Hashtable();
            var target = Player.GetComponent<PlayerTarget>();
            if(target.hasTarget & target.targetObject) {
                var bonuses = target.targetObject.GetComponent<PlayerBonuses>();
                if( bonuses ) {
                    foreach(var pair in bonuses.bonuses) {
                        result.Add((byte)pair.Key, bonuses.Value(pair.Key));
                    }
                }
            }
            return result;
        }

        public Hashtable GetSkillBinding()
        {
            return Player.GetComponent<PlayerSkills>().GetInfo();
        }

        public Hashtable GetInventory() {
            return _actor.Inventory.GetInfo();
        }



        public Hashtable AddFromContainer(string containerId, byte containerType, string objId) 
        {
            Player.nebulaObject.SetInvisibility(false);
            NebulaObject targetObject;
            (Player.nebulaObject.world as MmoWorld).TryGetObject(containerType, containerId, out targetObject);
            if (!targetObject) {
                return new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.ObjectNotFound } };
            }
            IChest chest = targetObject.GetInterface<IChest>();

            if (chest == null) {
                return new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.ObjectNotFound } };
            }

            if(Player.transform.DistanceTo((chest as NebulaBehaviour).transform) > 70 ) {
                return new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.DistanceIsFar } };
            }

            if(Player.Inventory.FreeSlots <= 0 ) {
                return new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.LowInventorySpace } };
            }

            ServerInventoryItem obj;
            if(Player.Inventory.AddFromContainer(chest, Player.nebulaObject.Id, objId, out obj)) {
                Player.EventOnInventoryItemAdded(containerId, containerType, new List<IInventoryObject> { obj.Object });
                Player.EventOnInventoryUpdated();

                if(obj.Object.Type == InventoryObjectType.craft_resource && obj.Object.Id == "res_034") {
                    Player.GetComponent<AchievmentComponent>().AddVariable("collected_orange_craft_element", obj.Count);
                }
                Player.GetComponent<PlayerEventSubscriber>().OnEvent(new InventoryItemsAddedEvent(Player.nebulaObject, new List<ServerInventoryItem> { obj }));

                return new Hashtable {
                        {(int)SPC.ContainerId,            containerId     },
                        {(int)SPC.ContainerType,          containerType   },
                        {(int)SPC.ContainerItemId,          objId           },
                        {(int)SPC.ContainerItemType,     (byte)obj.Object.Type  },
                        {(int)SPC.ReturnCode,            (int)RPCErrorCode.Ok },
                        {(int)SPC.Content, chest.ContentRaw(Player.nebulaObject.Id) }
                };
            }
            return new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.UnknownError } };
        }

        public Hashtable AddAllFromContainer(string containerItemId, byte containerItemType)
        {
            try
            {
                Player.nebulaObject.SetInvisibility(false);
                //log.Info("add from container called...");
                Item containerItem;
                if (!this.Player.World.ItemCache.TryGetItem(containerItemType, containerItemId, out containerItem))
                {
                   // log.Error("container not found");
                    return new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.ObjectNotFound } };//GetErrorResponse("EM0011");
                }

                if(Player.transform.DistanceTo(containerItem.transform) > 70) {
                  //  log.Error("Player is very far from container");
                    return new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.DistanceIsFar } }; //GetErrorResponse("EM0011");
                }

                var iChest = containerItem.GetInterface<IChest>();

                if (iChest == null)
                {
                    //log.Info("container don't have IChest interface");
                    return new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.UnknownError} };//GetErrorResponse("EM0012"); 
                } 


                var chest = iChest;

                ConcurrentDictionary<string, ServerInventoryItem> filteredObjects;
                chest.TryGetActorObjects(Player.nebulaObject.Id, out filteredObjects);
                int sourceCount = (filteredObjects != null) ? filteredObjects.Count : 0;
              //  log.InfoFormat("receive {0} of chest objects", sourceCount);

                if(Player.Inventory.FreeSlots <= 0 && sourceCount > 0) {
                    var result =  new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.LowInventorySpace } };
                    result.Add((int)SPC.ContainerId, containerItemId);
                    result.Add((int)SPC.ContainerType, containerItemType);
                    result.Add((int)SPC.Content, chest.ContentRaw(Player.nebulaObject.Id));
                    return result;
                }
                ConcurrentBag<ServerInventoryItem> addedObjects = null;
                if(!Player.Inventory.AddAllFromChest(chest, Player.nebulaObject.Id, out addedObjects)) {
                    //   log.Info("error of adding from container objects");
                    return new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.UnknownError} };//GetErrorResponse("EM0016");
                }

                var achievments = Player.GetComponent<AchievmentComponent>();
                foreach (var addedInvItem in addedObjects) {
                    if (addedInvItem.Object.Type == InventoryObjectType.craft_resource && addedInvItem.Object.Id == "res_034") {
                        achievments.AddVariable("collected_orange_craft_element", addedInvItem.Count);
                    }
                    
                }

                if (addedObjects.Count > 0)
                {
                    foreach (var obj in addedObjects)
                    {
                        this.Player.EventOnInventoryItemAdded(containerItemId, containerItemType, new List<IInventoryObject> { obj.Object });
                    }
                    this.Player.EventOnInventoryUpdated();
                    Player.GetComponent<PlayerEventSubscriber>().OnEvent(new InventoryItemsAddedEvent(Player.nebulaObject, addedObjects.ToList() ));
                }

                if (addedObjects.Count < sourceCount)
                {
                    var result = new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.LowInventorySpace } };
                    result.Add((int)SPC.ContainerId, containerItemId);
                    result.Add((int)SPC.ContainerType, containerItemType);
                    result.Add((int)SPC.Content, chest.ContentRaw(Player.nebulaObject.Id));
                    return result;
                    //return new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.LowInventorySpace } };//GetSuccessResponse("Need {0} free slots in inventory".f(sourceCount - addedObjects.Count));
                }

                Hashtable res = new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.Ok } }; //GetSuccessResponse("All added successfully");
                res.Add((int)SPC.ContainerId, containerItemId);
                res.Add((int)SPC.ContainerType, containerItemType);
                res.Add((int)SPC.Content, chest.ContentRaw(Player.nebulaObject.Id));
                return res;
            }
            catch(Exception ex)
            {
                CL.Out(LogFilter.PLAYER, ex.Message);
                CL.Out(LogFilter.PLAYER, ex.StackTrace);
            }
            return new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.UnknownError } }; //GetErrorResponse("Exception occured");
        }


        public Hashtable MoveAsteroidToInventory(string asteroidId )
        {
            Player.nebulaObject.SetInvisibility(false);
            NebulaObject asteroidObject = null;
            if(!Player.nebulaObject.world.TryGetObject((byte)ItemType.Asteroid, asteroidId, out asteroidObject)) {
                return new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.ObjectNotFound } }; 
            }

            if (Player.transform.DistanceTo(asteroidObject.transform) > 70) {
                return new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.DistanceIsFar } };
            }

            AsteroidComponent asteroidComponent = asteroidObject.GetComponent<AsteroidComponent>();
            if(!asteroidComponent) {
                return new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.UnknownError } };
            }

            int freeSlots = this.Player.Inventory.FreeSlots;
            int slotsForItems = this.Player.Inventory.SlotsForItems(asteroidComponent.contentDictionary);

            if(freeSlots >= slotsForItems) {
                int totalCount = 0;
                foreach(var c in asteroidComponent.content) {
                    Player.Inventory.Add(c.Material, c.Count);
                    totalCount += c.Count;
                }

                asteroidComponent.ClearContent();
                asteroidComponent.DestroyIfEmpty();
                Player.EventOnInventoryUpdated();

                var achievments = Player.GetComponent<AchievmentComponent>();
                if (achievments != null) {
                    achievments.OnOreCollected(totalCount);
                }

                var res = new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.Ok } };
                res.Add((int)SPC.ContainerId, asteroidComponent.nebulaObject.Id);
                res.Add((int)SPC.ContainerType, asteroidComponent.nebulaObject.Type);
                res.Add((int)SPC.Content, asteroidComponent.contentRaw);
                return res;
            } else {
                return new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.LowInventorySpace } };
            }
        }

        public Hashtable MoveAsteroidItemToInventory(string asteroidId, string itemId, byte inventoryObjectType )
        {
            Player.nebulaObject.SetInvisibility(false);
            var messager = Player.GetComponent<MmoMessageComponent>();
            NebulaObject obj;
            if(!Player.nebulaObject.world.TryGetObject((byte)ItemType.Asteroid, asteroidId, out obj)) {
                return new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.ObjectNotFound } };
            }

            if(Player.transform.DistanceTo(obj.transform) > 70 ) {
                return new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.DistanceIsFar } };
            }
            AsteroidComponent asteroid = obj.GetComponent<AsteroidComponent>();
            if(!asteroid) {
                return new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.UnknownError } };
            }
            if(!asteroid.Contains(itemId, (InventoryObjectType)inventoryObjectType))
            {
                return new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.ObjectNotFound } };
            }

            Dictionary<string, InventoryObjectType> contentDictionary = new Dictionary<string, InventoryObjectType>
            {
                {itemId, inventoryObjectType.toEnum<InventoryObjectType>() }
            };

            int freeSlots = this.Player.Inventory.FreeSlots;
            int slotsForContent = this.Player.Inventory.SlotsForItems(contentDictionary);
            if(freeSlots < slotsForContent)
            {
                return new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.LowInventorySpace } };
            }

            var contentObject = asteroid.ContentObject(itemId);
            asteroid.RemoveContentObject(contentObject.Material.Id);

            int cnt = contentObject.Count;
            this.Player.Inventory.Add(contentObject.Material, contentObject.Count);

            var achievments = Player.GetComponent<AchievmentComponent>();
            if(achievments != null ) {
                achievments.OnOreCollected(cnt);
            }

            asteroid.DestroyIfEmpty();

            this.Player.EventOnInventoryUpdated();

            return new Hashtable {
                { (int)SPC.ReturnCode, (int)RPCErrorCode.Ok },
                {(int)SPC.ContainerId, asteroid.nebulaObject.Id},
                {(int)SPC.ContainerType, ItemType.Asteroid.toByte()},
                {(int)SPC.ContainerItemId, itemId},
                {(int)SPC.ContainerItemType, inventoryObjectType},
                {(int)SPC.Content, asteroid.contentRaw}
            };
        }

        public Hashtable MoveAllFromInventoryToStation() {
            var inventory = Player.Inventory;
            var station = Player.Station.StationInventory;

            List<string> itemCheckList = new List<string>();
            List<ServerInventoryItem> removeFromInventory = new List<ServerInventoryItem>();

            foreach(var pTyped in inventory.Items) {

                //exclude contract items from moving
                if(pTyped.Key == InventoryObjectType.contract_item) {
                    continue;
                }

                foreach(var pItem in pTyped.Value) {

                    itemCheckList.Clear();
                    itemCheckList.Add(pItem.Value.Object.Id);

                    if(station.HasSlotsForItems(itemCheckList)) {

                        if(station.Add(pItem.Value.Object, pItem.Value.Count)) {
                            removeFromInventory.Add(pItem.Value);
                        }

                    }

                }

            }

            foreach(var movedItem in removeFromInventory) {
                inventory.Remove(movedItem.Object.Type, movedItem.Object.Id, movedItem.Count);
            }

            if(removeFromInventory.Count > 0 ) {
                Player.EventOnStationHoldUpdated();
                Player.EventOnInventoryUpdated();
            }
            return new Hashtable {
                { (int)SPC.ReturnCode, (int)RPCErrorCode.Ok } ,
                { (int)SPC.MovedCount, removeFromInventory.Count }
            };
        }

        public Hashtable MoveItemFromInventoryToStation(byte inventoryObjectType, string inventoryItemId, int count )
        {
            ServerInventoryItem sourceItem = null;

            if ( !this.Player.Inventory.TryGetItem((InventoryObjectType)inventoryObjectType, inventoryItemId, out sourceItem) )
            {
               // log.InfoFormat("not founded item: {0} of type: {1}".f(inventoryItemId, (InventoryObjectType)inventoryObjectType));
                return new Hashtable { { ACTION_RESULT.RESULT, ACTION_RESULT.FAIL }, { ACTION_RESULT.MESSAGE, "EM0011" } };
            }

            if(sourceItem.Object.Type == InventoryObjectType.contract_item ) {
                return new Hashtable {
                    { ACTION_RESULT.RESULT, ACTION_RESULT.FAIL },
                    { ACTION_RESULT.MESSAGE, "Don't allow move contract items..."}
                };
            }

            int numberSlotsForItem = this.Player.Station.StationInventory.SlotsForItems(new Dictionary<string, InventoryObjectType> { { inventoryItemId, (InventoryObjectType)inventoryObjectType } });

            if(this.Player.Station.StationInventory.FreeSlots < numberSlotsForItem)
            {
                return new Hashtable { { ACTION_RESULT.RESULT, ACTION_RESULT.FAIL }, { ACTION_RESULT.MESSAGE, "EM0023" } };
            }

            if(sourceItem.Count < count ) {
                return new Hashtable {
                    { ACTION_RESULT.RESULT, ACTION_RESULT.FAIL },
                    { ACTION_RESULT.MESSAGE, "count"}
                };
            }

            if( !this.Player.Station.StationInventory.Add(sourceItem.Object, count))
            {
                return new Hashtable { { ACTION_RESULT.RESULT, ACTION_RESULT.FAIL }, { ACTION_RESULT.MESSAGE, "EM0024" } };
            }

            this.Player.Inventory.Remove((InventoryObjectType)inventoryObjectType, inventoryItemId, count);
            this.Player.EventOnInventoryUpdated();
            this.Player.EventOnStationHoldUpdated();

            return new Hashtable { { ACTION_RESULT.RESULT, ACTION_RESULT.SUCCESS }, {ACTION_RESULT.MESSAGE, "EM0000"}};
        }

        public Hashtable MoveItemFromStationToInventory(byte inventoryObjectType, string inventoryItemId, int count)
        {
            ServerInventoryItem sourceItem = null;
            if (!this.Player.Station.StationInventory.TryGetItem((InventoryObjectType)inventoryObjectType, inventoryItemId, out sourceItem))
            {
                return new Hashtable { { ACTION_RESULT.RESULT, ACTION_RESULT.FAIL }, { ACTION_RESULT.MESSAGE, "EM0011" } };
            }

            int numberSlotsForItem = this.Player.Inventory.SlotsForItems(new Dictionary<string, InventoryObjectType> { { inventoryItemId, (InventoryObjectType)inventoryObjectType } });

            if (this.Player.Inventory.FreeSlots < numberSlotsForItem)
            {
                return new Hashtable { { ACTION_RESULT.RESULT, ACTION_RESULT.FAIL }, { ACTION_RESULT.MESSAGE, "EM0015" } };
            }

            if(sourceItem.Count < count ) {
                return new Hashtable {
                    { ACTION_RESULT.RESULT, ACTION_RESULT.FAIL},
                    { ACTION_RESULT.MESSAGE, "count"}
                };
            }

            if (!this.Player.Inventory.Add(sourceItem.Object, count))
            {
                return new Hashtable { { ACTION_RESULT.RESULT, ACTION_RESULT.FAIL }, { ACTION_RESULT.MESSAGE, "EM0017" } };
            }

            this.Player.Station.StationInventory.Remove((InventoryObjectType)inventoryObjectType, inventoryItemId, count);
            this.Player.EventOnInventoryUpdated();
            this.Player.EventOnStationHoldUpdated();

            return new Hashtable { { ACTION_RESULT.RESULT, ACTION_RESULT.SUCCESS }, { ACTION_RESULT.MESSAGE, "EM0000" } };
        }


        public static Hashtable GetErrorResponse(string message)
        {
            return new Hashtable { { ACTION_RESULT.RESULT, ACTION_RESULT.FAIL }, { ACTION_RESULT.MESSAGE, message } };
        }

        public static Hashtable GetSuccessResponse(string message, Hashtable retHash = null )
        {
            Hashtable result =  new Hashtable { 
                { ACTION_RESULT.RESULT,     ACTION_RESULT.SUCCESS   }, 
                { ACTION_RESULT.MESSAGE,    message                 },
            };
            if (retHash != null) {
                result.Add(ACTION_RESULT.RETURN, retHash);
            }
            return result;
        }

        public Hashtable RequestContainer(string itemId, byte itemType ) {

            try {
                NebulaObject targetObject;
                (Player.nebulaObject.world as MmoWorld).TryGetObject(itemType, itemId, out targetObject);
                if (!targetObject) {
                    return new Hashtable();
                }


                IChest chest = targetObject.GetInterface<IChest>();

                if (chest == null) {
                    return new Hashtable();
                }

                Hashtable result = chest.GetInfoForActor(Player.nebulaObject.Id);
                return result;
            } catch(Exception ex) {
                return new Hashtable();
            }
        }



        /// <summary>
        /// return ship model full info
        /// </summary>
        /// <returns></returns>
        public Hashtable RequestShipModel() {
            var ship = Player.GetComponent<PlayerShip>();
            /*
            if(ship.shipModel == null || ship.shipModel.GetInfo().Count != 5) {
              //  log.Info("RequestShipModel(): shipModel is null");
                return new Hashtable {
                    { (int)SPC.Info, new Hashtable() }
                };
            }*/
            ship.Load();
            return new Hashtable{
                {(int)SPC.Info, Player.GetComponent<PlayerShip>().shipModel.GetInfo() }
            };
        }

        public Hashtable SetNewRandomSlotModule(byte type) 
        {
            DropManager dropManager = DropManager.Get(Player.nebulaObject.resource);

            var moduleTemplate = Player.resource.ModuleTemplates.RandomModule((Workshop)Player.GetComponent<PlayerCharacterObject>().workshop, type.toEnum<ShipModelSlotType>());
            ModuleDropper.ModuleDropParams dropParams = new ModuleDropper.ModuleDropParams(Player.resource, moduleTemplate.Id, 
                Player.GetComponent<PlayerCharacterObject>().level, Difficulty.none, new Dictionary<string, int>(), ObjectColor.white, string.Empty);

            var module = dropManager.GetModuleDropper(dropParams).Drop() as ShipModule;
            ShipModule prevModule = null;
            Player.GetComponent<PlayerShip>().SetModule(module, out prevModule);
            return new Hashtable { { "status", "ok" } };
        }

        public Hashtable EquipModule(string moduleId )
        {
            var playerCharacter = Player.GetComponent<PlayerCharacterObject>();
            ServerInventoryItem item;
            if(!Player.Station.StationInventory.TryGetItem(InventoryObjectType.Module, moduleId, out item)) {
                return new Hashtable {
                    { (int)SPC.ReturnCode, (int)RPCErrorCode.ObjectNotFound }
                };
            }
            if(playerCharacter.level < item.Object.Level) {
                return new Hashtable {
                    {(int)SPC.ReturnCode, (int)RPCErrorCode.LevelNotEnough},
                    {(int)SPC.Data, item.Object.Level }
                };
            }
            return Player.GetComponent<PlayerShip>().EquipModule(moduleId, _actor.Station);
        }

        /// <summary>
        /// Transform inventory object and move to hold(work for schemes only)
        /// </summary>
        /// <param name="objType"></param>
        /// <param name="inventoryObjectId"></param>
        /// <returns></returns>
        public Hashtable TransformObjectAndMoveToHold(byte objType, string inventoryObjectId )
        {
            ServerInventoryItem schemeInventoryItem = null;

            if (Player.Station.StationInventory.TryGetItem(InventoryObjectType.Scheme, inventoryObjectId, out schemeInventoryItem)) {
                Dictionary<string, int> craftMaterials = ((SchemeObject)schemeInventoryItem.Object).CraftMaterials;
                Workshop schemeWorkshop = ((SchemeObject)schemeInventoryItem.Object).Workshop;
               // log.InfoFormat("scheme workshop: {0}", schemeWorkshop);

                if (craftMaterials == null) {
                   // log.Info("TransformObjectAndMoveToHold(): scheme craft materials is null");
                    craftMaterials = new Dictionary<string, int>();
                }

                bool checkCraftMaterials = Player.Station.StationInventory.CheckCraftMaterials(craftMaterials);

                if(checkCraftMaterials ) {

                    TryChangeSchemeColor((SchemeObject)schemeInventoryItem.Object);

                    var result = Player.Station.StationInventory.TransformScheme(inventoryObjectId, DropManager.Get(Player.resource));

                    foreach (var pairMaterial in craftMaterials) {
                        Player.Station.StationInventory.Remove(InventoryObjectType.Material, pairMaterial.Key, pairMaterial.Value);
                    }
                    if(result[ACTION_RESULT.RESULT].ToString() == ACTION_RESULT.SUCCESS) {
                        Player.EventOnInventoryUpdated();
                        Player.EventOnStationHoldUpdated();

                        var achievments = Player.GetComponent<AchievmentComponent>();
                        if(achievments != null ) {
                            achievments.OnModuleCraft();
                        }

                     //   log.InfoFormat("result module workshop: {0}", result[(int)SPC.Workshop].ToString());
                    }

                    Dictionary<InventoryObjectType, int> dict = new Dictionary<InventoryObjectType, int>();
                    foreach(var  entry in Player.Station.StationInventory.Items) {
                        dict.Add(entry.Key, entry.Value.Count);
                    }
                 //   log.Info(dict.toHash().ToStringBuilder().ToString());

                    return result;
                } else {
                   // log.Info("don't has craft materials");
                    return GetErrorResponse("not needed crafting materials");
                }
            } else {
              //  log.Info("don't found scheme in station inventory");
                return GetErrorResponse("not founded scheme in inventory");
            }

        }

        private void TryChangeSchemeColor(SchemeObject scheme) {
            var passiveBonuses = Player.GetComponent<PassiveBonusesComponent>();
            if(passiveBonuses.craftColoredModuleTier > 0 ) {
                scheme.TryChangeColor(passiveBonuses.craftColorredModuleBonus);
            }
        }

        public Hashtable GetStation()
        {
            return this._actor.Station.GetInfo();
        }

        public Hashtable AddScheme() {
           // log.Info("AddScheme() called");
            ShipModelSlotType slotType = CommonUtils.RandomSlotType();
            return AddSchemeAtSlot(slotType);
        }

        public Hashtable AddSchemeAtSlot(ShipModelSlotType slotType)
        {
            //get random workshop
            Workshop workshop = (Workshop)Player.GetComponent<PlayerCharacterObject>().workshop;

            //Create drop manager for random workshop
            DropManager dropMgr = DropManager.Get(Player.resource);

            //Get module for random slot
            ModuleInfo info = Player.resource.ModuleTemplates.RandomModule(workshop, slotType);

            SchemeDropper schemeDropper = dropMgr.GetSchemeDropper(workshop, Player.GetComponent<PlayerCharacterObject>().level);

            var scheme = schemeDropper.Drop() as SchemeObject;

            if (!this.Player.Inventory.HasFreeSpace()) {
                return new Hashtable { 
                    {ACTION_RESULT.RESULT, ACTION_RESULT.FAIL },
                    {ACTION_RESULT.MESSAGE, "EM0015" },
                    {(int)SPC.ErrorMessageId, "EM0015"}
                };
            }

            bool success = this.Player.Inventory.Add(scheme, 1);
            this.Player.EventOnInventoryUpdated();

            if (!success) {
                return new Hashtable{
                    {ACTION_RESULT.RESULT, ACTION_RESULT.FAIL },
                    {ACTION_RESULT.MESSAGE, "EM0017" },
                    {(int)SPC.ErrorMessageId, "EM0017"}
                };
            }

            return new Hashtable{
                {ACTION_RESULT.RESULT, ACTION_RESULT.SUCCESS },
                {ACTION_RESULT.MESSAGE, string.Empty },
                {ACTION_RESULT.RETURN, scheme.Id }
            };
        }

        public Hashtable CmdAddOres() {
            int counter = 0;
            foreach (var oreInfo in Player.resource.Materials.Ores) {
                MaterialObject materialObject = new MaterialObject(oreInfo.Id, (Workshop)Player.GetComponent<PlayerCharacterObject>().workshop, ((MmoWorld)this._actor.World).Zone.Level, oreInfo);
                if (Player.Inventory.Add(materialObject, 1)) {
                    counter++;
                }
            }
           // log.InfoFormat("ores added to inventory: {0}, player inventory = {1}/{2}", counter, Player.Inventory.SlotsUsed, Player.Inventory.MaxSlots);
            this.Player.EventOnInventoryUpdated();

            return GetSuccessResponse("added {0} ores".f(counter));
        }

        /// <summary>
        /// Adding oreCount units all ores to station inventory and schemeCount differenct Schemes
        /// </summary>
        /// <param name="oreCount"></param>
        /// <param name="schemeCount"></param>
        public Hashtable TestAddOreAndSchemesToStation(int oreCount, int schemeCount) {


            Player.application.updater.EnqueueAtUpdateLoop(() => {
                Workshop playerWorkshop = (Workshop)Player.GetComponent<PlayerCharacterObject>().workshop;
                MmoWorld pworld = Player.World as MmoWorld;
                int level = Player.GetComponent<PlayerCharacterObject>().level;

                int counter = 0;
                foreach (var ore in Player.resource.Materials.Ores) {
                    MaterialObject materialObject = new MaterialObject(ore.Id, playerWorkshop, 1, ore);
                    if (Player.Station.StationInventory.HasFreeSpace()) {
                        if (Player.Station.StationInventory.Add(materialObject, oreCount)) {
                            counter++;
                        }
                    }
                }
             //   log.InfoFormat("ores added to inventory = {0}, player inventory = {1}/{2}", counter, Player.Station.StationInventory.SlotsUsed, Player.Station.StationInventory.MaxSlots);


                DropManager dropMgr = DropManager.Get(Player.resource);

                for (int i = 0; i < schemeCount; i++) {

                    ShipModelSlotType slotType = CommonUtils.GetRandomEnumValue<ShipModelSlotType>(new List<ShipModelSlotType>());

                    var moduleTemplate = Player.resource.ModuleTemplates.RandomModule(playerWorkshop, slotType);

                    if (moduleTemplate == null) {
                    //    log.InfoFormat("not found module template for workshop = {0}, slot type = {1}", playerWorkshop, slotType);
                        continue;
                    }

                    var schemeDropper = dropMgr.GetSchemeDropper(playerWorkshop, level);

                    var scheme = schemeDropper.Drop() as SchemeObject;

                    if (Player.Station.StationInventory.HasFreeSpace()) {
                        Player.Station.StationInventory.Add(scheme, 1);
                    } else {
                       // log.InfoFormat("TestAddOreAndSchemesToStation(): Station inventory don't has enough space for scheme");
                    }
                }

                Player.EventOnStationHoldUpdated();

              //  log.Info("enqueued action completed for RPC TestAddOreAndSchemesToStation()");

            });
            return GetSuccessResponse("Ok");
        }

        public Hashtable addne() {
            foreach(var pb in Player.resource.PassiveBonuses.allData) {
                NebulaElementObject neObj = new NebulaElementObject(pb.elementID, pb.elementID);
                Player.Inventory.Add(neObj, 1);
            }
            Player.EventOnInventoryUpdated();
            return new Hashtable {
                { (int)SPC.ReturnCode, RPCErrorCode.Ok }
            };
        }



        public Hashtable GetWeapon()
        {
            return Player.GetComponent<ShipWeapon>().GetInfo();
        }


        /// <summary>
        /// Get player info of player
        /// </summary>
        /// <returns></returns>
        public Hashtable GetPlayerInfo()
        {
            return new Hashtable {
                {(int)SPC.Id,       Player.GetComponent<PlayerCharacterObject>().characterId },
                {(int)SPC.Name,     Player.name },
                {(int)SPC.Workshop, (int)Player.GetComponent<PlayerCharacterObject>().workshop },
                {(int)SPC.Race,     (int)Player.GetComponent<RaceableObject>().race },
                {(int)SPC.Exp,      (int)Player.GetComponent<PlayerCharacterObject>().exp },
                {(int)SPC.Model,    Player.GetComponent<PlayerShip>().shipModel.ModelHash() }
            };
        }

        /// <summary>
        /// Destroy module in station hold and return craft materials to inventory
        /// </summary>
        /// <param name="moduleId"></param>
        /// <returns></returns>
        public Hashtable DestroyModule( string moduleId )
        {
           // log.InfoFormat("destroy module: {0} called", moduleId);

            ServerInventoryItem holdObject = null;
            if(this._actor.Station.StationInventory.TryGetItem(InventoryObjectType.Module, moduleId, out holdObject))
            {
                ShipModule module = holdObject.Object as ShipModule;
                Dictionary<string, int> materials = module.CraftMaterials;
                if(this._actor.Inventory.SlotsForItems(materials.ToDictionary(p => p.Key, p => InventoryObjectType.Material)) <= this._actor.Inventory.FreeSlots)
                {
                    this._actor.Station.StationInventory.Remove(module.Type, module.Id, 1);
                    foreach (var materialPair in materials)
                    {
                        var oreInfo = Player.resource.Materials.Ore(materialPair.Key);
                        if (oreInfo != null)
                        {
                            MaterialObject materialObject = new MaterialObject(oreInfo.Id, (Workshop)Player.GetComponent<PlayerCharacterObject>().workshop, module.Level, oreInfo);
                            int valueToAdd = materialPair.Value / 2;
                            if (valueToAdd > 0)
                            {
                                this._actor.Inventory.Add(materialObject, valueToAdd);
                            }
                        }

                    }
                    this._actor.EventOnStationHoldUpdated();
                    this._actor.EventOnInventoryUpdated();
                    return GetSuccessResponse("Module destroyed successfully");
                }
                else
                {
                    return GetErrorResponse("Inventory don't have enough free slots for materials");
                }
            }
            else
            {
                return GetErrorResponse("Hold not contains such module");
            }
        }



        private Hashtable AddDemoOres(int count)
        {
            foreach(var ore in Player.resource.Materials.Ores)
            {
                var mtrl = new MaterialObject(ore.Id, (Workshop)Player.GetComponent<PlayerCharacterObject>().workshop, ((MmoWorld)this.Player.World).Zone.Level, ore);

                if( false == this.Player.Inventory.Add(mtrl, count))
                {
                    return GetErrorResponse("adding ore to inventory error");
                }
            }
            this.Player.EventOnInventoryUpdated();
            return GetSuccessResponse("ores added successfully");
        }

        public Hashtable AddInventorySlots()
        {
            int maxSlots = this.Player.Inventory.MaxSlots;
            this.Player.Inventory.ChangeMaxSlots(maxSlots + 10);
            this.Player.EventOnInventoryUpdated();
            return new Hashtable{
                {ACTION_RESULT.RESULT, ACTION_RESULT.SUCCESS},
                {ACTION_RESULT.MESSAGE, "OK"},
                {ACTION_RESULT.RETURN, this.Player.Inventory.MaxSlots }
            };
        }

        public Hashtable DestroyInventoryItem(byte inventoryType, byte type, string itemId, int count)
        {
            var targetInventory = (((InventoryType)inventoryType) == InventoryType.ship) ? this.Player.Inventory : this.Player.Station.StationInventory;

            int inventoryCount = targetInventory.ItemCount(type.toEnum<InventoryObjectType>(), itemId);
            if(inventoryCount < count ) {
                return GetErrorResponse("count");
            }

            //if(type == (byte)InventoryObjectType.contract_item) {
            //    return GetErrorResponse("type");
            //}

            if(count > 0 )
            {
                targetInventory.Remove(type.toEnum<InventoryObjectType>(), itemId, count);

                if (inventoryType == (byte)InventoryType.ship) {
                    this.Player.EventOnInventoryUpdated();
                } else if (inventoryType == (byte)InventoryType.station) {
                    this.Player.EventOnStationHoldUpdated();
                }

                return GetSuccessResponse("removed {0} of item {1}".f(count, itemId));
            }
            else
            {
                return GetErrorResponse("doen't contains item: {0}".f(itemId));
            }
        }

        public Hashtable DestroyInventoryItems(byte inventoryType, Hashtable items) {
            var inventory = (((InventoryType)inventoryType) == InventoryType.ship) ? this.Player.Inventory : this.Player.Station.StationInventory;
            Hashtable removedItems = new Hashtable();
            foreach(DictionaryEntry itemEntry in items) {
                string itemId = itemEntry.Key.ToString();
                byte itemType = (byte)itemEntry.Value;
                int count = inventory.ItemCount((InventoryObjectType)itemType, itemId);
                if(count > 0 ) {
                    inventory.Remove((InventoryObjectType)itemType, itemId, count);
                    removedItems.Add(itemId, itemType);
                } else {
                    return new Hashtable { { ACTION_RESULT.RESULT, ACTION_RESULT.FAIL }, { ACTION_RESULT.RETURN, new Hashtable() } };
                }
            }
            Player.EventOnInventoryUpdated();
            Player.EventOnStationHoldUpdated();
            return new Hashtable { { ACTION_RESULT.RESULT, ACTION_RESULT.SUCCESS }, { ACTION_RESULT.RETURN, removedItems } };
        }

        /// <summary>
        /// Add inventory item to inventory from raw item info (add 1 item)
        /// </summary>
        /// <param name="rawItem"></param>
        /// <returns></returns>
        public Hashtable AddInventoryItem(Hashtable rawItem) {
            int count = 0;
            IInventoryObject item = InventoryUtils.Create(rawItem, out count);
            if(item == null ) {
                return new Hashtable { { ACTION_RESULT.RESULT, ACTION_RESULT.FAIL }, { ACTION_RESULT.MESSAGE, "item invalid"} };
            }
            if (!Player.Inventory.HasFreeSpace()) {
                return new Hashtable { { ACTION_RESULT.RESULT, ACTION_RESULT.FAIL }, { ACTION_RESULT.MESSAGE, "no free space" } };
            }
            if( !Player.Inventory.Add(item, 1)) {
                return new Hashtable { { ACTION_RESULT.RESULT, ACTION_RESULT.FAIL }, { ACTION_RESULT.MESSAGE, "error of adding" } };
            }
            return new Hashtable { { ACTION_RESULT.RESULT, ACTION_RESULT.SUCCESS } };
        }

        private MmoActor Player
        {
            get
            {
                return this._actor;
            }
        }


        public Hashtable EquipWeapon(byte inventoryType, string itemId )
        {
            var targetInventory = (((InventoryType)inventoryType) == InventoryType.ship) ? this.Player.Inventory : this.Player.Station.StationInventory;

            var playerCharacter = Player.GetComponent<PlayerCharacterObject>();

            ServerInventoryItem item;
            if (targetInventory.TryGetItem(InventoryObjectType.Weapon, itemId, out item))
            {
                if(playerCharacter.level < item.Object.Level) {
                    return new Hashtable {
                        {(int)SPC.ReturnCode, (int)RPCErrorCode.LevelNotEnough },
                        {(int)SPC.Data, item.Object.Level }
                    };
                    //return new Hashtable {
                    //    {ACTION_RESULT.RESULT, ACTION_RESULT.FAIL },
                    //    {ACTION_RESULT.MESSAGE, "You level low for weapon" }
                    //};
                }

                WeaponObject weapon = item.Object as WeaponObject;
                if(playerCharacter.workshop != (byte)weapon.workshop) {
                    return new Hashtable {
                        {(int)SPC.ReturnCode, (int)RPCErrorCode.InvalidObjectWorkshop },
                        {(int)SPC.Data, (byte)weapon.workshop }
                    };
                }

                targetInventory.Remove(InventoryObjectType.Weapon, itemId, 1);

                var oldWeapon = Player.GetComponent<ShipWeapon>().SetWeapon(item.Object as WeaponObject);
                if (oldWeapon != null) {
                    targetInventory.Add(oldWeapon, 1);
                }
                this.Player.EventOnInventoryUpdated();
                this.Player.EventOnStationHoldUpdated();
                Player.GetComponent<MmoMessageComponent>().ReceiveServiceMessage(ServiceMessageType.Info,
                    string.Format("Player equipped weapon = {0} and place old weapon to inventory = {1}",
                    item.Object.Id, (InventoryType)inventoryType));

                return new Hashtable {
                    { (int)SPC.ReturnCode, (int)RPCErrorCode.Ok }
                };
            }
            else
            {
                return new Hashtable {
                    {(int)SPC.ReturnCode, (int)RPCErrorCode.ObjectNotFound }
                };
            }
        }


        public Hashtable ClearStationHold()
        {
            this.Player.Station.StationInventory.Clear();
            return GetSuccessResponse("hold successfully");
        }

        public Hashtable ClearInventory()
        {
            this.Player.Inventory.Clear();
            return GetSuccessResponse("inventory cleared");
        }


        /// <summary>
        /// Replace all modules on ship
        /// </summary>
        /// <returns></returns>
        public Hashtable Rebuild()
        {
            DropManager dropManager = DropManager.Get(Player.resource);
            var types = System.Enum.GetValues(typeof(ShipModelSlotType)).Cast<ShipModelSlotType>().ToList();
            int counter = 0;
            foreach(var t in types)
            {
                //dropper.ModuleDropper.Drop()
                var info = Player.resource.ModuleTemplates.RandomModule((Workshop)Player.GetComponent<PlayerCharacterObject>().workshop, t);
                if( info != null )
                {
                    ModuleDropper.ModuleDropParams dropParams = new ModuleDropper.ModuleDropParams(Player.resource, info.Id, Player.GetComponent<PlayerCharacterObject>().level, 
                        Difficulty.none, this.GenerateTestCraftMaterials(), ObjectColor.white, string.Empty);
                    ModuleDropper moduleDropper = dropManager.GetModuleDropper(dropParams);
                    ShipModule newModule = moduleDropper.Drop() as ShipModule;
                    ShipModule oldModule;
                    Player.GetComponent<PlayerShip>().SetModule(newModule, out oldModule);
                    counter++;
                }
            }

            WeaponDropper.WeaponDropParams wDropParams = new WeaponDropper.WeaponDropParams(Player.resource,
                Player.GetComponent<PlayerCharacterObject>().level,
                (Workshop)Player.GetComponent<PlayerCharacterObject>().workshop, WeaponDamageType.damage, Difficulty.none);
            WeaponDropper weaponDropper = dropManager.GetWeaponDropper(wDropParams);
            WeaponObject newWeapon = weaponDropper.Drop() as WeaponObject;

            Player.GetComponent<ShipWeapon>().SetWeapon(newWeapon);

            counter++;

            return new Hashtable { { ACTION_RESULT.RESULT, ACTION_RESULT.SUCCESS }, { ACTION_RESULT.MESSAGE, "number of generated items: {0}".f(counter) } };
        }

        private bool IsSuccessResult(Hashtable result)
        {
            string realResult = result.GetValue<string>(ACTION_RESULT.RESULT, string.Empty);
            if (realResult == ACTION_RESULT.SUCCESS)
                return true;
            else
                return false;
        }



        private Dictionary<string, int> GenerateTestCraftMaterials()
        {
            Dictionary<string, int> craftMaterials = new Dictionary<string, int>();
            string firstOre = Player.resource.Materials.RandomOre().Id;
            string secondOre = Player.resource.Materials.RandomOre().Id;
            while( firstOre == secondOre )
            {
                secondOre = Player.resource.Materials.RandomOre().Id;
            }
            craftMaterials.Add(firstOre, 4);
            craftMaterials.Add(secondOre, 4);
            return craftMaterials;
        }


        public Hashtable GetChatUpdate()
        {
            Hashtable info = this.Player.Chat.GetInfo();
            this.Player.Chat.Clear();
            return info;
        }

        /// <summary>
        /// RPC when player button 'shift' for accelerated movement
        /// </summary>
        /// <returns></returns>
        public Hashtable OnShiftDown()
        {
            var playerAI = Player.GetComponent<AIState>();

            playerAI.SetShift(true);

            return new Hashtable {
                { (int)SPC.ControlState, (byte)playerAI.controlState },
                { (int)SPC.ShiftState, playerAI.shiftState.keyPressed },
                { (int)SPC.Speed, Player.GetComponent<MovableObject>().speed },
                { (int)SPC.MaxHitSpeed, Player.GetComponent<MovableObject>().maximumSpeed }
            };
        }

        /// <summary>
        /// RPC when player unpressed button shift for accelerated movement
        /// </summary>
        /// <returns></returns>
        public Hashtable OnShiftUp()
        {
            var playerAI = Player.GetComponent<AIState>();

            playerAI.SetShift(false);

            return new Hashtable {
                { (int)SPC.ControlState, (byte)playerAI.controlState },
                { (int)SPC.ShiftState, playerAI.shiftState.keyPressed },
                { (int)SPC.Speed, Player.GetComponent<MovableObject>().speed },
                { (int)SPC.MaxHitSpeed, Player.GetComponent<MovableObject>().maximumSpeed }
            };
        }

        public Hashtable Respawn()
        {
            if(!Player.nebulaObject) {
                Player.Respawn();
                return GetSuccessResponse("respawn occured");
            }
            return GetSuccessResponse("respawn error: ship not destroyed");
        }

        /// <summary>
        /// Collect and return basic combar information for fitting window in game
        /// </summary>
        /// <returns></returns>
        public Hashtable GetCombatParams()
        {
            var damagable = Player.GetComponent<ShipBasedDamagableObject>();
            var weapon = Player.GetComponent<ShipWeapon>();
            var energy = Player.GetComponent<ShipEnergyBlock>();
            var ship = Player.GetComponent<PlayerShip>();
            var movable = Player.GetComponent<PlayerShipMovable>();

            Hashtable result = new Hashtable {
                {(int)SPC.MaxHealth, damagable.maximumHealth },
                {(int)SPC.Resist, ship.commonResist },
                {(int)SPC.AcidResist, ship.acidResist },
                {(int)SPC.LaserResist, ship.laserResist},
                {(int)SPC.RocketResist, ship.rocketResist},
                {(int)SPC.Energy, energy.maximumEnergy  },
                {(int)SPC.Damage, weapon.GetDamage(false).totalDamage },
                {(int)SPC.CritChance, weapon.criticalChance },
                {(int)SPC.CritDamage, weapon.GetDamage(true).totalDamage },
                {(int)SPC.Speed, movable.maximumSpeed }
            };
            return result;

        }

        public Hashtable PrintServerStats()
        {
            ServerRuntimeStats.Default(Player.application).OutStats(ConsoleLogContext.Instance);
            return GetSuccessResponse("ok");
        }

        public Hashtable SetRandomBonus() {
            var cBonuses = Player.GetComponent<PlayerBonuses>();
            BonusType randomBonusType = CommonUtils.GetRandomEnumValue<BonusType>(new List<BonusType>());
            Buff buff = new Buff(Guid.NewGuid().ToString(), null, randomBonusType, 10, 0.5f);
            cBonuses.SetBuff(randomBonusType, buff);
            return GetSuccessResponse("ok");
        }

        /// <summary>
        /// Toggle god mode for player ( debug function )
        /// </summary>
        /// <returns></returns>
        public Hashtable TGM() {
            Player.application.updater.EnqueueAtUpdateLoop(() => {
                if(Player) {
                    var damagable = Player.GetComponent<ShipBasedDamagableObject>();
                    damagable.SetGod(!damagable.god);
               //     log.InfoFormat("player set GOD MODE = {0}", damagable.god);
                } else {
                  //  log.Info("TGM(): Player invalid");
                }
            });
            return new Hashtable {
                { ACTION_RESULT.RESULT, ACTION_RESULT.SUCCESS },
                { ACTION_RESULT.RETURN, Player.GetComponent<ShipBasedDamagableObject>().god }
            };
        }

        public Hashtable AddCredits(int credits) {
            var character = Player.GetComponent<PlayerCharacterObject>();
            var characterInfo = Player.GetPlayerCharacter();

            Player.application.updater.CallS2SMethod(NebulaCommon.ServerType.SelectCharacter, "AddCredits", new object[] {
                character.login, Player.nebulaObject.Id, characterInfo.CharacterId, credits
            });
            return GetSuccessResponse("ok");
        }

        [TestRPC]
        public Hashtable SetSkill(string hexStr, byte slotType) {
            int skillID = int.Parse(hexStr, System.Globalization.NumberStyles.HexNumber);
         //   log.InfoFormat("try set skill = {0}", skillID);
            var ship = Player.GetComponent<PlayerShip>();
            ship.shipModel.Slot((ShipModelSlotType)slotType).Module.SetSkill(skillID);
            Player.GetComponent<PlayerSkills>().UpdateSkills(ship.shipModel);
            Player.EventOnShipModelUpdated();
            Player.EventOnSkillsUpdated();
            foreach(var slot in ship.shipModel.Slots) {
               // log.InfoFormat("SLOT = {0}, SKILL = {1}", slot.Type, slot.Module.Skill.ToString("X8"));
            }
            return GetSuccessResponse(string.Empty);
        }

        public Hashtable RequestTeleportJump(string teleportID) {

            Player.application.updater.EnqueueAtUpdateLoop(() => {
                var world = Player.nebulaObject.world as MmoWorld;
                NebulaObject teleportGameObject;
                if (!world.TryGetObject((byte)ItemType.Teleport, teleportID, out teleportGameObject)) {
                    Player.GetComponent<MmoMessageComponent>().ReceiveTeleportJump(new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.ObjectNotFound } });
                    return; 
                }
                var teleport = teleportGameObject.GetComponent<Teleport>();
                if (!teleport) {
                    Player.GetComponent<MmoMessageComponent>().ReceiveTeleportJump(new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.UnknownError } });
                    return;
                }

                if (Player.GetComponent<PlayerTarget>().inCombat) {
                    Player.GetComponent<MmoMessageComponent>().ReceiveTeleportJump(new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.InCombat } });
                    return;
                }

                Player.nebulaObject.SetInvisibility(false);
                Player.GetComponent<MmoMessageComponent>().ReceiveTeleportJump(new Hashtable {
                    { (int)SPC.ReturnCode, (int)RPCErrorCode.Ok },
                    { (int)SPC.Position, teleportGameObject.transform.position.ToArray() },
                    { (int)SPC.ItemId, teleportGameObject.Id }
                });
            });
            return new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.Ok } };
        }

        public Hashtable TestBuffs() {
            foreach(BonusType buff in Enum.GetValues(typeof(BonusType))) {
                Player.GetComponent<PlayerBonuses>().SetBuff(new Buff(Guid.NewGuid().ToString(), null,
                     buff, 20, 0.5f));
            }
            return new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.Ok } };
        }

        public Hashtable multhp(float mult) {
            var targetObject = Player.nebulaObject.Target().targetObject;
            if(targetObject.Damagable()) {
                targetObject.Damagable().ForceSetHealth(targetObject.Damagable().health * mult);
            }
            return new Hashtable();
        }

        public Hashtable invis(int invisibility) {
            NebulaObject target = null;
            if(Player.nebulaObject.Target().hasTarget) {
                target = Player.nebulaObject.Target().targetObject;
            } else {
                target = Player.nebulaObject;
            }

            if(invisibility != 0) {
                target.SetInvisibility(true);
            } else {
                target.SetInvisibility(false);
            }
            return new Hashtable();
        }

        /// <summary>
        /// Testing broadcats message to game servers and after to client about race changes event
        /// </summary>
        /// <returns></returns>
        public Hashtable sendrace() {
            Player.application.updater.SendS2SWorldRaceChanged("H5", (byte)Race.Humans, (byte)Race.Criptizoids);
          //  log.InfoFormat("change race sended to Master [red]");
            return new Hashtable {
                { (int)SPC.ReturnCode, (int)RPCErrorCode.Ok}
            };
        }

        public Hashtable SetRace(int race) {
            Player.nebulaObject.mmoWorld().SetCurrentRace((Race)(byte)race);
            return new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.Ok } };
        }

        public Hashtable addexp(int count) {
            Player.GetComponent<PlayerCharacterObject>().AddExp(count);
            return new Hashtable();
        }

        private int MaxCountTurretsInWorld(MmoWorld world) {
            int outpostCount = world.GetItems((item) => item.GetComponent<MainOutpost>()).Count;
            int fortificationCount = world.GetItems((item) => item.GetComponent<Outpost>()).Count;
            return 9 + outpostCount * 3 + fortificationCount * 3;
        }

        private int MaxCountTurretsAlways {
            get {
                return 9 + 3 + 9;
            }
        }

        public Hashtable StartLearning(int bonusType) {
            var data = Player.nebulaObject.resource.PassiveBonuses.GetData((PassiveBonusType)bonusType);
            if(data == null ) {
                return new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.UnknownError } };
            }

            var passiveBonusesComponent = Player.GetComponent<PassiveBonusesComponent>();
            var playerBonusData = passiveBonusesComponent.GetData((PassiveBonusType)bonusType);
            if(playerBonusData == null ) {
                return new Hashtable {
                    { (int)SPC.ReturnCode, (int)RPCErrorCode.UnknownError }
                };
            }

            if(playerBonusData.tier >= 100 ) {
                return new Hashtable {
                    { (int)SPC.ReturnCode, (int)RPCErrorCode.MaxTierReached }
                };
            }

            int required = (playerBonusData.tier + 1) * data.nebulaElementsForTier;
            int playerCount = Player.Inventory.ItemCount(InventoryObjectType.nebula_element, data.elementID);

            if(playerCount < required) {
                return new Hashtable {
                    { (int)SPC.ReturnCode, (int)RPCErrorCode.DontEnoughInventoryItems}
                };
            }

            if(playerBonusData.learningStarted ) {
                return new Hashtable {
                    { (int)SPC.ReturnCode, (int)RPCErrorCode.LearningAlreadyStarted }
                };
            }

            passiveBonusesComponent.StartLearning((PassiveBonusType)bonusType);
            Player.Inventory.Remove(InventoryObjectType.nebula_element, data.elementID, required);
            Player.EventOnInventoryUpdated();
            return new Hashtable {
                { (int)SPC.ReturnCode, (int)RPCErrorCode.Ok }
            };
        }

        public Hashtable GetPassiveBonuses() {
            Player.GetComponent<PassiveBonusesComponent>().SendUpdate();
            return new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.Ok } };
        }

        public Hashtable GetMiningStationInfo(string miningStationID, byte miningStationType) {
            NebulaObject miningStation;
            if(!Player.nebulaObject.mmoWorld().TryGetObject(miningStationType, miningStationID, out miningStation)) {
                return new Hashtable {
                    { (int)SPC.ReturnCode, (int)RPCErrorCode.ObjectNotFound }
                };
            }
            var miningStationComponent = miningStation.GetComponent<MiningStation>();
            if (!miningStationComponent) {
                return new Hashtable {
                    { (int)SPC.ReturnCode, (int)RPCErrorCode.UnknownError}
                };
            }

            miningStationComponent.SendInfoToPlayer(Player);

            return new Hashtable {
                { (int)SPC.ReturnCode, (int)RPCErrorCode.Ok }
            };
        }

        public Hashtable CollectElementsFromMiningStation(string miningStationID) {

            MmoWorld world = Player.nebulaObject.mmoWorld();

            NebulaObject miningStationObject;
            if(!world.TryGetObject((byte)ItemType.Bot, miningStationID, out miningStationObject)) {
                return new Hashtable {
                    { (int)SPC.ReturnCode, (int)RPCErrorCode.ObjectNotFound }
                };
            }

            var miningStationComponent = miningStationObject.GetComponent<MiningStation>();
            if(!miningStationComponent) {
                return new Hashtable {
                    { (int)SPC.ReturnCode, (int)RPCErrorCode.UnknownError }
                };
            }

            if(Player.nebulaObject.Id != miningStationComponent.ownedPlayer ) {
                return new Hashtable {
                    { (int)SPC.ReturnCode, (int)RPCErrorCode.AccessDenied }
                };
            }

            if(miningStationComponent.currentCount == 0 ) {
                return new Hashtable {
                    { (int)SPC.ReturnCode, (int)RPCErrorCode.IsEmpty }
                };
            }

            if(Player.Inventory.SlotsForItems(
                new Dictionary<string, InventoryObjectType> {
                    { miningStationComponent.nebulaElementID, InventoryObjectType.nebula_element }
                } ) > Player.Inventory.FreeSlots ) {
                return new Hashtable {
                    { (int)SPC.ReturnCode, (int)RPCErrorCode.LowInventorySpace }
                };
            }

            NebulaElementObject element = new NebulaElementObject(miningStationComponent.nebulaElementID, miningStationComponent.nebulaElementID);
            int count = miningStationComponent.currentCount;
            Player.Inventory.Add(element, miningStationComponent.currentCount);
          //  log.InfoFormat("added to inventory = {0} nebula elements [red]", miningStationComponent.currentCount);

            Player.EventOnInventoryUpdated();
            miningStationComponent.MakeEmpty();

            var achievments = Player.GetComponent<AchievmentComponent>();
            if(achievments != null) {
                achievments.OnCollectNebulaElement(count);
            }

            return new Hashtable {
                { (int)SPC.ReturnCode, (int)RPCErrorCode.Ok },
                { (int)SPC.Count, count }
            };

        }


        public Hashtable CreatePersonalBeacon(string itemID) {
            MmoWorld world = Player.nebulaObject.mmoWorld();
            Race playerRace = (Race)Player.nebulaObject.Raceable().race;

            int itemCount = Player.Inventory.ItemCount(InventoryObjectType.personal_beacon, itemID);
            if(itemCount == 0 ) {
                return new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.ObjectNotFound } };
            }
            ServerInventoryItem invItem;
            if(!Player.Inventory.TryGetItem(InventoryObjectType.personal_beacon, itemID, out invItem)) {
                return new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.UnknownError } };
            }

            PersonalBeaconObject beacon = invItem.Object as PersonalBeaconObject;

            int beaconCount = world.GetItems((item) => {
                var itemRaceable = item.Raceable();
                if (item.GetComponent<PersonalBeacon>() && itemRaceable) {
                    if (itemRaceable.race == (byte)playerRace) {
                        return true;
                    }
                }
                return false;
            }).Count;

            if(beaconCount >= 10) {
                return new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.MaxCountOfBeaconsReached } };
            }

            Dictionary<ComponentID, ComponentData> components = new Dictionary<ComponentID, ComponentData> {
                { ComponentID.Model, new ModelComponentData(GetBeaconModel(playerRace)) },
                { ComponentID.NebulaObject, new NebulaObjectComponentData(ItemType.Teleport, new Dictionary<byte, object>(), "", 1, 0 ) },
                { ComponentID.Raceable, new RaceableComponentData(playerRace) },
                { ComponentID.Teleport, new PersonalBeaconComponentData(beacon.interval) }
            };
            NebulaObjectData data = new NebulaObjectData {
                componentCollection = components,
                ID = Guid.NewGuid().ToString(),
                position = Player.transform.position,
                rotation = Vector3.Zero
            };
            var beaconObj = ObjectCreate.NebObject(world, data);
            beaconObj.SetDatabaseSaveable(true);
            beaconObj.AddToWorld();

            Player.Inventory.Remove(InventoryObjectType.personal_beacon, itemID, 1);
            Player.EventOnInventoryUpdated();
            return new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.Ok } };
        }

        private string GetBeaconModel(Race race) {
            switch (race) {
                case Race.Humans:
                    return "TELEPORT";
                case Race.Borguzands:
                    return "BORGUZAND_TELEPORT";
                case Race.Criptizoids:
                    return "KRIPTIZID_TELEPORT";
                default:
                    return "NEUTRAL_TELEPORT";
            }
        }

        public Hashtable ApplyUpgradeToFortification(string fortificationID, string inventoryItemID ) {
            MmoWorld world = Player.nebulaObject.mmoWorld();
            int itemCount = Player.Inventory.ItemCount(InventoryObjectType.fort_upgrade, inventoryItemID);
            if (itemCount == 0) {
                return new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.ObjectNotFound } };
            }
            ServerInventoryItem invItem;
            if(!Player.Inventory.TryGetItem(InventoryObjectType.fort_upgrade, inventoryItemID, out invItem)) {
                return new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.UnknownError } };
            }
            FortUpgradeObject fortificationUpgradeObject = invItem.Object as FortUpgradeObject;

            NebulaObject fortificationObject;
            if(!world.TryGetObject((byte)ItemType.Bot, fortificationID, out fortificationObject)) {
                return new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.FortificationNotFound } };
            }

            var botCharacter = fortificationObject.GetComponent<CharacterObject>();
            if(!botCharacter) {
                return new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.ComponentNotFound } };
            }
            if(botCharacter.level < fortificationUpgradeObject.minLevel || botCharacter.level > fortificationUpgradeObject.maxLevel) {
                return new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.LevelInNotRange } };
            }

            float dist = Player.transform.DistanceTo(fortificationObject.transform);
            if(dist > Player.nebulaObject.Weapon().optimalDistance + fortificationObject.size) {
                return new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.DistanceIsFar } };
            }
            botCharacter.SetLevel(botCharacter.level + 1);
            fortificationObject.GetComponent<Outpost>().SetConstruct(10);
            
            Player.Inventory.Remove(InventoryObjectType.fort_upgrade, inventoryItemID, 1);
            Player.EventOnInventoryUpdated();
            Player.nebulaObject.mmoWorld().SaveWorldState();
            return new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.Ok } };
        }

        public Hashtable ApplyUpgradeToOutpost(string outpostID, string inventoryItemID ) {
            MmoWorld world = Player.nebulaObject.mmoWorld();

            int itemCount = Player.Inventory.ItemCount(InventoryObjectType.out_upgrade, inventoryItemID);
            if(itemCount == 0) {
                return new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.ObjectNotFound } };
            }
            ServerInventoryItem invItem;
            if(!Player.Inventory.TryGetItem(InventoryObjectType.out_upgrade, inventoryItemID, out invItem)) {
                return new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.UnknownError } };
            }

            OutpostUpgradeObject outpostUpgradeObject = invItem.Object as OutpostUpgradeObject;

            NebulaObject outpostObject;
            if(!world.TryGetObject((byte)ItemType.Bot, outpostID, out outpostObject)) {
                return new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.OutpostNotFound } };
            }

            var botCharacter = outpostObject.GetComponent<CharacterObject>();
            if(!botCharacter) {
                return new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.ComponentNotFound } };
            }

            if(botCharacter.level < outpostUpgradeObject.minLevel || botCharacter.level > outpostUpgradeObject.maxLevel ) {
                return new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.LevelInNotRange } };
            }

            float dist = Player.transform.DistanceTo(outpostObject.transform);
            if(dist > Player.nebulaObject.Weapon().optimalDistance + outpostObject.size) {
                return new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.DistanceIsFar } };
            }

            botCharacter.SetLevel(botCharacter.level + 1);
            outpostObject.GetComponent<MainOutpost>().SetConstruct(10);
            Player.Inventory.Remove(InventoryObjectType.out_upgrade, inventoryItemID, 1);
            Player.EventOnInventoryUpdated();
            Player.nebulaObject.mmoWorld().SaveWorldState();
            return new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.Ok } };
        }

        /// <summary>
        /// Apply repair patch to player
        /// </summary>
        public Hashtable ApplyRepairPatch(string itemID) {
            float interval = 30.0f;

            int itemCount = Player.Inventory.ItemCount(InventoryObjectType.repair_patch, itemID);
            if(itemCount == 0) {
                return new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.ObjectNotFound } };
            }
            ServerInventoryItem invItem;
            if(!Player.Inventory.TryGetItem(InventoryObjectType.repair_patch, itemID, out invItem)) {
                return new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.UnknownError } };
            }

            RepairPatchObject repairPatchObject = invItem.Object as RepairPatchObject;
            var shipDamagable = Player.GetComponent<ShipBasedDamagableObject>();
            shipDamagable.SetIncreaseRegenMultiplier(repairPatchObject.value, interval);
            Player.Inventory.Remove(InventoryObjectType.repair_patch, itemID, 1);
            Player.EventOnInventoryUpdated();

            return new Hashtable {
                { (int)SPC.ReturnCode, (int)RPCErrorCode.Ok },
                { (int)SPC.Interval, interval},
                { (int)SPC.Count, (int)repairPatchObject.value}
            };
        }

        /// <summary>
        /// Applied restore kit to player
        /// </summary>
        public Hashtable ApplyRepairKit(string itemID) {
            int itemCount = Player.Inventory.ItemCount(InventoryObjectType.repair_kit, itemID);
            if(itemCount == 0) {
                return new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.ObjectNotFound } };
            }
            ServerInventoryItem invItem;
            if(!Player.Inventory.TryGetItem(InventoryObjectType.repair_kit, itemID, out invItem)) {
                return new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.UnknownError } };
            }

            RepairKitObject repairKitObject = invItem.Object as RepairKitObject;
            var damagable = Player.nebulaObject.Damagable();
            float restoreHealth = damagable.maximumHealth * repairKitObject.value;
            damagable.ForceSetHealth(damagable.health + restoreHealth);
            Player.Inventory.Remove(InventoryObjectType.repair_kit, itemID, 1);
            Player.EventOnInventoryUpdated();
            return new Hashtable {
                { (int)SPC.ReturnCode, (int)RPCErrorCode.Ok },
                { (int)SPC.Count, (int)restoreHealth }
            };
        }

        public Hashtable CreateMiningStation(string planetID, string inventoryItemID, int slotNumber) {

            MmoWorld world = Player.nebulaObject.mmoWorld();
            Race playerRace = (Race)Player.nebulaObject.Raceable().race;

            //find target planet 
            NebulaObject planetObject;
            if (false == world.TryGetObject((byte)ItemType.Bot, planetID, out planetObject)) {
                return new Hashtable {
                    { (int)SPC.ReturnCode, (int)RPCErrorCode.PlanetNotFounded }
                };
            }

            PlanetObject planetComponent = planetObject.GetComponent<PlanetObject>();
            if (!planetComponent) {
                return new Hashtable {
                    { (int)SPC.ReturnCode, (int)RPCErrorCode.UnknownError }
                };
            }

            if (false == planetComponent.IsFreeSlot(slotNumber)) {
                return new Hashtable {
                    { (int)SPC.ReturnCode, (int)RPCErrorCode.SlotNotFree }
                };
            }


            int inventoryItemCount = Player.Inventory.ItemCount(InventoryObjectType.mining_station, inventoryItemID);
            if (inventoryItemCount == 0) {
                return new Hashtable {
                    { (int)SPC.ReturnCode, (int)RPCErrorCode.ObjectNotFound }
                };
            }


            ServerInventoryItem inventoryItem = null;
            if (false == Player.Inventory.TryGetItem(InventoryObjectType.mining_station, inventoryItemID, out inventoryItem)) {
                return new Hashtable {
                    { (int)SPC.ReturnCode, (int)RPCErrorCode.UnknownError }
                };
            }

            MiningStationInventoryObject miningStationInventoryObject = inventoryItem.Object as MiningStationInventoryObject;

            float timeForSingleElement = (60.0f * 60.0f) / miningStationInventoryObject.speed;

            string characterID = Player.GetComponent<PlayerCharacterObject>().characterId;

            var miningData = Player.resource.playerConstructions.miningStation;

            Dictionary<ComponentID, ComponentData> components = new Dictionary<ComponentID, ComponentData> {
                { ComponentID.Model, new ModelComponentData(GetDrillModel((Race)(byte)miningStationInventoryObject.race))},
                { ComponentID.NebulaObject, new NebulaObjectComponentData(ItemType.Bot, new Dictionary<byte, object>(), string.Empty, miningData.size, 0 ) },
                { ComponentID.Raceable, new RaceableComponentData((Race)(byte)miningStationInventoryObject.race)},
                { ComponentID.Damagable, new NotShipDamagableComponentData(miningData.hp, miningData.ignoreDamageAtStart, miningData.ignoreDamageInterval, miningData.createContainer) },
                { ComponentID.Bonuses, new BonusesComponentData() },
                { ComponentID.Bot, new BotComponentData(BotItemSubType.Drill) },
                { ComponentID.MiningStation, new MiningStationComponentData(planetComponent.element,
                    miningStationInventoryObject.capacity,
                    timeForSingleElement,
                    planetObject.Id,
                    Player.nebulaObject.Id,
                    miningStationInventoryObject.capacity * 3,
                    characterID)},
                { ComponentID.Character, new BotCharacterComponentData(CommonUtils.RandomWorkshop((Race)(byte)miningStationInventoryObject.race),
                        30, Turret.SelectFraction(playerRace))},
                { ComponentID.CombatAI, new StayAIComponentData(false, 0, Nebula.Server.AttackMovingType.AttackStay, miningData.useHitProbForAgro) },
                { ComponentID.Movable, new SimpleMovableComponentData(0) },
                { ComponentID.Weapon, new SimpleWeaponComponentData(miningData.optimalDistance, 1, miningData.cooldown, true, miningData.damageInTargetHp) },
                { ComponentID.Target, new TargetComponentData() },

            };

            //log.InfoFormat("creating mining station with max count = {0}, time for single element = {1}, total count = {2}, element = {3} [red]",
            // miningStationInventoryObject.capacity, timeForSingleElement, miningStationInventoryObject.capacity * 3, planetComponent.element);

            NebulaObjectData data = new NebulaObjectData {
                componentCollection = components,
                ID = "DR_" + Guid.NewGuid().ToString(),
                position = Player.transform.position,
                rotation = Player.transform.rotation
            };
            var miningStationObject = ObjectCreate.NebObject(world, data);
            miningStationObject.SetDatabaseSaveable(true);
            miningStationObject.AddToWorld();

            planetComponent.SetStation(miningStationObject.GetComponent<MiningStation>(), slotNumber);

            Player.Inventory.Remove(InventoryObjectType.mining_station, inventoryItemID, 1);
            Player.EventOnInventoryUpdated();
            Player.nebulaObject.mmoWorld().SaveWorldState();

            Player.GetComponent<PlayerCharacterObject>().OnSetMiningStation();

            return new Hashtable {
                { (int)SPC.ReturnCode, (int)RPCErrorCode.Ok}
            };
        }

        public Hashtable CreatePlanetObjectMiningStation(int row, int column, string itemId) {
            Race race = Player.GetComponent<RaceableObject>().getRace();
            MmoWorld world = Player.nebulaObject.mmoWorld();

            var resourceData = (world.Resource() as Res).playerConstructions.planetMiningStationData;

            if (world.Zone.worldType != WorldType.instance) {
                return new Hashtable {
                    { (int)SPC.ReturnCode, (int)RPCErrorCode.InvalidWorldType }
                };
            }
            var commandCenter = world.FindObjectOfType<CommanderCenterPlanetObject>();
            if (!commandCenter) {
                return new Hashtable {
                    { (int)SPC.ReturnCode, (int)RPCErrorCode.BuildCommandCenterFirst }
                };
            } else {
                var raceComponent = commandCenter.GetComponent<RaceableObject>();
                if (raceComponent != null && (raceComponent.getRace() != race)) {
                    return new Hashtable {
                        { (int)SPC.ReturnCode, (int)RPCErrorCode.BuildCommandCenterFirst }
                    };
                }
            }
            if (world.HasObjectAtCell(row, column)) {
                return new Hashtable {
                    { (int)SPC.ReturnCode, (int)RPCErrorCode.CellAlreadyFilled }
                };
            }

            int currentMiningStationsCount = world.Filter(obj => obj.GetComponent<PlanetMiningStationObject>() != null).Count;
            if (currentMiningStationsCount >= world.maxCountPlanetMiningStations) {
                return new Hashtable {
                    { (int)SPC.ReturnCode, (int)RPCErrorCode.BuildAdditionalResourceHangar }
                };
            }

            Dictionary<ComponentID, ComponentData> components = new Dictionary<ComponentID, ComponentData> {
                { ComponentID.Model, new ModelComponentData(GetPlanetMiningStationModel(race)) },
                { ComponentID.NebulaObject, new NebulaObjectComponentData(ItemType.Bot, new Dictionary<byte, object>(), string.Empty, 200, 0) },
                { ComponentID.Raceable, new RaceableComponentData(race) },
                { ComponentID.Damagable, new NotShipDamagableComponentData(resourceData.maxHp, true, 60, false ) },
                { ComponentID.Bonuses, new BonusesComponentData() },
                { ComponentID.Bot, new BotComponentData(BotItemSubType.PlanetBuilding) },
                { ComponentID.PlanetBuilding, new MiningStationPlanetObjectComponentData(row, column, PlanetBasedObjectType.MiningStation, Player.nebulaObject.Id, 
                "CraftOre0001", resourceData.maxSlots, 0, 0, resourceData.life, resourceData.workSpeed) },
                { ComponentID.Character, new BotCharacterComponentData(CommonUtils.RandomWorkshop(race), 30, Turret.SelectFraction(race))},
                { ComponentID.CombatAI, new StayAINonCombatComponentData(false, 0)},
                { ComponentID.Movable, new SimpleMovableComponentData(0) }
            };
            NebulaObjectData data = new NebulaObjectData {
                componentCollection = components,
                ID = "MS_" + Guid.NewGuid().ToString(),
                position = world.GetCellPosition(row, column),
                rotation = Vector3.Zero
            };

            var msObj = ObjectCreate.NebObject(world, data);
            msObj.SetDatabaseSaveable(true);
            msObj.AddToWorld();
            world.SaveWorldState();
            return new Hashtable {
                { (int)SPC.ReturnCode, (int)RPCErrorCode.Ok }
            };
        }

        public Hashtable CreatePlanetObjectResourceAccelerator(int row, int column, string itemId ) {
            Race race = Player.GetComponent<RaceableObject>().getRace();
            MmoWorld world = Player.nebulaObject.mmoWorld();

            var resourceData = (world.Resource() as Res).playerConstructions.planetResourceAcceleratorData;

            if (world.Zone.worldType != WorldType.instance) {
                return new Hashtable {
                    { (int)SPC.ReturnCode, (int)RPCErrorCode.InvalidWorldType }
                };
            }
            var commandCenter = world.FindObjectOfType<CommanderCenterPlanetObject>();
            if (!commandCenter) {
                return new Hashtable {
                    { (int)SPC.ReturnCode, (int)RPCErrorCode.BuildCommandCenterFirst }
                };
            } else {
                var raceComponent = commandCenter.GetComponent<RaceableObject>();
                if (raceComponent != null && (raceComponent.getRace() != race)) {
                    return new Hashtable {
                        { (int)SPC.ReturnCode, (int)RPCErrorCode.BuildCommandCenterFirst }
                    };
                }
            }
            if (world.HasObjectAtCell(row, column)) {
                return new Hashtable {
                    { (int)SPC.ReturnCode, (int)RPCErrorCode.CellAlreadyFilled }
                };
            }

            Dictionary<ComponentID, ComponentData> components = new Dictionary<ComponentID, ComponentData> {
                { ComponentID.Model, new ModelComponentData(GetPlanetResourceAcceleratorModel(race))},
                { ComponentID.NebulaObject, new NebulaObjectComponentData(ItemType.Bot, new Dictionary<byte, object>(), string.Empty, 200, 0 ) },
                { ComponentID.Raceable, new RaceableComponentData(race) },
                { ComponentID.Damagable, new NotShipDamagableComponentData(resourceData.maxHp, true, 60, false) },
                { ComponentID.Bonuses, new BonusesComponentData() },
                { ComponentID.Bot, new BotComponentData(BotItemSubType.PlanetBuilding) },
                { ComponentID.PlanetBuilding, new ResourceAcceleratorPlanetObjectComponentData(row, column, PlanetBasedObjectType.ResourceAccelerator, Player.nebulaObject.Id, resourceData.life, 0)},
                { ComponentID.Character, new BotCharacterComponentData(CommonUtils.RandomWorkshop(race), 30, Turret.SelectFraction(race) ) },
                { ComponentID.CombatAI, new StayAINonCombatComponentData(false, 0) },
                { ComponentID.Movable, new SimpleMovableComponentData(0) }
            };

            NebulaObjectData data = new NebulaObjectData {
                componentCollection = components,
                ID = "RA_" + Guid.NewGuid().ToString(),
                position = world.GetCellPosition(row, column),
                rotation = Vector3.Zero
            };

            var raObject = ObjectCreate.NebObject(world, data);
            raObject.SetDatabaseSaveable(true);
            raObject.AddToWorld();
            world.SaveWorldState();
            return new Hashtable {
                { (int)SPC.ReturnCode, (int)RPCErrorCode.Ok }
            };
        }

        public Hashtable CreatePlanetObjectResourceHangar(int row, int column, string itemId ) {
            Race race = Player.GetComponent<RaceableObject>().getRace();
            MmoWorld world = Player.nebulaObject.mmoWorld();
            var resourceData = (world.Resource() as Res).playerConstructions.planetResourceHangarData;

            if (world.Zone.worldType != WorldType.instance) {
                return new Hashtable {
                    { (int)SPC.ReturnCode, (int)RPCErrorCode.InvalidWorldType }
                };
            }
            var commandCenter = world.FindObjectOfType<CommanderCenterPlanetObject>();
            if (!commandCenter) {
                return new Hashtable {
                    { (int)SPC.ReturnCode, (int)RPCErrorCode.BuildCommandCenterFirst }
                };
            } else {
                var raceComponent = commandCenter.GetComponent<RaceableObject>();
                if (raceComponent != null && (raceComponent.getRace() != race)) {
                    return new Hashtable {
                        { (int)SPC.ReturnCode, (int)RPCErrorCode.BuildCommandCenterFirst }
                    };
                }
            }
            if (world.HasObjectAtCell(row, column)) {
                return new Hashtable {
                    { (int)SPC.ReturnCode, (int)RPCErrorCode.CellAlreadyFilled }
                };
            }

            Dictionary<ComponentID, ComponentData> components = new Dictionary<ComponentID, ComponentData> {
                { ComponentID.Model, new ModelComponentData(GetPlanetResourceHangarModel(race))},
                { ComponentID.NebulaObject, new NebulaObjectComponentData(ItemType.Bot, new Dictionary<byte, object>(), string.Empty, 200, 0 ) },
                { ComponentID.Raceable, new RaceableComponentData(race) },
                { ComponentID.Damagable, new NotShipDamagableComponentData(resourceData.maxHp, true, 60, false) },
                { ComponentID.Bonuses, new BonusesComponentData() },
                { ComponentID.Bot, new BotComponentData(BotItemSubType.PlanetBuilding) },
                { ComponentID.PlanetBuilding, new ResourceHangarPlanetObjectComponentData(row, column, PlanetBasedObjectType.ResourceHangar, Player.nebulaObject.Id, resourceData.life, 0) },
                { ComponentID.Character, new BotCharacterComponentData(CommonUtils.RandomWorkshop(race), 30, Turret.SelectFraction(race) ) },
                { ComponentID.CombatAI, new StayAINonCombatComponentData(false, 0) },
                { ComponentID.Movable, new SimpleMovableComponentData(0) }
            };

            NebulaObjectData data = new NebulaObjectData {
                componentCollection = components,
                ID = "RH_" + Guid.NewGuid().ToString(),
                position = world.GetCellPosition(row, column ),
                rotation = Vector3.Zero
            };

            var resourceHangarObject = ObjectCreate.NebObject(world, data);
            resourceHangarObject.SetDatabaseSaveable(true);
            resourceHangarObject.AddToWorld();
            world.SaveWorldState();
            return new Hashtable {
                { (int)SPC.ReturnCode, (int)RPCErrorCode.Ok }
            };
        }

        public Hashtable CreatePlanetObjectTurret(int row, int column, string itemId ) {
            Race race = Player.GetComponent<RaceableObject>().getRace();

            MmoWorld world = Player.nebulaObject.mmoWorld();
            var resourceData = (world.Resource() as Res).playerConstructions.planetTurretData;

            if (world.Zone.worldType != WorldType.instance) {
                return new Hashtable {
                    { (int)SPC.ReturnCode, (int)RPCErrorCode.InvalidWorldType }
                };
            }
            var commandCenter = world.FindObjectOfType<CommanderCenterPlanetObject>();
            if(!commandCenter ) {
                return new Hashtable {
                    { (int)SPC.ReturnCode, (int)RPCErrorCode.BuildCommandCenterFirst }
                };
            } else {
                var raceComponent = commandCenter.GetComponent<RaceableObject>();
                if(raceComponent != null && (raceComponent.getRace() != race )) {
                    return new Hashtable {
                        { (int)SPC.ReturnCode, (int)RPCErrorCode.BuildCommandCenterFirst }
                    };
                }
            }
            if (world.HasObjectAtCell(row, column)) {
                return new Hashtable {
                    { (int)SPC.ReturnCode, (int)RPCErrorCode.CellAlreadyFilled }
                };
            }

            Dictionary<ComponentID, ComponentData> components = new Dictionary<ComponentID, ComponentData> {
                { ComponentID.Model,                new ModelComponentData(GetPlanetTurretModel(race)) },
                { ComponentID.NebulaObject,         new NebulaObjectComponentData(ItemType.Bot, new Dictionary<byte, object>(), string.Empty, 50, 0 ) },
                { ComponentID.Raceable,             new RaceableComponentData(race) },
                { ComponentID.Damagable,            new NotShipDamagableComponentData(resourceData.maxHp, true, 60, false ) },
                { ComponentID.Bonuses,              new BonusesComponentData() },
                { ComponentID.Bot,                  new BotComponentData(BotItemSubType.PlanetBuilding) },
                { ComponentID.PlanetBuilding,       new TurretPlanetObjectComponentData(row, column, PlanetBasedObjectType.Turret, Player.nebulaObject.Id, resourceData.life, 0) },
                { ComponentID.Character,            new BotCharacterComponentData(CommonUtils.RandomWorkshop(race), 30, Turret.SelectFraction(race) ) },
                { ComponentID.CombatAI,             new StayAIComponentData(false, 0, Nebula.Server.AttackMovingType.AttackStay, false) },
                { ComponentID.Movable,              new SimpleMovableComponentData(0) },
                { ComponentID.Weapon,               new SimpleWeaponComponentData(resourceData.od, resourceData.damage, 2, false, 0) },
                { ComponentID.Target,               new TargetComponentData() }
            };

            NebulaObjectData data = new NebulaObjectData {
                componentCollection = components,
                ID = "PT_" + Guid.NewGuid().ToString(),
                position = world.GetCellPosition(row, column),
                rotation = Vector3.Zero
            };

            var turretPlanetObject = ObjectCreate.NebObject(world, data);
            turretPlanetObject.SetDatabaseSaveable(true);
            turretPlanetObject.AddToWorld();
            world.SaveWorldState();
            return new Hashtable {
                { (int)SPC.ReturnCode, (int)RPCErrorCode.Ok }
            };
        }

        public Hashtable CreatePlanetObjectCommandCenter(int row, int column, string itemId ) {
            MmoWorld world = Player.nebulaObject.mmoWorld();
            var resourceData = (world.Resource() as Res).playerConstructions.planetCommandCenterData;

            if(world.Zone.worldType != WorldType.instance ) {
                return new Hashtable {
                    { (int)SPC.ReturnCode, (int)RPCErrorCode.InvalidWorldType }
                };
            }
            if(world.FindObjectOfType<CommanderCenterPlanetObject>()) {
                return new Hashtable {
                    { (int)SPC.ReturnCode, (int)RPCErrorCode.MultipleObjectRestricted }
                };
            }
            if(world.HasObjectAtCell(row, column)) {
                return new Hashtable {
                    { (int)SPC.ReturnCode, (int)RPCErrorCode.CellAlreadyFilled }
                };
            }

            Race race = Player.GetComponent<RaceableObject>().getRace();

            //delete inventory item will be leter
            Dictionary<ComponentID, ComponentData> components = new Dictionary<ComponentID, ComponentData> {
                { ComponentID.Model,            new ModelComponentData(GetCommandCenterModel(race)) },
                { ComponentID.NebulaObject,     new NebulaObjectComponentData(ItemType.Bot, new Dictionary<byte, object>(), string.Empty, 300, 0) },
                { ComponentID.Raceable,         new RaceableComponentData(race) },
                { ComponentID.Damagable,        new NotShipDamagableComponentData(resourceData.maxHp, false, 0, false) },
                { ComponentID.Bonuses,          new BonusesComponentData() },
                { ComponentID.Bot,              new BotComponentData(BotItemSubType.PlanetBuilding) },
                { ComponentID.PlanetBuilding,   new CommandCenterPlanetObjectComponentData(row, column, PlanetBasedObjectType.CommanderCenter, Player.nebulaObject.Id, resourceData.life, 0)  },
                { ComponentID.Character,        new BotCharacterComponentData(CommonUtils.RandomWorkshop(race), 30, Turret.SelectFraction(race)) },
                { ComponentID.CombatAI,         new StayAINonCombatComponentData(false, 0f) },
                { ComponentID.Movable,          new SimpleMovableComponentData(0) }
            };

            NebulaObjectData data = new NebulaObjectData {
                componentCollection = components,
                ID = "DR_" + Guid.NewGuid().ToString(),
                position = world.GetCellPosition(row, column),
                rotation = Vector3.Zero
            };

            var commandCenterObject = ObjectCreate.NebObject(world, data);
            commandCenterObject.SetDatabaseSaveable(true);
            commandCenterObject.AddToWorld();
            world.SaveWorldState();
            return new Hashtable {
                { (int)SPC.ReturnCode, (int)RPCErrorCode.Ok }
            };
        }

        public Hashtable MakeTurretFromItem(string itemID) {
            var world = Player.nebulaObject.mmoWorld();
            Race race = (Race)Player.nebulaObject.Raceable().race;
            int itemCount = Player.Inventory.ItemCount(InventoryObjectType.turret, itemID);
            if(itemCount == 0) {
                return new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.ObjectNotFound } };
            }
            ServerInventoryItem invItem;
            if(!Player.Inventory.TryGetItem(InventoryObjectType.turret, itemID, out invItem)) {
                return new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.UnknownError } };
            }
            TurretInventoryObject turretInventoryObject = invItem.Object as TurretInventoryObject;
            if(turretInventoryObject.race != (int)race) {
                return new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.InvalidRace } };
            }

            if(turretInventoryObject.race != (int)world.ownedRace) {
                return new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.InvalidWorldRace } };
            }

            int currentTurretsCount = world.GetItems((item) => item.GetComponent<Turret>()).Count;
            if(currentTurretsCount >= MaxCountTurretsAlways) {
                return new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.TurretCountLimitReached } };
            }

            if(currentTurretsCount >= MaxCountTurretsInWorld(world)) {
                return new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.NeedMoreFortifications } };
            }

            float separateDistance = 300;
            Vector3 myPosition = Player.transform.position;
            float hDist = Vector3.Distance(myPosition, world.Zone.humanSP);
            float bDist = Vector3.Distance(myPosition, world.Zone.borguzandSP);
            float cDist = Vector3.Distance(myPosition, world.Zone.criptizidSP);

            if (hDist < separateDistance || bDist < separateDistance || cDist < separateDistance) {
                return new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.VeryCloseToSpawnPoint } };
            }

            var turretData = Player.resource.playerConstructions.turret;
            Dictionary<ComponentID, ComponentData> components = new Dictionary<ComponentID, ComponentData> {
                { ComponentID.Damagable, new OutpostDamagableComponentData(turretData.hp, turretData.ignoreDamageAtStart, turretData.ignoreDamageInterval, turretData.createContainer, turretData.fixedInputDamage, turretData.additionalHp) },
                { ComponentID.Model, new ModelComponentData(GetTurretModel(race))},
                { ComponentID.CombatAI, new FreeFlyNearPointComponentData(true, 0.5f, turretData.wanderRadius, Nebula.Server.AttackMovingType.AttackStay, turretData.useHitProbForAgro) },
                { ComponentID.Movable, new SimpleMovableComponentData(10) },
                { ComponentID.Weapon, new SimpleWeaponComponentData(turretData.optimalDistance, 1, turretData.cooldown, true, turretData.damageInTargetHpPc) },
                { ComponentID.Target, new TargetComponentData() },
                { ComponentID.Character, new BotCharacterComponentData(CommonUtils.RandomWorkshop(race), 1, Turret.SelectFraction(race)) },
                { ComponentID.Raceable, new RaceableComponentData(race)  },
                { ComponentID.Bonuses, new BonusesComponentData() },
                { ComponentID.Bot, new BotComponentData(BotItemSubType.Turret) },
                { ComponentID.Turret, new TurretComponentData() },
                { ComponentID.NebulaObject, new NebulaObjectComponentData(ItemType.Bot, new Dictionary<byte, object>(), "",  turretData.size, 0 ) }
            };
            NebulaObjectData data = new NebulaObjectData {
                ID = "TUR_" + Guid.NewGuid().ToString(),
                position = myPosition,
                rotation = Vector3.Zero,
                componentCollection = components
            };
            var turretObj = ObjectCreate.NebObject(world, data);
            turretObj.SetDatabaseSaveable(true);
            turretObj.AddToWorld();

            Player.Inventory.Remove(InventoryObjectType.turret, itemID, 1);
            Player.EventOnInventoryUpdated();
            Player.nebulaObject.mmoWorld().SaveWorldState();

            var achievments = Player.GetComponent<AchievmentComponent>();
            achievments.OnTurretCreated();

            return new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.Ok } };
        }

        public Hashtable MakeFortificationFromItem(string itemID) {
            var world = Player.nebulaObject.mmoWorld();
            Race race = (Race)Player.nebulaObject.Raceable().race;
            int itemCount = Player.Inventory.ItemCount(InventoryObjectType.fortification, itemID);
            if(itemCount == 0 ) {
                return new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.ObjectNotFound } };
            }
            ServerInventoryItem invItem;
            if(!Player.Inventory.TryGetItem(InventoryObjectType.fortification, itemID, out invItem)) {
                return new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.UnknownError } };
            }

            FortificationInventoryObject fortificationInventoryObject = invItem.Object as FortificationInventoryObject;
            if(fortificationInventoryObject.race != (int)race) {
                return new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.InvalidRace } };
            }

            if(world.ownedRace != race ) {
                return new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.InvalidWorldRace } };
            }

            var outposts = world.GetItems((item) => item.GetComponent<MainOutpost>());
            if(outposts.Count == 0) {
                return new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.NeedConstructOutpostBefore } };
            }


            var fortifications = world.GetItems((item) => item.GetComponent<Outpost>());
            if(fortifications.Count >= 3) {
                return new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.MaxCountOfFortificationsReached } };
            }

            float separateDistance = 300;
            Vector3 myPosition = Player.transform.position;
            foreach(var pOutpost in outposts) {
                if(Vector3.Distance(myPosition, pOutpost.Value.transform.position) < separateDistance ) {
                    return new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.VeryCloseToOtherSystemConstructions } };
                }
            }

            foreach(var pFortification in fortifications) {
                if(Vector3.Distance(myPosition, pFortification.Value.transform.position) < separateDistance) {
                    return new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.VeryCloseToOtherSystemConstructions } };
                }
            }

            float hDist = Vector3.Distance(myPosition, world.Zone.humanSP);
            float bDist = Vector3.Distance(myPosition, world.Zone.borguzandSP);
            float cDist = Vector3.Distance(myPosition, world.Zone.criptizidSP);

            if (hDist < separateDistance || bDist < separateDistance || cDist < separateDistance) {
                return new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.VeryCloseToSpawnPoint } };
            }

            var fortData = Player.resource.playerConstructions.fortification;
            Dictionary<ComponentID, ComponentData> components = new Dictionary<ComponentID, ComponentData> {
                { ComponentID.Damagable, new OutpostDamagableComponentData(fortData.hp, fortData.ignoreDamageAtStart, fortData.ignoreDamageInterval, fortData.createContainer, fortData.fixedInputDamage, fortData.additionalHp) },
                { ComponentID.Model, new ModelComponentData(GetFortificationModel(race)) },
                { ComponentID.Bot, new BotComponentData(BotItemSubType.Outpost) },
                { ComponentID.Bonuses, new BonusesComponentData() },
                { ComponentID.Character, new BotCharacterComponentData(CommonUtils.RandomWorkshop(race), 1, Turret.SelectFraction(race)) },
                { ComponentID.Outpost, new OutpostComponentData() },
                { ComponentID.Raceable, new RaceableComponentData(race) },
                { ComponentID.NebulaObject, new NebulaObjectComponentData (ItemType.Bot, new Dictionary<byte, object>(), "",  fortData.size, 0) },
                { ComponentID.CombatAI, new StayAIComponentData(false, 0.5f, Nebula.Server.AttackMovingType.AttackStay, true) },
                { ComponentID.Movable, new SimpleMovableComponentData(0) },
                { ComponentID.Weapon, new SimpleWeaponComponentData(fortData.optimalDistance, 1, fortData.cooldown, true, fortData.damageInTargetHpPc) },
                { ComponentID.Target, new TargetComponentData() }
            };
            NebulaObjectData data = new NebulaObjectData {
                ID = "FORT_" + Guid.NewGuid().ToString(),
                position = myPosition,
                rotation = Vector3.Zero,
                componentCollection = components
            };
            var fortificationObj = ObjectCreate.NebObject(world, data);
            fortificationObj.SetDatabaseSaveable(true);
            fortificationObj.AddToWorld();

            Player.Inventory.Remove(InventoryObjectType.fortification, itemID, 1);
            Player.EventOnInventoryUpdated();
            Player.nebulaObject.mmoWorld().SaveWorldState();

            var achievments = Player.GetComponent<AchievmentComponent>();
            achievments.OnFortificationCreated();

            Player.GetComponent<PlayerCharacterObject>().OnSetFortification();

            return new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.Ok } };
        }

        public Hashtable MakeOutpostFromItem(string itemID) {
            var world = Player.nebulaObject.mmoWorld();
            Race race = (Race)Player.nebulaObject.Raceable().race;

            int itemCount = Player.Inventory.ItemCount(InventoryObjectType.outpost, itemID);
            if(itemCount == 0 ) {
                return new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.ObjectNotFound } };
            }
            ServerInventoryItem invItem;
            if(!Player.Inventory.TryGetItem(InventoryObjectType.outpost, itemID, out invItem)) {
                return new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.UnknownError } };
            }

            OutpostInventoryObject outpostInventoryObject = invItem.Object as OutpostInventoryObject;
            if(outpostInventoryObject.race != (int)race) {
                return new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.InvalidRace } };
            }


            int numOutpostInWorld = world.GetItems((item) => item.GetComponent<MainOutpost>()).Count;
            if(numOutpostInWorld > 0 ) {
                return new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.AlreadyExists } };
            }

            float separateDistance = 300;
            Vector3 myPosition = Player.transform.position;
            float hDist = Vector3.Distance(myPosition, world.Zone.humanSP);
            float bDist = Vector3.Distance(myPosition, world.Zone.borguzandSP);
            float cDist = Vector3.Distance(myPosition, world.Zone.criptizidSP);

            if(hDist < separateDistance || bDist < separateDistance || cDist < separateDistance) {
                return new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.VeryCloseToSpawnPoint } };
            }

            var outData = Player.resource.playerConstructions.outpost;
            Dictionary<ComponentID, ComponentData> components = new Dictionary<ComponentID, ComponentData> {
                { ComponentID.Damagable, new OutpostDamagableComponentData(outData.hp, outData.ignoreDamageAtStart, outData.ignoreDamageInterval, false, outData.fixedInputDamage, outData.additionalHp) },
                { ComponentID.Model, new ModelComponentData(GetOutpostModel()) },
                { ComponentID.MainOutpost, new MainOutpostComponentData() },
                { ComponentID.Bot, new BotComponentData(BotItemSubType.MainOutpost) },
                { ComponentID.Bonuses, new BonusesComponentData() },
                { ComponentID.Raceable, new RaceableComponentData(race) },
                { ComponentID.Character, new BotCharacterComponentData(CommonUtils.RandomWorkshop(race), 1, Turret.SelectFraction(race)) },
                { ComponentID.NebulaObject, new NebulaObjectComponentData(ItemType.Bot, new Dictionary<byte, object>(), "", outData.size, 0) }
            };

            NebulaObjectData data = new NebulaObjectData {
                ID = "OUT_" + Guid.NewGuid().ToString(),
                position = myPosition,
                rotation = Vector3.Zero,
                componentCollection = components
            };

            var outpostObj = ObjectCreate.NebObject(world, data);
            outpostObj.SetDatabaseSaveable(true);
            outpostObj.AddToWorld();

            Player.Inventory.Remove(InventoryObjectType.outpost, itemID, 1);
            Player.EventOnInventoryUpdated();
            Player.nebulaObject.mmoWorld().SaveWorldState();

            var achievments = Player.GetComponent<AchievmentComponent>();
            achievments.OnOutpostCreated();

            Player.GetComponent<PlayerCharacterObject>().OnSetOutpost();

            return new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.Ok } };
        }

        public Hashtable CreateFounderCube(string inventoryItemId) {

            var inventory = Player.Inventory;
            if(false == inventory.HasItem(InventoryObjectType.founder_cube, inventoryItemId)) {
                return CreateResponse(RPCErrorCode.ItemNotFound);
            }

            ServerInventoryItem fitem;
            if(false == inventory.TryGetItem(InventoryObjectType.founder_cube, inventoryItemId, out fitem)) {
                return CreateResponse(RPCErrorCode.ItemNotFound);
            }

            FounderCubeInventoryObject founderCubeInventoryObject = fitem.Object as FounderCubeInventoryObject;

            if(false == founderCubeInventoryObject.ReadyToUse() ) {
                return CreateResponse(RPCErrorCode.NotReady);
            }

            var beacons = Player.nebulaObject.mmoWorld().GetItems(item => {
                if (item.GetComponent<Teleport>()) {
                    if (item.transform.DistanceTo(Player.transform) < 300) {
                        return true;
                    }
                }
                return false;
            });

            if(beacons.Count > 0 ) {
                return CreateResponse(RPCErrorCode.VeryCloseToTeleports);
            }

            var playerCharacter = Player.GetComponent<PlayerCharacterObject>();

            string characterId = playerCharacter.characterId;
            string characterName = playerCharacter.characterName;
            string guildId = playerCharacter.guildId;
            string guildName = playerCharacter.guildName;
            string gameRef = Player.nebulaObject.Id;

            Dictionary<ComponentID, ComponentData> components = new Dictionary<ComponentID, ComponentData> {
                { ComponentID.Model ,           new ModelComponentData("founder_cube") },
                { ComponentID.Bot,              new BotComponentData(BotItemSubType.FounderCube) },
                { ComponentID.NebulaObject,     new NebulaObjectComponentData(ItemType.Bot, new Dictionary<byte, object>(), string.Empty, 1, 0) },
                { ComponentID.FounderCube,      new FounderCubeComponentData(characterId, characterName, guildId, guildName, gameRef, 0) },
                { ComponentID.Raceable,         new RaceableComponentData((Race)Player.GetComponent<RaceableObject>().race) }
            };

            NebulaObjectData data = new NebulaObjectData {
                ID = "FC" + Guid.NewGuid().ToString(),
                position = Player.transform.position,
                rotation = Player.transform.rotation,
                componentCollection = components
            };

            var founderCube = ObjectCreate.NebObject(Player.nebulaObject.mmoWorld(), data);
            founderCube.SetDatabaseSaveable(true);
            founderCube.AddToWorld();
            founderCubeInventoryObject.SetUseTimeNow();
            Player.EventOnInventoryUpdated();
            Player.nebulaObject.mmoWorld().SaveWorldState();

            return CreateResponse(RPCErrorCode.Ok);
        }



        private Hashtable CreateResponse(RPCErrorCode code) {
            return new Hashtable {
                {(int)SPC.ReturnCode, (int)code}
            };
        }

        private string GetPlanetMiningStationModel(Race race) {
            switch(race) {
                case Race.Humans:
                    return "h_p_mining_station";
                case Race.Borguzands:
                    return "b_p_mining_station";
                case Race.Criptizoids:
                    return "c_p_mining_station";
                default:
                    return string.Empty;
            }
        }

        private string GetPlanetResourceAcceleratorModel(Race race) {
            switch(race) {
                case Race.Humans:
                    return "h_resource_accelerator";
                case Race.Borguzands:
                    return "b_resource_accelerator";
                case Race.Criptizoids:
                    return "c_resource_accelerator";
                default:
                    return string.Empty;
            }
        }

        private string GetPlanetResourceHangarModel(Race race) {
            switch(race) {
                case Race.Humans:
                    return "h_resource_hangar";
                case Race.Borguzands:
                    return "b_resource_hangar";
                case Race.Criptizoids:
                    return "c_resource_hangar";
                default:
                    return string.Empty;
            }
        }
        private string GetPlanetTurretModel(Race race) {
            switch(race) {
                case Race.Humans:
                    return "h_planet_turret";
                case Race.Borguzands:
                    return "b_planet_turret";
                case Race.Criptizoids:
                    return "c_planet_turret";
                default:
                    return string.Empty;
            }
        }
        private string GetCommandCenterModel(Race race) {
            switch(race) {
                case Race.Humans:
                    return "h_command_center";
                case Race.Borguzands:
                    return "b_command_center";
                case Race.Criptizoids:
                    return "c_command_center";
                default:
                    return string.Empty;
            }
        }

        private string GetDrillModel(Race race) {
            switch(race) {
                case Race.Humans:
                    return "h_drill";
                case Race.Borguzands:
                    return "b_drill";
                case Race.Criptizoids:
                    return "c_drill";
                default:
                    return string.Empty;
            }
        }

        private string GetOutpostModel() {
            var race = (Race)Player.nebulaObject.Raceable().race;
            switch(race) {
                case Race.Humans:
                    return "HUMAN_OUTPOST";
                case Race.Criptizoids:
                    return "CRIPTIZID_OUTPOST";
                case Race.Borguzands:
                    return "BORGUZAND_OUTPOST";
                default:
                    return string.Empty;
            }
        }

        private string GetFortificationModel(Race race) {
            switch(race) {
                case Race.Humans:
                    return "HUMAN_FORTIFICATION";
                case Race.Borguzands:
                    return "BORGUZAND_FORTIFICATION";
                case Race.Criptizoids:
                    return "CRIPTIZID_FORTIFICATION";
                default:
                    return string.Empty;
            }
        }

        private string GetTurretModel(Race race) {
            switch(race) {
                case Race.Humans:
                    return "HUMAN_TURRET";
                case Race.Borguzands:
                    return "BORGUZAND_TURRET";
                case Race.Criptizoids:
                    return "CRIPTIZID_TURRET";
                default:
                    return string.Empty;
            }
        }

        public Hashtable GetPlanetInfo(string planetID) {
            var planet = Player.nebulaObject.mmoWorld().GetItem((item) => {
                if (item.Id == planetID && item.Type == (byte)ItemType.Bot) {
                    return true;
                }
                return false;
            });
            if(planet != null ) {
                var planetComponent = planet.GetComponent<PlanetObject>();
                if(planetComponent ) {
                    Player.GetComponent<MmoMessageComponent>().ReceivePlanetInfo(planetComponent.GetInfo());
                    return new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.Ok } };
                }
            }
            return new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.UnknownError } };
        }

        public Hashtable AddTimedEffect() {
            ExpTimedEffect timedEffect = new ExpTimedEffect(1, (int)(CommonUtils.SecondsFrom1970() + 600), 1);
            Player.GetComponent<PlayerTimedEffects>().AddTimedEffect(timedEffect);
            return new Hashtable {
                { (int)SPC.ReturnCode, (int)RPCErrorCode.Ok }
            };
        }

        /// <summary>
        /// Test RPC method, add some boost object to player inventory
        /// </summary>
        /// <returns></returns>
        public Hashtable AddExpBoost() {
            ExpBoostObject expObject = new ExpBoostObject("ginp0011", 1, 1200, 1);
            if(Player.Inventory.HasSlotsForItems(new List<string> { expObject.Id })) {
                Player.Inventory.Add(expObject, 1);
                Player.EventOnInventoryUpdated();

                return new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.Ok } };
            }
            return new Hashtable {
                { (int)SPC.ReturnCode, (int)RPCErrorCode.UnknownError }
            };
        }

        private ServerInventory GetInventory(byte inventoryType) {
            ServerInventory serverInventory = null;
            switch ((InventoryType)inventoryType) {
                case InventoryType.ship: {
                        serverInventory = Player.Inventory;
                    }
                    break;
                case InventoryType.station: {
                        serverInventory = Player.Station.StationInventory;
                    }
                    break;
            }
            return serverInventory;
        }

        public Hashtable UseLootBoxObject(byte inventoryType, string itemId ) {
            ServerInventory serverInventory = GetInventory(inventoryType);
            if(serverInventory == null ) {
                return new Hashtable {
                    { (int)SPC.ReturnCode, (int)RPCErrorCode.InvalidInventoryType }
                };
            }
            ServerInventoryItem item;
            if (!serverInventory.TryGetItem(InventoryObjectType.loot_box, itemId, out item)) {
                return new Hashtable {
                    { (int)SPC.ReturnCode, (int)RPCErrorCode.ItemNotFound }
                };
            }
            LootBoxObject lootBoxObject = item.Object as LootBoxObject;
            if(lootBoxObject == null ) {
                return new Hashtable {
                    { (int)SPC.ReturnCode, (int)RPCErrorCode.ObjectNotFound }
                };
            }

            int level = Player.nebulaObject.Character().level;
            Workshop workshop = (Workshop)Player.nebulaObject.Character().workshop;

            Nebula.Drop.DropList dropList = Player.resource.dropLists.GetList(lootBoxObject.dropList);

            if(dropList == null ) {
                return new Hashtable {
                    { (int)SPC.ReturnCode, (int)RPCErrorCode.DropListNotFound}
                };
            }

            List<ServerInventoryItem> resultItems = dropList.Roll(Player.resource, level, workshop);
            List<string> ids = resultItems.Select(it => it.Object.Id).ToList();

            if (false == serverInventory.HasSlotsForItems( ids )) {
                return new Hashtable {
                    { (int)SPC.ReturnCode, (int)RPCErrorCode.NeedFreeSlots },
                    { (int)SPC.Count, serverInventory.NumSlotsForItems(ids) }
                };
            }

            serverInventory.Remove(InventoryObjectType.loot_box, itemId, 1);

            foreach(ServerInventoryItem newItem in resultItems ) {
                serverInventory.Add(newItem.Object, newItem.Count);
            }

            object[] newItems = new object[resultItems.Count];
            for(int i = 0; i < resultItems.Count; i++ ) {
                newItems[i] = resultItems[i].GetInfo();
            }

            InventoryType invType = (InventoryType)inventoryType;

            if (invType == InventoryType.ship) {
                Player.EventOnInventoryUpdated();
            } else if (invType == InventoryType.station) {
                Player.EventOnStationHoldUpdated();
            }

            return new Hashtable {
                {(int)SPC.ReturnCode, RPCErrorCode.Ok },
                {(int)SPC.Items, newItems }
            };
        }

        public Hashtable UseExpBoostObject(byte inventoryType, string itemId  ) {

            ServerInventory serverInventory = GetInventory(inventoryType);

            if(serverInventory == null ) {
                return new Hashtable {
                    { (int)SPC.ReturnCode, (int)RPCErrorCode.InvalidInventoryType }
                };
            }

            ServerInventoryItem item;
            if( !serverInventory.TryGetItem(InventoryObjectType.exp_boost, itemId, out item) ) {
                return new Hashtable {
                    { (int)SPC.ReturnCode, (int)RPCErrorCode.ItemNotFound }
                };
            }

            ExpBoostObject expObject = item.Object as ExpBoostObject;

            if(expObject == null ) {
                return new Hashtable {
                    { (int)SPC.ReturnCode, (int)RPCErrorCode.ObjectNotFound }
                };
            }

            ExpTimedEffect expEffect = new ExpTimedEffect(expObject.value, CommonUtils.SecondsFrom1970() + expObject.interval, expObject.tag);
            Player.GetComponent<PlayerTimedEffects>().AddTimedEffect(expEffect);
            serverInventory.Remove(InventoryObjectType.exp_boost, itemId, 1);

            InventoryType invType = (InventoryType)inventoryType;

            if (invType == InventoryType.ship) {
                Player.EventOnInventoryUpdated();
            } else if(invType == InventoryType.station ) {
                Player.EventOnStationHoldUpdated();
            }

            Player.GetComponent<PlayerLoaderObject>().SaveTimedEffects();

            return new Hashtable {
                {(int)SPC.ReturnCode, (int)RPCErrorCode.Ok },
                {(int)SPC.Interval, expObject.interval },
                {(int)SPC.Value, expObject.value },
                {(int)SPC.Tag, expObject.tag }
            };
        } 

        public Hashtable ReceiveDamage(float dmg) {
            WeaponDamage weaponDamage = new WeaponDamage(WeaponBaseType.Rocket);
            weaponDamage.SetBaseTypeDamage(dmg);

            float actual = Player.nebulaObject.Damagable().ReceiveDamage(new InputDamage(null, weaponDamage)).totalDamage;
            return new Hashtable {
                {(int)SPC.ReturnCode, (int)RPCErrorCode.Ok },
                {(int)SPC.Damage, dmg },
                {(int)SPC.ActualDamage, actual}
            };
        }

        public Hashtable AddRandomCraftElement() {
            var data = Player.resource.craftObjects.random;
            CraftResourceObject obj = new CraftResourceObject(data.id, data.color);
            bool status = false;
            if(Player.Inventory.HasSlotsForItems(new List<string> { obj.Id })) {
                if(Player.Inventory.Add(obj, 1)) {
                    status = true;
                }
            }
            if(status) {
                Player.EventOnInventoryUpdated();
            }
            return new Hashtable {
                { (int)SPC.ReturnCode, (int)RPCErrorCode.Ok },
                { (int)SPC.Status, status }
            };
        }

        //====================================Pet Operations=====================================
        public Hashtable GetPets() {
            return m_PetOps.GetPets();
        }

        public Hashtable AddRandomPet() {
            return m_PetOps.AddRandomPet();
        }

        /*
        public Hashtable ActivatePet(string id) {
            return m_PetOps.ActivatePet(id);
        }

        public Hashtable DeactivatePet(string id) {
            return m_PetOps.DeactivatePet(id);
        }*/

        public Hashtable ActivatePet(string deactivatePetId, string activatePetId ) {
            return m_PetOps.ActivatePet(deactivatePetId, activatePetId);
        }

        public Hashtable AddOrReplaceActiveSkill(string petId, int oldSkill, int newSkill) {
            return m_PetOps.AddOrReplaceActiveSkill(petId, oldSkill, newSkill);
        }

        public Hashtable SetCurrentEnergy(float en) {
            Player.GetComponent<ShipEnergyBlock>().SetCurrentEnergy(en);
            return new Hashtable {
                { (int)SPC.ReturnCode, (int)RPCErrorCode.Ok },
                { (int)SPC.Energy, Player.GetComponent<ShipEnergyBlock>().currentEnergy }
            };
        }

        public Hashtable SetPassiveSkill(string petId, int skillId ) {
            return m_PetOps.SetPassiveSkill(petId, skillId);
        }

        public Hashtable AddPetScheme() {
            return m_PetOps.AddPetScheme();
        }

        public Hashtable AddPetSkin() {
            return m_PetOps.AddPetSkin();
        }

        public Hashtable TransformPetSchemeToPet(string schemeId) {
            return m_PetOps.TransformPetSchemeToPet(schemeId);
        }

        public Hashtable DestroyPet(string petId ) {
            return m_PetOps.DestroyPet(petId);
        }


        public Hashtable ImprovePetColor(string petId) {
            return m_PetOps.ImprovePetColor(petId);
        }

        public Hashtable AddAllCraftResources() {
            return m_PetOps.AddAllCraftResources();
        }

        public Hashtable ImprovePetMastery(string petId) {
            return m_PetOps.ImprovePetMastery(petId);
        }

        public Hashtable ChangePetSkin(string skinItemId, string petId) {
            return m_PetOps.ChangePetSkin(skinItemId, petId);
        }

        public Hashtable ActivatePetSkill(string petId, int skill, bool activate) {
            return m_PetOps.ActivatePetSkill(petId, skill, activate);
        }

        public Hashtable GetPetAtWorld(string itemId ) {
            return m_PetOps.GetPetAtWorld(itemId);
        }

        private Hashtable GetDump(NebulaObject obj) {
            Hashtable hash = new Hashtable();
            foreach (int cid in obj.componentIds) {
                var comp = obj.GetComponent(cid);
                if (comp != null) {
                    hash.Add(((ComponentID)cid).ToString(), comp.DumpHash());
                }
            }
            return hash;
        }

        public Hashtable DumpTarget() {
            var targetComponent = Player.nebulaObject.Target();

            Hashtable hash = null;
            if (targetComponent.targetObject) {
                hash = GetDump(targetComponent.targetObject);
            } else {
                hash = GetDump(Player.nebulaObject);
            }
            string str = hash.ToStringBuilder().ToString();
            log.Info(str);
            return hash;
        }

        //=================================Contract Operations===================
        //public Hashtable AcceptTestContract() {
        //    return m_ContractOps.AcceptTestContract();
        //}

        public Hashtable GetContracts() {
            return m_ContractOps.GetContracts();
        }

        public Hashtable CompleteContract(string id) {
            return m_ContractOps.CompleteContract(id);
        }

        //public Hashtable AcceptContract(int icategory) {
        //    return m_ContractOps.AcceptContract(icategory);
        //}

        public Hashtable ProposeContract(int category) {
            return m_ContractOps.ProposeContract(category);
        }

        public Hashtable AcceptContract(string contractId ) {
            return m_ContractOps.AcceptContract(contractId);
        }

        public Hashtable DeclineContract(string contractId ) {
            return m_ContractOps.DeclineContract(contractId);
        }

        public int RestartLoop() {
            Player.application.updater.Restart();
            return (int)RPCErrorCode.Ok;
        }

        public int TestAddContractItems() {
            return m_ContractOps.TestAddContractItems();
        }

        public int TestRemoveContractItems() {
            return m_ContractOps.TestRemoveContractItems();
        }

        public Hashtable GetAchievmentInfo() {
            return Player.GetComponent<AchievmentComponent>().GetInfo();
        }
    }
}
