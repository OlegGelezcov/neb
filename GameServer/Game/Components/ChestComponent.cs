using Common;
using ExitGames.Logging;
using GameMath;
using Nebula.Balance;
using Nebula.Engine;
using Nebula.Game.Pets;
using Nebula.Inventory.DropList;
using Nebula.Inventory.Objects;
using ServerClientCommon;
using Space.Game;
using Space.Game.Drop;
using Space.Game.Inventory;
using Space.Game.Inventory.Objects;
using Space.Game.Resources;
using Space.Server;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Nebula.Game.Components {

    public class ChestComponent : NebulaBehaviour, IChest {

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public ConcurrentDictionary<string, ConcurrentDictionary<string, ServerInventoryItem>> content { get; private set; }

        public float duration { get; private set; }



        private float timer;

        public void SetDuration(float dur) {
            duration = dur;
            timer = duration;
        }

        public void Fill(ConcurrentDictionary<string, DamageInfo> inDamagers, List<ServerInventoryItem> itemsPerPlayer) {
            if(content == null ) {
                content = new ConcurrentDictionary<string, ConcurrentDictionary<string, ServerInventoryItem>>();
            }
            content.Clear();
            foreach(var pDamager in inDamagers) {
                content.TryAdd(pDamager.Key, CopyTransform(itemsPerPlayer));
            }
        }

        private ConcurrentDictionary<string, ServerInventoryItem> CopyTransform(List<ServerInventoryItem> items ) {
            var dict = new ConcurrentDictionary<string, ServerInventoryItem>();
            foreach(var it in items ) {
                dict.TryAdd(it.Object.Id, new ServerInventoryItem(it.Object, it.Count));
            }
            return dict;
        }

        public void Fill(ConcurrentDictionary<string, DamageInfo> inDamagers, DropListComponent dropListComponent, ChestSourceInfo sourceInfo = null) {

            content = new ConcurrentDictionary<string, ConcurrentDictionary<string, ServerInventoryItem>>();

            if (dropListComponent != null) {
                foreach (var damagePair in inDamagers) {
                    var dropPair = dropListComponent.GetDropList(damagePair.Value);
                    FillForDamager(damagePair.Value, dropPair.dropList, sourceInfo);
                }
            }
        }

        private void FillForDamager(DamageInfo damager, ItemDropList dropList, ChestSourceInfo sourceInfo) {

            ConcurrentDictionary<string, ServerInventoryItem> newObjects = new ConcurrentDictionary<string, ServerInventoryItem>();

            Workshop lootWorkshop = (Workshop)damager.workshop;

            //in 10% dropped source workshop items
            if (sourceInfo != null && sourceInfo.hasWorkshop) {
                if (Rand.Float01() < 0.1f) {
                    lootWorkshop = sourceInfo.sourceWorkshop;
                }
            }

            DropManager dropManager = DropManager.Get(resource);

            int lootLevel = damager.level;
            if(sourceInfo != null && sourceInfo.level != 0) {
                log.InfoFormat("set loot level = {0} yellow", sourceInfo.level);
                lootLevel = sourceInfo.level;
            }

            Difficulty d = Difficulty.starter;
            if(sourceInfo != null ) {
                d = sourceInfo.difficulty;
            }


            int groupCount = 1;

            NebulaObject playerObject = GetNebulaObject(damager);

            groupCount = GetGroupCount(playerObject);

            float remapWeight = GetColorRemapWeight(playerObject, groupCount);

            if (dropList == null) {
                //generate single weapon
                GenerateWeapons(dropManager, lootLevel, lootWorkshop, d, newObjects, remapWeight);

                GenerateSchemes(dropManager, lootLevel, lootWorkshop, d, newObjects, remapWeight);
            } else {
                GenerateFromDropList(
                    (Race)damager.race,
                    dropManager,
                    lootLevel,
                    lootWorkshop,
                    d,
                    newObjects,
                    remapWeight,
                    dropList,
                    damager.level,
                    groupCount);
            }
            //generate single scheme


            //generate creadits
            //CreditsObject creadits = new CreditsObject(resource.MiscItemDataRes.CreditsObject());
            //creadits.SetCount(20);
            //newObjects.TryAdd(creadits.Id, creadits);
            if( content.TryAdd(damager.DamagerId, newObjects) ) {
                if(damager.DamagerType == (byte)ItemType.Avatar) {
                    NotifyChestDamager(damager);
                }
            }
        }

        private void GenerateFromDropList(
            Race race,
            DropManager dropManager,
            int lootLevel,
            Workshop lootWorkshop,
            Difficulty difficulty,
            ConcurrentDictionary<string, ServerInventoryItem> newObjects,
            float remapWeight,
            ItemDropList dropList,
            int playerLevel,
            int groupCount) {

            foreach(var dropItem in dropList.items) {
                int count;
                if(dropItem.Roll(out count, groupCount, playerLevel)) {
                    if(count > 0 ) {
                        switch(dropItem.type) {
                            case InventoryObjectType.Weapon: {
                                    for(int i = 0; i < count; i++) {
                                        GenerateWeapon(dropManager, lootLevel, lootWorkshop, difficulty, newObjects, remapWeight);
                                    }
                                }
                                break;
                            case InventoryObjectType.Scheme: {
                                    for(int i = 0; i < count; i++ ) {
                                        GenerateScheme(dropManager, lootLevel, lootWorkshop, difficulty, newObjects, remapWeight);
                                    }
                                }
                                break;
                            case InventoryObjectType.Material: {
                                    var matDropItem = dropItem as MaterialDropItem;
                                    if(matDropItem != null ) {
                                        GenerateMaterials(matDropItem.templateId, count, newObjects);
                                    }
                                }
                                break;
                            case InventoryObjectType.nebula_element: {
                                    var nebElemDropItem = dropItem as NebulaElementDropItem;
                                    if(nebElemDropItem != null ) {
                                        GenerateNebulaElements(nebElemDropItem.templateId, count, newObjects);
                                    }
                                }
                                break;
                            case InventoryObjectType.craft_resource: {
                                    var craftResourceDropItem = dropItem as CraftResourceDropItem;
                                    if(craftResourceDropItem != null ) {
                                        GenerateCraftResource(craftResourceDropItem.templateId, count, newObjects);
                                    }
                                }
                                break;
                            case InventoryObjectType.pet_scheme: {
                                    var petSchemeDropItem = dropItem as PetSchemeDropItem;
                                    if(petSchemeDropItem != null ) {
                                        GeneratePetScheme(race ,petSchemeDropItem.template, petSchemeDropItem.petColor, count, newObjects);
                                    }
                                }
                                break;
                            case InventoryObjectType.contract_item: {
                                    var contractDropItem = dropItem as ContractObjectDropItem;
                                    if(contractDropItem != null ) {
                                        GenerateContractItemObject(contractDropItem.templateId, contractDropItem.contractId, count, newObjects);
                                    }
                                }
                                break;
                            case InventoryObjectType.planet_resource_hangar: {
                                    var hangarDropItem = dropItem as PlanetHangarDropItem;
                                    if(hangarDropItem != null &&  count > 0 ) {
                                        GeneratePlanetHangarScheme(count, newObjects);
                                    }
                                }
                                break;
                            case InventoryObjectType.planet_resource_accelerator: {
                                    var accDropItem = dropItem as PlanetResourceAcceleratorDropItem;
                                    if(accDropItem != null && count > 0 ) {
                                        GeneratePlanetResourceAcceleratorScheme(count, newObjects);
                                    }
                                }
                                break;
                        }
                    }
                }
            }
        }

        private void NotifyChestDamager(DamageInfo damager) {
            MmoWorld world = nebulaObject.mmoWorld();
            NebulaObject playerObj;
            if(world.TryGetObject((byte)damager.DamagerType, damager.DamagerId, out playerObj)) {
                var petManager = playerObj.GetComponent<PetManager>();
                if(petManager) {
                    petManager.ChestFilled(nebulaObject);
                }
            }
        }

        private void GeneratePlanetResourceAcceleratorScheme(int count, ConcurrentDictionary<string, ServerInventoryItem> newObjects) {
            PlanetResourceAcceleratorInventoryObject obj = new PlanetResourceAcceleratorInventoryObject();
            newObjects.TryAdd(obj.Id, new ServerInventoryItem(obj, count));
        }

        private void GeneratePlanetHangarScheme(int count, ConcurrentDictionary<string, ServerInventoryItem> newObjects ) {
            PlanetResourceHangarInventoryObject obj = new PlanetResourceHangarInventoryObject();
            newObjects.TryAdd(obj.Id, new ServerInventoryItem(obj, count));
        }

        private void GenerateContractItemObject(string template, string contractId, int count, ConcurrentDictionary<string, ServerInventoryItem> newObjects ) {
            if(string.IsNullOrEmpty(template)) {
                return;
            }
            ContractItemObject obj = new ContractItemObject(template, contractId);
            newObjects.TryAdd(obj.Id, new ServerInventoryItem(obj, count));
        }

        private void GeneratePetScheme(Race race, string template, PetColor color, int count, ConcurrentDictionary<string, ServerInventoryItem> newObjects) {
            if(string.IsNullOrEmpty(template)) {
                if (race != Race.None) {
                    template = resource.petParameters.defaultModels[race];
                } else {
                    template = "hp_1";
                }
            }
            PetSchemeObject obj = new PetSchemeObject(template, color, false);
            newObjects.TryAdd(obj.Id, new ServerInventoryItem(obj, count));
        }

        private void GenerateCraftResource(string template, int count, ConcurrentDictionary<string, ServerInventoryItem> newObjects) {
            if(string.IsNullOrEmpty(template)) {
                template = resource.craftObjects.random.id;
            }
            var data = resource.craftObjects[template];
            if (data != null) {
                CraftResourceObject obj = new CraftResourceObject(data.id, data.color, false);
                newObjects.TryAdd(obj.Id, new ServerInventoryItem(obj, count));
            }
        }

        private void GenerateNebulaElements(string template, int count, ConcurrentDictionary<string, ServerInventoryItem> newObjects) {
            if(string.IsNullOrEmpty(template)) {
                template = resource.PassiveBonuses.allData[Rand.Int(0, resource.PassiveBonuses.allData.Length - 1)].elementID;
            }
            NebulaElementObject obj = new NebulaElementObject(template, template);
            newObjects.TryAdd(obj.Id, new ServerInventoryItem(obj, count));
        }

        private void GenerateMaterials(string template, int count, ConcurrentDictionary<string, ServerInventoryItem> newObjects) {
            if(string.IsNullOrEmpty(template)) {
                template = resource.Materials.Ores.AnyElement().Id;
            }
            MaterialObject mat = new MaterialObject(template);
            newObjects.TryAdd(mat.Id, new ServerInventoryItem(mat, count));
        }

        private NebulaObject GetNebulaObject(DamageInfo damager) {
            NebulaObject playerObject;
            if(nebulaObject.mmoWorld().TryGetObject((byte)ItemType.Avatar, damager.DamagerId, out playerObject)) {
                return playerObject;
            }
            return null;
        }

        private int GetGroupCount(NebulaObject playerObject) {
            int groupCount = 1;
            if(playerObject != null ) {
                var playerCharacter = playerObject.GetComponent<PlayerCharacterObject>();

                if (playerCharacter != null) {
                    if (playerCharacter.hasGroup) {
                        if (playerCharacter.group.memberCount > 1) {
                            groupCount = playerCharacter.group.memberCount;
                        }
                    }
                }
            }
            return groupCount;
        }

        private float GetColorRemapWeight(NebulaObject playerObject,  int groupCount) {
            if(playerObject != null ) {
                var passiveBonuses = playerObject.GetComponent<PassiveBonusesComponent>();
                float remap = 0.0f;
                if (passiveBonuses != null) {
                    remap += passiveBonuses.coloredLootBonus;
                }

                var character = playerObject.GetComponent<PlayerCharacterObject>();

                remap += BalanceFormulas.RemapParameter(character.level, groupCount);
                return Mathf.Clamp01(remap);
            }
            return 0f;
        }

        private void GenerateSchemes(DropManager dropManager, int lootLevel, Workshop lootWorkshop, Difficulty d, ConcurrentDictionary<string, ServerInventoryItem> newObjects, float remapWeight) {
            for(int i = 0; i < ChestUtils.NumOfSchemes(d); i++) {
                var moduleTemplate = resource.ModuleTemplates.RandomModule(lootWorkshop, CommonUtils.RandomEnumValue<ShipModelSlotType>());
                var schemeDropper = dropManager.GetSchemeDropper(lootWorkshop, lootLevel, remapWeight);
                IInventoryObject schemeObject = schemeDropper.Drop() as IInventoryObject;
                newObjects.TryAdd(schemeObject.Id, new ServerInventoryItem(schemeObject, 1));
                //log.InfoFormat("scheme of level = {0} generated", schemeObject.Level);
            }
        }

        private void GenerateScheme(DropManager dropManager, int lootLevel, Workshop lootWorkshop, Difficulty d, ConcurrentDictionary<string, ServerInventoryItem> newObjects, float remapWeight) {
            var moduleTemplate = resource.ModuleTemplates.RandomModule(lootWorkshop, CommonUtils.RandomEnumValue<ShipModelSlotType>());
            var schemeDropper = dropManager.GetSchemeDropper(lootWorkshop, lootLevel, remapWeight);
            IInventoryObject schemeObject = schemeDropper.Drop() as IInventoryObject;
            newObjects.TryAdd(schemeObject.Id, new ServerInventoryItem(schemeObject, 1));
            //log.InfoFormat("scheme of level = {0} generated", schemeObject.Level);
        }


        private void GenerateWeapon(DropManager dropManager, int lootLevel, Workshop lootWorkshop, Difficulty d, ConcurrentDictionary<string, ServerInventoryItem> newObjects, float remapWeight) {
            ObjectColor color = resource.ColorRes.GenColor(ColoredObjectType.Weapon).color;
            WeaponDropper.WeaponDropParams weaponParams = new WeaponDropper.WeaponDropParams(resource, lootLevel, lootWorkshop, WeaponDamageType.damage, Difficulty.none);
            var weaponDropper = dropManager.GetWeaponDropper(weaponParams, remapWeight);
            IInventoryObject weaponObject = weaponDropper.Drop() as IInventoryObject;
            newObjects.TryAdd(weaponObject.Id, new ServerInventoryItem(weaponObject, 1));
            //log.InfoFormat("weapon of level = {0} generated", weaponObject.Level);
        }
        private void GenerateWeapons(DropManager dropManager, int lootLevel, Workshop lootWorkshop, Difficulty d, ConcurrentDictionary<string, ServerInventoryItem> newObjects, float remapWeight) {
            for(int i = 0; i < ChestUtils.NumOfWeapons(d); i++) {
                ObjectColor color = resource.ColorRes.GenColor(ColoredObjectType.Weapon).color;
                WeaponDropper.WeaponDropParams weaponParams = new WeaponDropper.WeaponDropParams(resource, lootLevel, lootWorkshop, WeaponDamageType.damage, Difficulty.none);
                var weaponDropper = dropManager.GetWeaponDropper(weaponParams, remapWeight);
                IInventoryObject weaponObject = weaponDropper.Drop() as IInventoryObject;
                newObjects.TryAdd(weaponObject.Id, new ServerInventoryItem(weaponObject, 1));
                //log.InfoFormat("weapon of level = {0} generated", weaponObject.Level);
            }
        }

        public override void Update(float deltaTime) {
            timer -= deltaTime;
            if(timer < 0 ) {
                (nebulaObject as Item).Destroy();
            }
        }

        public bool HasContentForActor(string actorId) {
            return (content.ContainsKey(actorId)) && (content[actorId].Count > 0);
        }

        public bool TryRemoveObject(string actorId, string objectId) {
            ConcurrentDictionary<string, ServerInventoryItem> filtered = null;
            if(content.TryGetValue(actorId, out filtered)) {
                ServerInventoryItem removedObject;
                return filtered.TryRemove(objectId, out removedObject);
            }
            return false;
        }

        public bool TryRemoveAllActorObjects(string actorId) {
            ConcurrentDictionary<string, ServerInventoryItem> filtered = null;
            return content.TryRemove(actorId, out filtered);
        } 

        public bool TryRemoveActorObjectids(string actorId, List<string> ids) {
            ConcurrentDictionary<string, ServerInventoryItem> filtered = null;
            bool result = true;

            if(content.TryGetValue(actorId, out filtered)) {
                foreach(var id in ids) {
                    ServerInventoryItem removed = null;
                    result = result && filtered.TryRemove(id, out removed);
                }

                if(filtered.Count == 0) {
                    ConcurrentDictionary<string, ServerInventoryItem> removedList = null;
                    content.TryRemove(actorId, out removedList);
                }
            }
            return result;
        }

        public bool TryGetActorObjects(string actorId, out ConcurrentDictionary<string, ServerInventoryItem> result) {
            return content.TryGetValue(actorId, out result);
        }

        public bool TryGetObject(string actorId, string objectId, out ServerInventoryItem resultObject) {
            resultObject = null;
            ConcurrentDictionary<string, ServerInventoryItem> filteredObjects = null;
            if(content.TryGetValue(actorId, out filteredObjects)) {
                if(filteredObjects.TryGetValue(objectId, out resultObject)) {
                    return true;
                }
            }
            return false;
        }

        public Hashtable GetInfoForActor(string actorId) {
            Hashtable info = new Hashtable();
            ConcurrentDictionary<string, ServerInventoryItem> filteredObjects = null;
            if(TryGetActorObjects(actorId, out filteredObjects)) {
                foreach(var pair in filteredObjects) {
                    info.Add(pair.Value.Object.Id, pair.Value.GetInfo());
                }
            }

            info.Add((int)SPC.Target, nebulaObject.Id);
            info.Add((int)SPC.TargetType, nebulaObject.Type);

            return info;
        }

        public object[] ContentRaw(string playerID ) {
            ConcurrentDictionary<string, ServerInventoryItem> filteredObjects = null;
            if (TryGetActorObjects(playerID, out filteredObjects)) {

                object[] raw = new object[filteredObjects.Count];
                int index = 0;
                foreach(var p in filteredObjects) {
                    raw[index++] = new Hashtable { { (int)SPC.Count, 1 }, { (int)SPC.Info, p.Value.GetInfo() } };
                }
                return raw;
            }
            return new object[] { };
        }

        public override int behaviourId {
            get {
                return (int)ComponentID.Chest;
            }
        }
    }

    public class ChestSourceInfo {
        public bool hasWorkshop { get; set; }
        public Workshop sourceWorkshop { get; set; }
        public int level { get; set; }
        public Difficulty difficulty { get; set; }
    }

    public static class ChestUtils {

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public static bool RollDropChest(Difficulty d) {
            log.InfoFormat("RollDropChest() for npc difficulty = {0} green", d);

            float randomNumber = Rand.Float01();
            log.InfoFormat("random number = {0} green", randomNumber);

            switch(d) {
                case Difficulty.starter:
                    return randomNumber < 0.3f;
                case Difficulty.easy:
                case Difficulty.easy2:
                    return randomNumber < 0.6f;
                default:
                    return true;
            }
        }

        public static int NumOfSchemes(Difficulty d) {
            switch(d) {
                case Difficulty.starter:
                    return 1;
                case Difficulty.easy:
                case Difficulty.easy2:
                    return 1;
                case Difficulty.medium:
                case Difficulty.none:
                    return 2;
                case Difficulty.hard:
                    return 4;
                case Difficulty.boss:
                    return 5;
                case Difficulty.boss2:
                    return 10;
                default:
                    return 0;
            }
        }

        public static int NumOfWeapons(Difficulty d) {
            switch(d) {
                case Difficulty.starter:
                    return 0;
                case Difficulty.easy:
                case Difficulty.easy2:
                    return 1;
                case Difficulty.medium:
                case Difficulty.none:
                    return 1;
                case Difficulty.hard:
                    return 2;
                case Difficulty.boss:
                    return 3;
                case Difficulty.boss2:
                    return 5;
                default:
                    return 0;
            }
        }

    }
}
