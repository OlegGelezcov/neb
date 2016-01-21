using Common;
using ExitGames.Logging;
using Nebula.Engine;
using Nebula.Game.Components;
using Nebula.Game.Pets;
using Nebula.Game.Utils;
using Nebula.Inventory.Objects;
using Nebula.Pets;
using ServerClientCommon;
using Space.Game;
using Space.Game.Inventory;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Game{

    public class PetOperations {
        private static readonly ILogger s_Log = LogManager.GetCurrentClassLogger();

        private MmoActor player { get; set; }

        public PetOperations(MmoActor player) {
            this.player = player;
        }

        public Hashtable GetPets() {
            return player.GetComponent<PetManager>().GetInfo();
        }

        public Hashtable AddRandomPet() {

            PetDropper.PetDropSettings settings = new PetDropper.PetDropSettings();
            settings.OnGenerateColor();

            settings.OffGeneratePassiveSkill();
            settings.SetPassiveSkill(-1);

            settings.OnGenerateModel();
            settings.OnGenerateActiveSkills();
            settings.OnGenerateDamageType();

            settings.OffSetMastery();
            settings.SetMastery(0);

            settings.OffGenerateRace();
            settings.SetRace((Race)player.GetComponent<RaceableObject>().race);

            PetDropper dropper = new PetDropper();
            PetInfo info = dropper.Drop(player.resource.petParameters, settings, player.resource.petSkills, player.resource.petPassiveBonuses);
            player.GetComponent<PetManager>().AddPet(info);
            return new Hashtable {
                {(int)SPC.ReturnCode, (int)RPCErrorCode.Ok },
                {(int)SPC.Info, info.GetInfo(player.resource) }
            };
        }


        public Hashtable TransformPetSchemeToPet(string schemeId) {
            var inventory = player.Station.StationInventory;
            var petManager = player.GetComponent<PetManager>();
            var playerRaceable = player.GetComponent<RaceableObject>();

            if(false == inventory.HasItem(InventoryObjectType.pet_scheme, schemeId)) {
                return CreateResponse(RPCErrorCode.ItemNotFound);
            }
            ServerInventoryItem schemeItem = null;
            if( false == inventory.TryGetItem(InventoryObjectType.pet_scheme, schemeId, out schemeItem) ) {
                return CreateResponse(RPCErrorCode.ErrorOfGettingInventoryItem);
            }

            PetSchemeObject petSchemeObject = schemeItem.Object as PetSchemeObject;

            if(false == petManager.hasFreeSpace) {
                return CreateResponse(RPCErrorCode.LowPetAngarSpace);
            }

            string model = player.resource.petParameters.defaultModels[(Race)playerRaceable.race];
            if(string.IsNullOrEmpty(model)) {
                return CreateResponse(RPCErrorCode.ResourceDataError);
            }

            PetDropper.PetDropSettings settings = new PetDropper.PetDropSettings();

            settings.OffGenerateColor();
            settings.SetColor(petSchemeObject.petColor);

            settings.OffGenerateModel();
            settings.SetModel(model);

            settings.OnGeneratePassiveSkill();
            settings.OnGenerateActiveSkills();
            settings.OnGenerateDamageType();

            settings.OnSetMastery();
            settings.SetMastery(0);

            settings.OffGenerateRace();
            settings.SetRace((Race)playerRaceable.race);

            PetDropper dropper = new PetDropper();
            PetInfo petInfo = dropper.Drop(player.resource.petParameters, settings, player.resource.petSkills, player.resource.petPassiveBonuses);

            inventory.Remove(InventoryObjectType.pet_scheme, schemeId, 1);

            if(false == petManager.AddPet(petInfo)) {
                return CreateResponse(RPCErrorCode.ErrorOfAddingPetToCollection);
            }

            player.EventOnStationHoldUpdated();

            Hashtable hash = CreateResponse(RPCErrorCode.Ok);
            hash.Add((int)SPC.Pet, petInfo.GetInfo(player.resource));

            return hash;
        }

        public Hashtable DestroyPet(string petId ) {
            var petManager = player.GetComponent<PetManager>();
            bool success = petManager.RemovePetInfo(petId);
            var response = CreateResponse(RPCErrorCode.Ok);
            response.Add((int)SPC.Status, success);
            return response;
        }

        private Hashtable CreateResponse(RPCErrorCode code) {
            return new Hashtable {
                { (int)SPC.ReturnCode, (int)code }
            };
        }

        /*
        public Hashtable ActivatePet(string id) {
            bool status = player.GetComponent<PetManager>().ActivatePet(id);
            return new Hashtable {
                {(int)SPC.ReturnCode, (int)RPCErrorCode.Ok },
                {(int)SPC.Status, status }
            };
        }


        public Hashtable DeactivatePet(string id) {
            bool status = player.GetComponent<PetManager>().DeactivatePet(id);
            return new Hashtable {
                {(int)SPC.ReturnCode, (int)RPCErrorCode.Ok },
                {(int)SPC.Status, status }
            };
        }*/

        public Hashtable ActivatePet(string deactivatePetId, string activatePetId ) {
            var petManager = player.GetComponent<PetManager>();
            bool deactivateSuccess = true;
            if(!string.IsNullOrEmpty(deactivatePetId)) {
                deactivateSuccess = petManager.DeactivatePet(deactivatePetId);
            }

            bool activateSuccess = true;
            if(!string.IsNullOrEmpty(activatePetId)) {
                activateSuccess = petManager.ActivatePet(activatePetId);
            }

            return new Hashtable {
                {(int)SPC.ReturnCode, (int)RPCErrorCode.Ok },
                {(int)SPC.Status, (deactivateSuccess && activateSuccess )  }
            };
        }

        /// <summary>
        /// Add or replace skill
        /// </summary>
        /// <param name="petId">Id of pet</param>
        /// <param name="oldSkill">old skill id, set zero if we only add new skill</param>
        /// <param name="newSkill">new skill will be setted</param>
        /// <returns></returns>
        public Hashtable AddOrReplaceActiveSkill(string petId, int oldSkill, int newSkill ) {
            var petManager = player.GetComponent<PetManager>();
            bool success = petManager.AddOrReplaceActiveSkill(petId, oldSkill, newSkill);
            s_Log.InfoFormat("changing pet skill {0} => {1} status: {2}".Color(LogColor.white), oldSkill, newSkill, success);
            return new Hashtable {
                { (int)SPC.ReturnCode, (int)RPCErrorCode.Ok },
                { (int)SPC.Status, success }
            };
        }

        public Hashtable SetPassiveSkill(string petId, int skillId ) {
            var petManager = player.GetComponent<PetManager>();
            bool success = petManager.SetPassiveSkill(petId, skillId);
            s_Log.InfoFormat("set passive skill to {0} => {1}".Color(LogColor.white), skillId, success);
            return new Hashtable {
                {(int)SPC.ReturnCode, (int)RPCErrorCode.Ok },
                {(int)SPC.Status, success }
            };
        }

        public Hashtable AddPetScheme() {
            PetSchemeObject schemeObject = new PetSchemeObject(Guid.NewGuid().ToString(), PetColor.gray, false);
            var inventory = player.Inventory;
            if(inventory.HasSlotsForItems(new List<string> { schemeObject.Id })) {
                inventory.Add(schemeObject, 1);
                player.EventOnInventoryUpdated();
                return new Hashtable {
                    {(int)SPC.ReturnCode, (int)RPCErrorCode.Ok },
                    {(int)SPC.Status, true }
                };
            }
            return new Hashtable {
                {(int)SPC.ReturnCode, (int)RPCErrorCode.UnknownError }
            };
        }

        public Hashtable AddAllCraftResources() {
            var stationInventory = player.Station.StationInventory;
            foreach(var data in player.resource.craftObjects.all) {
                if(false == stationInventory.HasSlotsForItems(new List<string> { data.id })) {
                    break;
                } else {
                    CraftResourceObject obj = new CraftResourceObject(data.id, data.color, false);
                    stationInventory.Add(obj, 10);
                }
            }
            player.EventOnStationHoldUpdated();
            return CreateResponse(RPCErrorCode.Ok);
        }

        public Hashtable AddPetSkin() {
            string skin = player.resource.petParameters.typeTable.GetRandomType((Race)player.GetComponent<RaceableObject>().race);
            if(false == string.IsNullOrEmpty(skin)) {
                PetSkinObject skinObject = new PetSkinObject(Guid.NewGuid().ToString(), skin, false);
                var inventory = player.Inventory;
                if(inventory.HasSlotsForItems(new List<string> { skinObject.Id })) {
                    if(inventory.Add(skinObject, 1)) {
                        player.EventOnInventoryUpdated();
                        return new Hashtable {
                            {(int)SPC.ReturnCode, (int)RPCErrorCode.Ok },
                            {(int)SPC.Status, true }
                        };
                    }
                }
            }
            return new Hashtable {
                {(int)SPC.ReturnCode, (int)RPCErrorCode.UnknownError }
            };
        }

        public Hashtable ImprovePetColor(string petId) {
            var petManager = player.GetComponent<PetManager>();
            var petData = petManager.GetPetInfo(petId);
            if(petData == null ) {
                return CreateResponse(RPCErrorCode.PetNotFound);
            }

            if(petData.hasMaxColor) {
                return CreateResponse(RPCErrorCode.PetAlreadyHasMaxColor);
            }

            var requirement = player.resource.petParameters.petUpgrades[petData.color];
            if(requirement == null ) {
                return CreateResponse(RPCErrorCode.ResourceDataError);
            }

            var stationInventory = player.Station.StationInventory;
            if(false == stationInventory.HasCraftResourceItems(requirement.entries)) {
                return CreateResponse(RPCErrorCode.NotEnoughInventoryItems);
            }

            if(false == petManager.ImproveColor(petId)) {
                return CreateResponse(RPCErrorCode.UnknownError);
            }

            if(false == stationInventory.RemoveCraftResourceItems(requirement.entries)) {
                return CreateResponse(RPCErrorCode.UnknownError);
            }

            UpgradeSkillWhenColorUp(petData);

            var message = player.GetComponent<MmoMessageComponent>();
            player.EventOnStationHoldUpdated();
            message.ReceivePetsUpdate();
            return CreateResponse(RPCErrorCode.Ok);
        }

        private void UpgradeSkillWhenColorUp(PetInfo pet) {
            var maxSkills = player.resource.petParameters.activeSkillCountTable[pet.color];         
            if(maxSkills > pet.skills.Count) {
                int count = maxSkills - pet.skills.Count;
                List<PetActiveSkill> newSkills = player.resource.petSkills.GetRandomSkills(count, pet.skills);
                if(newSkills.Count > 0 ) {
                    foreach(var skill in newSkills) {
                        pet.AddActiveSkill(skill);
                    }
                }
            }
        }

        public Hashtable ImprovePetMastery(string petId) {
            var petManager = player.GetComponent<PetManager>();
            var petInfo = petManager.GetPetInfo(petId);
            if(petInfo == null ) {
                return CreateResponse(RPCErrorCode.PetNotFound);
            }

            if(petInfo.hasMaxMastery) {
                return CreateResponse(RPCErrorCode.PetAlreadyHasMaxMastery);
            }

            var requirement = player.resource.petParameters.masteryUpgrades[petInfo.mastery];
            if(requirement == null ) {
                return CreateResponse(RPCErrorCode.ResourceDataError);
            }

            var stationInventory = player.Station.StationInventory;
            if(false == stationInventory.HasCraftResourceItems(requirement.entries)) {
                return CreateResponse(RPCErrorCode.NotEnoughInventoryItems);
            }

            if(petInfo.nextMastery > petInfo.maxAllowedMastery) {
                return CreateResponse(RPCErrorCode.PetColorNotEnoughForImproving);
            }

            if(false == petManager.ImproveMastery(petId)) {
                return CreateResponse(RPCErrorCode.UnknownError);
            }

            if(false == stationInventory.RemoveCraftResourceItems(requirement.entries)) {
                return CreateResponse(RPCErrorCode.UnknownError);
            }

            var message = player.GetComponent<MmoMessageComponent>();
            player.EventOnStationHoldUpdated();
            message.ReceivePetsUpdate();
            return CreateResponse(RPCErrorCode.Ok);
        }

        public Hashtable ChangePetSkin(string skinItemId, string petId ) {
            var petManager = player.GetComponent<PetManager>();
            var petInfo = petManager.GetPetInfo(petId);
            if (petInfo == null) {
                return CreateResponse(RPCErrorCode.PetNotFound);
            }

            var stationInventory = player.Station.StationInventory;
            if(false == stationInventory.HasItem(InventoryObjectType.pet_skin, skinItemId)) {
                return CreateResponse(RPCErrorCode.ItemNotFound);
            }

            ServerInventoryItem skinItem;
            if(false == stationInventory.TryGetItem(InventoryObjectType.pet_skin, skinItemId, out skinItem)) {
                return CreateResponse(RPCErrorCode.UnknownError);
            }

            var petSkinObject = skinItem.Object as PetSkinObject;

            if(petSkinObject.skin == petInfo.type ) {
                return CreateResponse(RPCErrorCode.PetAlreadySettedThisSkin);
            }

            if(false == player.resource.petParameters.typeTable.HasType(petSkinObject.skin)) {
                return CreateResponse(RPCErrorCode.ResourceDataError);
            }
            var typeObj = player.resource.petParameters.typeTable[petSkinObject.skin];

            var playerRaceable = player.GetComponent<RaceableObject>();

            if(typeObj != (Race)playerRaceable.race) {
                return CreateResponse(RPCErrorCode.InvalidRace);
            }

            if(false == petManager.SetModel(petId, petSkinObject.skin)) {
                return CreateResponse(RPCErrorCode.UnknownError);
            }

            stationInventory.Remove(InventoryObjectType.pet_skin, skinItemId, 1);
            player.EventOnStationHoldUpdated();

            var message = player.GetComponent<MmoMessageComponent>();
            message.ReceivePetsUpdate();

            return CreateResponse(RPCErrorCode.Ok);
        }

        public Hashtable ActivatePetSkill(string petId, int skillId, bool activate) {
            bool success = player.GetComponent<PetManager>().ActivateSkill(petId, skillId, activate);
            Hashtable hash = CreateResponse(RPCErrorCode.Ok);
            hash.Add((int)SPC.Status, success);
            return hash;
        }

        public Hashtable GetPetAtWorld(string itemId) {
            var world = player.nebulaObject.mmoWorld();
            NebulaObject petItem = null;
            if(false == world.TryGetObject((byte)ItemType.Bot, itemId, out petItem)) {
                return CreateResponse(RPCErrorCode.ItemNotFound);
            }
            var petComponent = petItem.GetComponent<PetObject>();
            if(false == petComponent) {
                return CreateResponse(RPCErrorCode.ComponentNotFound);
            }

            if(petComponent.info == null ) {
                return CreateResponse(RPCErrorCode.UnknownError);
            }

            var info = petComponent.info.GetInfo(player.resource);
            var result = CreateResponse(RPCErrorCode.Ok);
            result.Add((int)SPC.Info, info);
            return result;
        }
    }
}
