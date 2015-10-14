using Common;
using ExitGames.Logging;
using Nebula.Engine;
using Nebula.Game.Components;
using Space.Database;
using Space.Game;
using Space.Game.Drop;
using Space.Game.Ship;
using System.Collections;
using System.Collections.Generic;
using System;
using GameMath;
using Space.Game.Resources;
using Space.Game.Inventory;
using ServerClientCommon;
using Nebula.Database;

namespace Nebula.Game.Components {

    [REQUIRE_COMPONENT(typeof(ShipWeapon))]
    [REQUIRE_COMPONENT(typeof(PlayerCharacterObject))]
    [REQUIRE_COMPONENT(typeof(MmoActor))]
    [REQUIRE_COMPONENT(typeof(PlayerSkills))]
    [REQUIRE_COMPONENT(typeof(AIState))]
    [REQUIRE_COMPONENT(typeof(PlayerBonuses))]
    [REQUIRE_COMPONENT(typeof(PlayerShipMovable))]
    public class PlayerShip : BaseShip {

        public Hashtable StartModel { get; private set; }

        private static ILogger log = LogManager.GetCurrentClassLogger();
        
        private AIState mAI;
        private PlayerBonuses mBonuses;
        private MmoActor player;
        private ShipWeapon mWeapon;
        private PlayerCharacterObject mCharacter;



        public override void Start() {
            mWeapon = RequireComponent<ShipWeapon>();
            player = RequireComponent<MmoActor>();
            RequireComponent<PlayerSkills>();
            mAI = RequireComponent<AIState>();
            mBonuses = RequireComponent<PlayerBonuses>();
            mCharacter = RequireComponent<PlayerCharacterObject>();
            SetModel(new ShipModel(resource));
            
            log.Info("PlayerShip.Start() completed");
        }



        public void Load() {
            log.InfoFormat("PlayerShip LOad() [dy]");
            var workshop = (Workshop)GetComponent<CharacterObject>().workshop;
            var dropMgr = DropManager.Get(nebulaObject.world.Resource());


            //ShipModelDocument document = GameApplication.Instance.Load(player.nebulaObject.Id, mCharacter.characterId, DatabaseDocumentType.ShipModel) as ShipModelDocument;

            bool isNew = false;
            var dbModel = ShipModelDatabase.instance.LoadShipModel(mCharacter.characterId, resource as Res, out isNew);

            if (isNew) {
                GenerateNewShipModel(dropMgr);
                ShipModelDatabase.instance.SaveShipModel(mCharacter.characterId, shipModel);
            } else {
                shipModel.Replace(dbModel);
            }

            shipModel.Update();
            GetComponent<MmoActor>().EventOnShipModelUpdated();
            log.Info("PlayerShip.Load() completed");
        }

        public void SetStartModel(Hashtable inStartModel) {
            StartModel = inStartModel;
        }

        public override float damageResistance {
            get {
                //check if resist blocked
                if(blockResist.blocked) {
                    return 0f;
                }

                float val = shipModel.resistance;
                val *= Mathf.Clamp01(1 + mBonuses.resistPcBonus);
                val += mBonuses.resistCntBonus;
                val = Mathf.Clamp01(val);
                return val;
            }
        }

        public override int holdCapacity {
            get {
                return shipModel.cargo;
            }
        }

        public override void Update(float deltaTime) {

            base.Update(deltaTime);

            nebulaObject.properties.SetProperty((byte)PS.AngleSpeed, 0.5f);
            nebulaObject.properties.SetProperty((byte)PS.Acceleration, 20.0f);
            nebulaObject.properties.SetProperty((byte)PS.ModulePrefabs, shipModel.GetModulePrefabs());
            //nebulaObject.properties.SetProperty((byte)PS.Resist, damageResistance);


            //check for inventory size with cargo consistency
            if (player) {
                int cargo = holdCapacity;
                if (player.Inventory != null) {
                    if (player.Inventory.MaxSlots != cargo) {
                        if (player.GetComponent<PlayerLoaderObject>().loaded) {
                            player.Inventory.ChangeMaxSlots(cargo);
                            player.EventOnInventoryUpdated();
                        }
                    }
                }
            }
        }


        public override void SetModule(ShipModule module, out ShipModule prevModule) {
            base.SetModule(module, out prevModule);
           

            GetComponent<PlayerSkills>().UpdateSkills(shipModel);

            var player = GetComponent<MmoActor>();
            if (module != null) {
                player.application.MasterUpdateShipModule(player.nebulaObject.Id, mCharacter.characterId, module.SlotType, module.TemplateModuleId);
            }

            player.EventOnShipModelUpdated();
            player.EventOnSkillsUpdated();
        }


        public Hashtable EquipModule(string moduleId, WorkhouseStation station) {
            var character = GetComponent<CharacterObject>();
            var player = GetComponent<MmoActor>();


            ServerInventoryItem obj = null;
            if (!station.StationInventory.TryGetItem(InventoryObjectType.Module, moduleId, out obj)) {
                return new Hashtable {
                    { (int)SPC.ReturnCode, (int)RPCErrorCode.ObjectNotFound }
                };
            }

            if (!(obj.Object is ShipModule)) {
                return new Hashtable {
                    {(int)SPC.ReturnCode, (int)RPCErrorCode.InvalidObjectType }
                };
            }

            ShipModule module = obj.Object as ShipModule;
            if (module.Workshop != (Workshop)character.workshop) {
                return new Hashtable {
                    {(int)SPC.ReturnCode, (int)RPCErrorCode.InvalidObjectWorkshop},
                    {(int)SPC.Data, (byte)module.Workshop}
                };
            }

            ShipModel model = shipModel;
            int willBeHoldCount = model.HoldCountWhenInstalledThatModule(module);
            int usedInventoryCount = player.Inventory.SlotsUsed;

            if (willBeHoldCount < usedInventoryCount) {
                return new Hashtable {
                    { (int)SPC.ReturnCode, (int)RPCErrorCode.LowInventorySpace },
                    {   (int)SPC.Data, (usedInventoryCount - willBeHoldCount )}
                };
            }

            station.StationInventory.Remove(InventoryObjectType.Module, moduleId, 1);

            ShipModule prevModule = null;
            this.SetModule(module, out prevModule);
            player.Inventory.ChangeMaxSlots(holdCapacity);

            if (prevModule == null) {
                return new Hashtable
                {
                    {(int)SPC.ReturnCode, (int)RPCErrorCode.Ok }
                };
            }

            if (!station.StationInventory.Add(prevModule, 1)) {
                return new Hashtable {
                    { (int)SPC.ReturnCode, (int)RPCErrorCode.FailAddToStation }
                };
            }

            if (player != null) {
                player.EventOnStationHoldUpdated();
                player.EventOnInventoryUpdated();
                player.GetComponent<MmoMessageComponent>().ReceiveUpdateCombatStats();
            }

            return new Hashtable
                {
                    {(int)SPC.ReturnCode, (int)RPCErrorCode.Ok }
                };
        }




        #region PRIVATE METHODS
        private void GenerateNewShipModel(DropManager dropManager) {
            ShipModule prevModule = null;
            var character = GetComponent<CharacterObject>();

            Hashtable model = new Hashtable();
            foreach (DictionaryEntry entry in StartModel) {
                log.InfoFormat("start model {0} = {1}", entry.Key, entry.Value);
                model.Add(int.Parse(entry.Key.ToString()), entry.Value);
            }

            log.InfoFormat("Module: {0}", (string)model[(int)ShipModelSlotType.CB]);
            ModuleDropper.ModuleDropParams dpCB = new ModuleDropper.ModuleDropParams(nebulaObject.world.Resource(), (string)model[(int)ShipModelSlotType.CB],
                character.level, Difficulty.none, this.GenerateStartModuleCraftMaterials(), ObjectColor.white, string.Empty);
            var module = dropManager.GetModuleDropper(dpCB).Drop() as ShipModule;
            SetModule(module, out prevModule);
            log.InfoFormat("Generate module with skill = {0}", module.Skill);

            log.InfoFormat("Module: {0}", (string)model[(int)ShipModelSlotType.CM]);
            ModuleDropper.ModuleDropParams dpCM = new ModuleDropper.ModuleDropParams(nebulaObject.world.Resource(), (string)model[(int)ShipModelSlotType.CM],
                character.level, Difficulty.none, this.GenerateStartModuleCraftMaterials(), ObjectColor.white, string.Empty);
            module = dropManager.GetModuleDropper(dpCM).Drop() as ShipModule;
            SetModule(module, out prevModule);
            log.InfoFormat("Generate module with skill = {0}", module.Skill);

            log.InfoFormat("Module: {0}", (string)model[(int)ShipModelSlotType.DF]);
            ModuleDropper.ModuleDropParams dpDF = new ModuleDropper.ModuleDropParams(nebulaObject.world.Resource(), (string)model[(int)ShipModelSlotType.DF],
                character.level, Difficulty.none, this.GenerateStartModuleCraftMaterials(), ObjectColor.white, string.Empty);
            module = dropManager.GetModuleDropper(dpDF).Drop() as ShipModule;
            SetModule(module, out prevModule);
            log.InfoFormat("Generate module with skill = {0}", module.Skill);

            log.InfoFormat("Module: {0}", (string)model[(int)ShipModelSlotType.DM]);
            ModuleDropper.ModuleDropParams dpDM = new ModuleDropper.ModuleDropParams(nebulaObject.world.Resource(), (string)model[(int)ShipModelSlotType.DM],
                character.level, Difficulty.none, this.GenerateStartModuleCraftMaterials(), ObjectColor.white, string.Empty);
            module = dropManager.GetModuleDropper(dpDM).Drop() as ShipModule;
            SetModule(module, out prevModule);
            log.InfoFormat("Generate module with skill = {0}", module.Skill);

            log.InfoFormat("Module: {0}", (string)model[(int)ShipModelSlotType.ES]);
            ModuleDropper.ModuleDropParams dpES = new ModuleDropper.ModuleDropParams(nebulaObject.world.Resource(), (string)model[(int)ShipModelSlotType.ES],
                character.level, Difficulty.none, this.GenerateStartModuleCraftMaterials(), ObjectColor.white, string.Empty);
            module = dropManager.GetModuleDropper(dpES).Drop() as ShipModule;
            SetModule(module, out prevModule);
            log.InfoFormat("Generate module with skill = {0}", module.Skill);
        }

        /// <summary>
        /// Generate craft materials for start modules
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, int> GenerateStartModuleCraftMaterials() {
            Dictionary<string, int> craftMaterials = new Dictionary<string, int> {
                { nebulaObject.world.Resource().Materials.Ores[0].Id, 4},
                { nebulaObject.world.Resource().Materials.Ores[1].Id, 4}
            };
            return craftMaterials;
        }
        #endregion

    }
}