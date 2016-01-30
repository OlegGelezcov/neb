using Common;
using ExitGames.Logging;
using Nebula.Database;
using Nebula.Engine;
using Nebula.Game.Components;
using Nebula.Game.Utils;
using Nebula.Pets;
using Space.Game;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System;
using System.Collections;
using GameMath;
using Nebula.Inventory.Objects;

namespace Nebula.Game.Pets {
    public class PetManager : NebulaBehaviour, IInfoSource {
        private const int kNoSkill = 0;

        private readonly float kPetRespawnInterval = 60;
        private const float kRecreateKilledPetsInterval = 10.0f;
        private const int kAutoLootPetBonus = 11;
        public const int kMaxPetCount = 10;
        private const string kCraftResourceForPetDeconstruction = "res_034";


        private static ILogger s_Log = LogManager.GetCurrentClassLogger();

        private PlayerCharacterObject m_Character;
        private MmoActor m_Player;
        private PlayerTarget m_PlayerTarget;
        private PlayerBonuses m_Bonuses;

        private float m_RecreateKilledPetsTimer = kRecreateKilledPetsInterval;

        private readonly ConcurrentDictionary<string, PetObject> m_Pets = new ConcurrentDictionary<string, PetObject>();
        private PetCollection m_PetInfoCollection = new PetCollection(kMaxPetCount);


        private bool m_Initialized = false;

        public void Load() {
            if (m_Character == null) {
                m_Character = GetComponent<PlayerCharacterObject>();
            }
            bool isNew = false;
            m_PetInfoCollection = PetDatabase.instance.LoadPets(m_Character.characterId, nebulaObject.resource as Res, out isNew);
            Reinitialize();
        }

        public override void Start() {
            base.Start();
            m_Character = GetComponent<PlayerCharacterObject>();
            m_Player = GetComponent<MmoActor>();
            m_PlayerTarget = GetComponent<PlayerTarget>();
            m_Bonuses = GetComponent<PlayerBonuses>();
        }

        public override void Update(float deltaTime) {
            try {
                if (!m_Initialized) {
                    m_Initialized = true;
                    CreatePets();
                } else {
                    //At space recreate killed pets every 10 seconds in no combat state
                    m_RecreateKilledPetsTimer -= deltaTime;
                    if (m_RecreateKilledPetsTimer <= 0.0f) {
                        m_RecreateKilledPetsTimer = kRecreateKilledPetsInterval;
                        CreateKilledPets();
                    }
                }
                //s_Log.InfoFormat("pet manager updated");
            } catch(Exception exception) {
                s_Log.InfoFormat(exception.Message);
                s_Log.InfoFormat(exception.StackTrace);
            }
        }

        public bool SetModel(string petId, string model) {
            return pets.SetModel(petId, model);
        }

        /// <summary>
        /// Ref to player
        /// </summary>
        private MmoActor player {
            get {
                return m_Player;
            }
        }

        /// <summary>
        /// Behaviour id of component
        /// </summary>
        public override int behaviourId {
            get {
                return (int)ComponentID.PetManager;
            }
        }

        /// <summary>
        /// Set flag to reinitialize
        /// </summary>
        public void Reinitialize() {
            s_Log.InfoFormat("pet manager reinitialized");
            m_Initialized = false;
        }

        public bool HasPetWithValidActiveSkill(int id) {
            foreach(var pet in m_Pets) {
                if(pet.Value.HasValidActiveSkill(id)) {
                    return true;
                }
            }
            return false;
        }

        public bool UseExplicit(int id, NebulaObject target) {
            foreach(var pet in m_Pets) {
                if(pet.Value.HasValidActiveSkill(id)) {
                    if(pet.Value.UseExplicit(id, target)) {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Add pet to collection and recreate if needed
        /// </summary>
        /// <param name="pet"></param>
        public bool AddPet(PetInfo pet) {
            bool success = pets.Add(pet);

            if (success) {
                var player = GetComponent<MmoActor>();
                if ((player && player.atSpace) || (false == player)) {
                    Reinitialize();
                }
            }
            return success;
        }

        public bool RemovePetInfo(string id) {
            var pet = pets.GetPet(id);
            if (pet != null) {

                bool success = pets.RemovePet(id);

                if(pet.color != PetColor.gray) {

                    if(m_Player) {

                        var stationInventory = m_Player.Station.StationInventory;

                        if(stationInventory.HasSlotsForItems(new List<string> { kCraftResourceForPetDeconstruction })) {

                            var craftData = resource.craftObjects[kCraftResourceForPetDeconstruction];

                            if(craftData != null ) {

                                CraftResourceObject craftResource = new CraftResourceObject(kCraftResourceForPetDeconstruction, craftData.color, false);
                                if(stationInventory.Add(craftResource, 1)) {
                                    m_Player.EventOnStationHoldUpdated();
                                }
                            }

                        }
                    }
                }

                if(success) {
                    if(m_Player) {
                        GetComponent<MmoMessageComponent>().ReceivePetsUpdate();
                    }
                }
                return success;
            }
            return false;
        }

        private void ReinitPetObject(PetInfo pet) {
            if (HasPetObject(pet.id)) {
                var petObj = GetPetObject(pet.id);
                if (petObj != null) {
                    petObj.Init(new PetObject.PetObjectInitData(nebulaObject, pet));
                }
            }
        }

        public bool AddOrReplaceActiveSkill(string petId, int oldSkill, int newSkill) {
            if(oldSkill == kNoSkill) {
                return AddActiveSkill(petId, newSkill);
            } else {
                var pet = pets.GetPet(petId);
                if (pet == null) {
                    return false;
                }
                bool successRemove = pet.RemoveActiveSkill(oldSkill);
                bool successAdd = true;

                if(successRemove) {
                    successAdd = AddActiveSkill(pet.id, newSkill);
                    if(!successAdd) {
                        ReinitPetObject(pet);
                    }
                }
                return (successRemove && successAdd);
            }
        }

        public bool AddActiveSkill(string petId, int skill ) {
            var pet = pets.GetPet(petId);
            if(pet == null ) {
                return false;
            }

            int maxSkills = resource.petParameters.activeSkillCountTable[pet.color];

            if(pet.GetActiveSkillCount() >= maxSkills ) {
                return false;
            }

            bool success = pet.AddActiveSkill(new PetActiveSkill {  id = skill, activated = true });
            if(success) {
                ReinitPetObject(pet);
            }
            return success;
        }

        public bool SetPassiveSkill(string petId, int skillID ) {
            var pet = pets.GetPet(petId);
            if(pet == null  ) {
                return false;
            }
            pet.SetPassiveSkill(skillID);
            ReinitPetObject(pet);
            return true;
        }

        /// <summary>
        /// Destroy not active pets from world
        /// </summary>
        private void DestroyNotActivePets() {
            //collect not active pet ids
            List<string> notActiveIds = new List<string>();
            foreach(var pet in pets.pets) {
                if(false == pet.active) {
                    notActiveIds.Add(pet.id);
                }
            }

            //if any not active pet exist at world destroy it
            List<string> destroyedPets = new List<string>();
            if(notActiveIds.Count > 0 ) {
                foreach(string id in notActiveIds) {
                    if(HasPetObject(id)) {
                        PetObject obj;
                        if(m_Pets.TryGetValue(id, out obj)) {
                            (obj.nebulaObject as GameObject).Destroy();
                            destroyedPets.Add(id);
                        }
                    }
                }
            }

            //If any pets was destroyed remove links to it from collection
            if(destroyedPets.Count > 0 ) {
                foreach(string id in destroyedPets) {
                    PetObject oldPet;
                    m_Pets.TryRemove(id, out oldPet);
                }
            }
        }

        /// <summary>
        /// Owner currently at space and not station
        /// </summary>
        private bool ownerAtSpace {
            get {
                if(player) {
                    if(player.atSpace) {
                        return true;
                    } else {
                        return false;
                    }
                } else {
                    return true;
                }
            }
        }

        /// <summary>
        /// Owner in comabt state now?
        /// </summary>
        private bool ownerInComabat {
            get {
                if(m_PlayerTarget) {
                    return m_PlayerTarget.inCombat;
                }
                return false;
            }
        }

        /// <summary>
        /// Owner in not combat state now?
        /// </summary>
        private  bool ownerNotCombat {
            get {
                return (false == ownerInComabat);
            }
        }

        /// <summary>
        /// Respawn interval expire on pet ??
        /// </summary>
        private bool RespawnOk(PetInfo pet) {
            return (CommonUtils.SecondsFrom1970() - pet.killedTime) > kPetRespawnInterval;
        }

        /// <summary>
        /// Spawn pet and add to pet collection and to world
        /// </summary>
        /// <param name="pet"></param>
        private void SpawnPet(PetInfo pet) {
            try {
                NebulaObject oldObject;

                if ((nebulaObject.world as MmoWorld).TryGetObject((byte)ItemType.Bot, pet.id, out oldObject)) {
                    (oldObject as GameObject).Destroy();
                }

                NebulaObject newPet = ObjectCreate.CreatePet(nebulaObject.world as MmoWorld, nebulaObject, pet);
                (newPet as GameObject).AddToWorld();
                m_Pets.TryAdd(newPet.Id, newPet.GetComponent<PetObject>());
                //s_Log.InfoFormat("pet = {0} spawned".Color(LogColor.white), pet.id);
            } catch(Exception exception) {
                s_Log.InfoFormat(exception.Message);
                s_Log.InfoFormat(exception.StackTrace);
            }
        }

        /// <summary>
        /// Spawn killed pets in space
        /// </summary>
        private void CreateKilledPets() {
           // s_Log.InfoFormat("CreateKilledPets(): pet collection count = {0}".Color(LogColor.orange), pets.pets.Count);
            //s_Log.InfoFormat("CreateKilledPets(): pet views count = {0}".Color(LogColor.orange), m_Pets.Count);

            if(ownerAtSpace) {
                foreach(var pet in pets.pets) {
                    if(pet.active) {
                        if(NoPetObject(pet.id) && RespawnOk(pet) && ownerNotCombat) {
                            s_Log.InfoFormat("spawn pet".Color(LogColor.orange));
                            SpawnPet(pet);
                        } else {
                            //if(false == NoPetObject(pet.id)) {
                            //    s_Log.InfoFormat("CreateKilledPets(): such pet already has view".Color(LogColor.orange));
                            //} else if(false == RespawnOk(pet)) {
                            //    s_Log.InfoFormat("CreateKilledPets(): respawn interval not completed".Color(LogColor.orange));
                            //} else if(false == ownerNotCombat) {
                            //    s_Log.InfoFormat("CreateKilledPets(): owner yet in combat".Color(LogColor.orange));
                            //}
                        }
                    } else {
                        //s_Log.InfoFormat("CreateKilledPets(): pet not active".Color(LogColor.orange));
                    } 
                }
            }
        }

        /// <summary>
        /// Create all pets
        /// </summary>
        public void CreatePets() {

            //Create all active pets
            foreach(var pet in m_PetInfoCollection.pets) {
                if(pet.active) {
                    if( RespawnOk(pet) && NoPetObject(pet.id)) {
                        SpawnPet(pet);
                    }
                }
            }

            //Destroy all invalid pets
            List<string> destroyedPets = new List<string>();
            foreach(var kvp in m_Pets) {
                if(false == pets.HasPet(kvp.Value.info.id)) {
                    (kvp.Value.nebulaObject as GameObject).Destroy();
                    destroyedPets.Add(kvp.Key);
                }
            }

            //if we destroyed something than remove links to it
            if(destroyedPets.Count > 0 ) {
                foreach(var id in destroyedPets) {
                    PetObject oldPet;
                    m_Pets.TryRemove(id, out oldPet);
                }
            }
            /*
            s_Log.InfoFormat("try create pets on pet manager [red]");
            if(m_Pets.Count == 0 ) {
                s_Log.InfoFormat("YES: pet is created... ".Color(LogColor.red));
                NebulaObject petObject = ObjectCreate.CreatePet(nebulaObject.world as MmoWorld, nebulaObject);
                (petObject as GameObject).AddToWorld();

                m_Pets.TryAdd(petObject.Id, petObject.GetComponent<PetObject>());

                petObject = ObjectCreate.CreatePet(nebulaObject.world as MmoWorld, nebulaObject);
                (petObject as GameObject).AddToWorld();
                m_Pets.TryAdd(petObject.Id, petObject.GetComponent<PetObject>());
            }*/
        }

        private bool NoPetObject(string id) {
            return (false == HasPetObject(id));
        }
        private bool HasPetObject(string id) {
            bool contains =  m_Pets.ContainsKey(id);
            if(contains) {

                PetObject val;

                if(m_Pets.TryGetValue(id, out val)) {
                    if(val == null || (false == val.nebulaObject)) {
                        m_Pets.TryRemove(id, out val);
                        return false;
                    }
                }
            }
            return contains;
        }
        private PetObject GetPetObject(string id) {
            PetObject obj;
            if(m_Pets.TryGetValue(id, out obj)) {
                return obj;
            }
            return null;
        }

        public void DestroyPets() {
            foreach (var kvp in m_Pets) {
                if (kvp.Value) {
                    (kvp.Value.nebulaObject as GameObject).Destroy();
                }
            }
        }

        /// <summary>
        /// Called when owner death occurs
        /// </summary>
        public void Death() {
            //s_Log.InfoFormat("Owner Death() and pets destroyed".Color(LogColor.orange));
            DestroyPets();
        }

        public void RemovePet(string id) {
            PetObject pet;
            if(m_Pets.TryRemove(id, out pet)) {
                //s_Log.InfoFormat("successfully removed pet = {0} [red]", id);
            }

            if(m_Pets.Count == 0 ) {
                //s_Log.InfoFormat("PetManager pets now is empty [red]");
            }
        }

        /// <summary>
        /// Update killed time on killed pet
        /// </summary>
        public void UpdateKilledTime(PetObject pet) {
           // s_Log.InfoFormat("update killed time on pet = {0}".Color(LogColor.orange), CommonUtils.SecondsFrom1970());
            var info = m_PetInfoCollection.GetPet(pet.nebulaObject.Id);
            info.SetKilledTime(CommonUtils.SecondsFrom1970());
        }

        public PetCollection pets {
            get {
                return m_PetInfoCollection;
            }
        }

        public PetInfo GetPetInfo(string petId ) {
            return pets.GetPet(petId);
        }

        public bool ImproveColor(string petId ) {
            return pets.ImproveColor(petId);
        }

        public bool ImproveMastery(string petId) {
            return pets.ImproveMastery(petId);
        }

        public bool hasFreeSpace {
            get {
                return pets.hasFreeSpace;
            }
        }

        public int countOfActivePets {
            get {
                return pets.countOfActivePets;
            }
        }

        public bool ActivateSkill(string petId, int skill, bool activated) {
            bool success = pets.ActivateSkill(petId, skill, activated);
            if(success) {
                if(ownerAtSpace) {
                    var pet = pets.GetPet(petId);
                    if(pet != null ) {
                        PetObject petObject = null;
                        if(m_Pets.TryGetValue(petId, out petObject)) {
                            petObject.Init(new PetObject.PetObjectInitData(nebulaObject, pet));
                        }
                    }
                }
                GetComponent<MmoMessageComponent>().ReceivePetsUpdate();
            }
            return success;
        }
        /// <summary>
        /// Add exp to all live pets
        /// </summary>
        /// <param name="exp"></param>
        public void AddExp(int exp) {
            foreach(var pPet in m_Pets) {
                pets.AddExp(pPet.Key, exp);
                var info = pets.GetPet(pPet.Key);
                if(info != null ) {
                    pPet.Value.SetInfo(info);
                }
            }
        }

        public Hashtable GetInfo() {
            return pets.GetInfo(nebulaObject.resource);
        }

        public bool ActivatePet(string id) {
            if(pets.ActivatePet(id)) {

                var player = GetComponent<MmoActor>();
                if((player && player.atSpace) || (false == player)) {
                    Reinitialize();
                }
                return true;
            }
            return false;
        }

        public bool DeactivatePet(string id) {
            if(pets.DeactivatePet(id)) {
                DestroyNotActivePets();
                return true;
            }
            return false;
        }

        public void ChestFilled(NebulaObject chestObject) {
            if(m_Bonuses) {
                if(false == Mathf.Approximately(m_Bonuses.autoLootBonus, 0f)) {
                    float dist = (chestObject.transform.position - transform.position).magnitude;
                    if(dist < 600) {
                        foreach(var kPet in m_Pets) {
                            if(kPet.Value.HasPassiveBonus(kAutoLootPetBonus)) {
                                kPet.Value.CollectChest(chestObject);
                                break;
                            }
                        }
                    }
                }
            }
        }

        public bool RollMastery() {
            foreach (var pet in m_Pets) {
                if(pet.Value.RollMastery()) {
                    return true;
                }
            }
            return false;
        }
    }
}
