// DataResources.cs
// Nebula
// 
// Created by Oleg Zhelestcov on Wednesday, November 5, 2014 8:06:25 PM
// Copyright (c) 2014 KomarGames. All rights reserved.
//
namespace Nebula.Resources {
    using Common;
    using Nebula.Client.Res;
    using Space.Game.Resources;
    using System.Collections.Generic;
    using Nebula;


    using UnityEngine;
    using System.Xml.Linq;
    using Common.Space.Game.Resources;

    public class DataResources {

        private static DataResources instance;

        public static DataResources Instance {
            get {
                if (instance == null) {
                    instance = new DataResources();
                    instance.Load();
                }
                return instance;
            }
        }

        private bool loaded = false;
        private List<ResModuleData> modules;
        private List<ResMaterialData> ores;
        private List<ResSkillData> skills;
        private List<ResSetData> sets;
        private Dictionary<string, ResTooltipData> tooltips;
        private List<ResObjectIconData> objectIcons;
        private Dictionary<BonusType, ResBuffData> buffs;
        private Dictionary<string, string> moduleSetNames;
        //private Dictionary<GameSoundsType, AudioClip> gameSounds;
        private Leveling leveling;
        private ResZones zones;
        private ResWeapons weapons;
        private ResHelp help;
        private ResRaces races;
        private ResSchemes schemes;
        private ResMiscInventoryItems miscInventoryItems;
        public ResGameEvents gameEvents { get; private set; }
        public ResPrefabsDB prefabsDb { get; private set; }
        public ResLocations locations { get; private set; }
        public ResAsteroids asteroids { get; private set; }
        public FractionResolver fractions { get; private set; }
        public ResSkillLeveling skillLeveling { get; private set; }
        public ResWorkshopStats workshopStats { get; private set; }
        public SubZoneWorldCollection subZoneCollection { get; private set; }
        public ResConsumable consumables { get; private set; }

        public ResNebulaElements nebulaElements { get; private set; }

        public ResPassiveBonuses passiveBonuses { get; private set; }

        private Dictionary<string, string> strings;
        private static List<StringData> mLauncherStrings;


        private void Load() {
            if (false == this.loaded) {
                this.modules = ResLoader.LoadModules(Resources.Load<TextAsset>("Data/Drop/modules").text);
                this.ores = ResLoader.LoadOres(Resources.Load<TextAsset>("Data/Materials/ore").text);
                this.skills = ResLoader.LoadSkills(Resources.Load<TextAsset>("Data/Skills/skills").text);

                try {
                    this.sets = ResLoader.LoadSets(Resources.Load<TextAsset>("Data/Drop/module_set").text);
                } catch (System.FormatException fe) {
                    Debug.LogErrorFormat(fe.Message);
                    Debug.LogErrorFormat(fe.StackTrace);
                    sets = new List<ResSetData>();
                }
                this.tooltips = ResLoader.LoadTooltips(Resources.Load<TextAsset>("DataClient/tooltips").text);
                this.objectIcons = ObjectIcons.LoadObjectIcons(Resources.Load<TextAsset>("Data/object_icons").text);
                this.buffs = ResLoader.LoadBuffs(Resources.Load<TextAsset>("DataClient/buffs").text);
                this.moduleSetNames = ModulesSetNames.LoadModulesSetNames(Resources.Load<TextAsset>("Data/set_names").text);
                //this.gameSounds = ResSoundsData.LoadGameSounds(Resources.Load<TextAsset>("Data/sounds").text);
                this.leveling = ResLoader.LoadLeveling(Resources.Load<TextAsset>("Data/leveling").text);
                this.strings = LoadAllStringFromFiles("DataClient/Strings");
                this.zones = ResLoader.LoadZones(Resources.Load<TextAsset>("DataClient/zones").text);
                this.weapons = ResLoader.LoadWeapons(Resources.Load<TextAsset>("Data/Drop/weapons").text);
                this.help = ResLoader.LoadHelp(Resources.Load<TextAsset>("DataClient/help").text);
                this.races = ResLoader.LoadRaces(Resources.Load<TextAsset>("DataClient/races").text);
                this.schemes = ResLoader.LoadSchemes(Resources.Load<TextAsset>("DataClient/schemes").text);
                this.miscInventoryItems = ResLoader.LoadMiscInventoryItems(Resources.Load<TextAsset>("Data/misc_inventory_items").text);
                asteroids = ResLoader.LoadAsteroids(Resources.Load<TextAsset>("Data/asteroids").text);
                consumables = new ResConsumable();
                consumables.Load(Resources.Load<TextAsset>("Data/consumable").text);

                fractions = new FractionResolver();
                fractions.LoadFromXmlText(Resources.Load<TextAsset>("Data/fractions").text);

                skillLeveling = new ResSkillLeveling();
                LoadSkillLeveling();

                workshopStats = new ResWorkshopStats();
                workshopStats.Load(Resources.Load<TextAsset>("DataClient/stats").text);
                subZoneCollection = new SubZoneWorldCollection();

                var subZoneXml = Resources.Load<TextAsset>("DataClient/subzones").text;
                subZoneCollection.Load(subZoneXml);
                //gameEvents = ResLoader.LoadGameEvents(Resources.Load<TextAsset>("Data/zones").text);
                LoadGameEvents();
                LoadLocations();

                prefabsDb = ResLoader.LoadPrefabsDB(Resources.Load<TextAsset>("DataClient/prefabs_db").text);

                nebulaElements = new ResNebulaElements();
                nebulaElements.Load(Resources.Load<TextAsset>("Data/Materials/nebula_element").text);

                passiveBonuses = new ResPassiveBonuses();
                passiveBonuses.Load(Resources.Load<TextAsset>("Data/passive_bonuses").text);

                this.loaded = true;
            }
        }

        private void LoadSkillLeveling() {
            skillLeveling.Clear();
            string xmlHeal = Resources.Load<TextAsset>("Data/Skills/heal_skills").text;
            skillLeveling.Add(xmlHeal);
            string xmlRdd = Resources.Load<TextAsset>("Data/Skills/rdd_skills").text;
            skillLeveling.Add(xmlRdd);
            string xmlSdd = Resources.Load<TextAsset>("Data/Skills/sdd_skills").text;
            skillLeveling.Add(xmlSdd);
            string xmlTank = Resources.Load<TextAsset>("Data/Skills/tank_skills").text;
            skillLeveling.Add(xmlTank);
        }

        private void LoadGameEvents() {
            gameEvents = new ResGameEvents();
            TextAsset[] assets = Resources.LoadAll<TextAsset>("Data/Zones");
            try {
                foreach (var asset in assets) {
                    //Debug.LogFormat("load events from : {0}", asset.name);
                    gameEvents.AddEvents(gameEvents.LoadFile(asset.text));
                }
            } catch (System.Exception exception) {
                Debug.Log(exception.Message);
                Debug.Log(exception.StackTrace);
            }
        }

        private void LoadLocations() {
            locations = new ResLocations();
            TextAsset[] assets = Resources.LoadAll<TextAsset>("Data/Zones");
            try {
                foreach (var asset in assets) {
                    //Debug.LogFormat("load location from : {0}", asset.name);
                    //gameEvents.AddEvents(gameEvents.LoadFile(asset.text));
                    locations.Load(asset.text);
                }
            } catch (System.Exception exception) {
                Debug.Log(exception.Message);
                Debug.Log(exception.StackTrace);
            }
        }



        private Dictionary<string, string> LoadAllStringFromFiles(string directoryPath) {
            Dictionary<string, string> result = new Dictionary<string, string>();
            TextAsset[] assets = Resources.LoadAll<TextAsset>("DataClient/Strings");
            try {
                foreach (var asset in assets) {
                    try {
                        //Debug.Log("load string asset: {0}".f(asset.name).Color(Color.yellow));
                        result.AddRange(LoadStrings(asset.text));

                        //var newStrings = LoadStrings(asset.text);
                        //foreach(var kv in newStrings) {
                        //    if(!result.ContainsKey(kv.Key)) {
                        //        result.Add(kv.Key, kv.Value);
                        //    } else {
                        //        Debug.LogError("DUPLICATE STRING KEY: " + kv.Key);
                        //    }
                        //}
                    } catch (System.ArgumentException e) {
                        Debug.Log(e.Message);
                        Debug.LogError("exception in asset: " + asset.name);
                    }
                }
            } catch (System.ArgumentException e) {

                Debug.LogError(e.Message);
                Debug.LogError(e.ParamName);
                if (e.Data != null) {
                    foreach (var p in e.Data) {
                        Debug.LogError(p.ToString());
                    }
                }
            }
            return result;
        }



        private static Dictionary<string, string> LoadStrings(string xml) {
            XDocument document = XDocument.Parse(xml);
            Dictionary<string, string> strings = new Dictionary<string, string>();
            foreach (var e in document.Element("strings").Elements("string")) {
                string key = e.Attribute("key").Value;
                string content = e.Attribute(Language()).Value;
                if (!strings.ContainsKey(key)) {
                    strings.Add(key, content);
                } else {
                    Debug.LogError("DUPLICATE STRING KEY = " + key);
                }
            }
            return strings;
        }

        private static string Language() {
            switch (Application.systemLanguage) {
                case SystemLanguage.Russian:
                    return "ru";
            }
            return "en";
        }

        public static string GetLauncherString(string key) {
            if (mLauncherStrings == null) {
                mLauncherStrings = ResStrings.LoadData(Resources.Load<TextAsset>("DataClient/launcher_strings").text, Language());
            }
            foreach (var sData in mLauncherStrings) {
                if (sData.key == key) {
                    return sData.value;
                }
            }
            return string.Empty;
        }

        public ResModuleData ModuleData(string id) {
            if (false == this.loaded)
                return null;
            return GetModule(id);
        }

        private ResModuleData GetModule(string id) {
            foreach (var module in this.modules) {
                if (module.Id == id) {
                    return module;
                }
            }
            return null;
        }

        public List<ResModuleData> Modules {
            get {
                return this.modules;
            }
        }

        public ResMaterialData OreData(string id) {
            if (false == this.loaded)
                return null;
            return GetOreData(id);
        }

        private ResMaterialData GetOreData(string id) {
            foreach (var ore in this.ores) {
                if (ore.Id == id) {
                    return ore;
                }
            }
            return null;
        }

        public ResSkillData SkillData(int id) {
            if (false == this.loaded)
                return null;
            return GetSkillData(id);
        }

        private ResSkillData GetSkillData(int id) {
            foreach (var skill in this.skills) {
                if (skill.id == id) {
                    return skill;
                }
            }
            return null;
        }

        public ResSkillData SkillData(System.Func<ResSkillData, bool> predicate) {
            foreach (var s in this.skills) {
                if (predicate(s)) {
                    return s;
                }
            }
            return null;
        }

        public ResSetData SetData(string id) {
            if (false == this.loaded)
                return null;

            foreach (var s in this.sets) {
                if (s.Id == id) {
                    return s;
                }
            }
            return null;
        }



        public ResTooltipData ToolTip(string id) {
            ResTooltipData data = null;
            this.tooltips.TryGetValue(id, out data);

            if (data == null) {
                data = new ResTooltipData { Id = id, Text = string.Empty };
            }
            return data;
        }

        public ResObjectIconData ObjectIcon(string id) {
            if (false == this.loaded)
                return null;

            foreach (var objIcon in this.objectIcons) {
                if (objIcon.Id == id) {
                    return objIcon;
                }
            }
            return null;
        }

        public ResObjectIconData ObjectIcon(IconType type) {
            if (false == this.loaded)
                return null;
            foreach (var objIcon in this.objectIcons) {
                if (objIcon.Type == type) {
                    return objIcon;
                }
            }
            return null;
        }

        public ResBuffData Buff(BonusType bonusType) {
            if (this.buffs.ContainsKey(bonusType)) {
                return this.buffs[bonusType];
            }
            return null;
        }

        public string NameModelSet(string setId) {
            if (moduleSetNames.ContainsKey(setId)) {
                return moduleSetNames[setId];
            }
            return "not";
        }
        //public AudioClip GetGameSound(GameSoundsType type) {
        //    if (gameSounds.ContainsKey(type)) {
        //        return gameSounds[type];
        //    }
        //    return null;
        //}

        //public ResLoreData GetRandomLoreData() {
        //    if (lore != null)
        //        return lore[Random.Range(0, lore.Count)];

        //    return null;
        //}

        public Leveling Leveling {
            get {
                return this.leveling;
            }
        }

        public ResSchemes Schemes {
            get {
                return this.schemes;
            }
        }

        public ResRaces ResRaces() {
            return this.races;
        }

        public string String(string key) {
            if (this.strings.ContainsKey(key))
                return this.strings[key];
            //Debug.LogError("not founded string with key: " + key);
            return key;
        }

        public ResZoneInfo ZoneForId(string id) {
            return this.zones.Zone(id);
        }

        public ResZoneInfo ZoneForScene(string scene) {
            return this.zones.ZoneForScene(scene);
        }

        public ResWeaponTemplate Weapon(string id) {
            return this.weapons.Weapon(id);
        }

        public List<ResHelpElement> HelpElements() {
            return this.help.Elements();
        }

        public ResMiscInventoryItems ResMiscInventoryItems() {
            return this.miscInventoryItems;
        }

        public string GetGameRefID() {

            string gameRefID = PlayerPrefs.GetString(PrefKeys.GAME_REF_ID, string.Empty);
            //if (string.IsNullOrEmpty(gameRefID)) {
            //    gameRefID = System.Guid.NewGuid().ToString();
            //    PlayerPrefs.SetString("GAME_REF_ID", gameRefID);
            //    PlayerPrefs.Save();
            //}
            return gameRefID;
        }

        public string GetLogin() {
            return PlayerPrefs.GetString(PrefKeys.LOGIN, string.Empty);
        }

        public void UpdateGameRefID(string newID) {
            PlayerPrefs.SetString(PrefKeys.GAME_REF_ID, newID);
            PlayerPrefs.Save();
        }

        public Location GetLocation(string id) {
            return locations.GetLocation(id);
        }

        public string GetSavedChatGroup() {
            return PlayerPrefs.GetString(PrefKeys.SAVED_CHAT_GROUP, ChatGroup.zone.ToString());
        }

        public void SaveChatGroup(ChatGroup group) {
            PlayerPrefs.SetString(PrefKeys.SAVED_CHAT_GROUP, group.ToString());
        }
    }

}