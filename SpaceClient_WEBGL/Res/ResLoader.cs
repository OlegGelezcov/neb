

namespace Nebula.Client.Res
{
    using Common;
    using System;
    using System.Collections.Generic;
    using System.Linq;
#if UP
    using Nebula.Client.UP;
#else
    using System.Xml.Linq;
#endif
    using ExitGames.Client.Photon;
    using Space.Game.Resources;
    using global::Common;

    public static class ResLoader
    {
        public static List<ResModuleData> LoadModules(string text)
        {
#if UP
            UPXDocument document = new UPXDocument(text);
#else
            XDocument document = XDocument.Parse(text);
#endif
            return document.Element("modules").Elements("module").Select(e =>
                {
                    return new ResModuleData
                    {
                        Id = e.Attribute("id").Value,
                        Workshop = (Workshop)System.Enum.Parse(typeof(Workshop), e.Attribute("workshop").Value),
                        SlotType = (ShipModelSlotType)System.Enum.Parse(typeof(ShipModelSlotType), e.Attribute("type").Value),
                        SetId = e.Attribute("set").Value,
                        Model = e.Attribute("model").Value,
                        NameId = e.Attribute("name").Value,
                        DescriptionId = e.Attribute("description").Value
                    };
                }).ToList();
        }

        public static List<ResMaterialData> LoadOres(string text)
        {
#if UP
            UPXDocument document = new UPXDocument(text);
#else
            XDocument document = XDocument.Parse(text);
#endif
            return document.Element("materials").Elements("material").Select(e =>
            {

                return new ResMaterialData
                {
                    Id = e.Attribute("id").Value,
                    Type = (MaterialType)System.Enum.Parse(typeof(MaterialType), e.Attribute("type").Value),
                    Name = e.Attribute("name").Value,
                    Description = e.Attribute("description").Value,
                    craftDescription = e.Attribute("cr_desc").Value,
                    Color = e.GetEnum<ObjectColor>("color")
                };
            }).ToList();
        }

        public static List<ResSkillData> LoadSkills(string text)
        {
#if UP
            UPXDocument document = new UPXDocument(text);
#else
            XDocument document = XDocument.Parse(text);
#endif
            return document.Element("skills").Elements("skill").Select(e => {
                Hashtable inputs = new Hashtable();
                var dump = e.Element("inputs").Elements("input").Select(i =>
                    {
                        inputs.Add(i.Attribute("key").Value, CommonUtils.ParseValue(i.Attribute("value").Value, i.Attribute("type").Value));
                        return i.Attribute("key").Value;
                    }).ToList();
                return new ResSkillData
                {
                    cooldown = e.GetFloat("cooldown"),
                    description = e.GetString("description"),
                    durability = e.GetFloat("durability"),
                    energy = e.GetFloat("energy"),
                    id = int.Parse(e.GetString("id"), System.Globalization.NumberStyles.HexNumber),
                    name = e.GetString("name"),
                    type = (SkillType)Enum.Parse(typeof(SkillType), e.Attribute("type").Value),
                    inputs = inputs,
                    targetType = (SkillTargetType)Enum.Parse(typeof(SkillTargetType), e.GetString("target_type"))
                };
            }).ToList();
        }

        public static List<ResSetData> LoadSets(string text)
        {
#if UP
            UPXDocument document = new UPXDocument(text);
#else
            XDocument document = XDocument.Parse(text);
#endif
            var moduleSets = document.Element("sets").Elements("set").Select(setElement =>
            {
                string id = setElement.Attribute("id").Value;
                string name = setElement.Attribute("name").Value;
                int unlockLevel = setElement.Attribute("unlock_level").ToInt();
                float dropProb = setElement.Attribute("drop_prob").ToFloat();
                Workshop workshop = (Workshop)Enum.Parse(typeof(Workshop), setElement.Attribute("workshop").Value);
                int skill = int.Parse(setElement.GetString("skill"), System.Globalization.NumberStyles.HexNumber);
                bool isDefault = setElement.GetBool("is_default");

                return new ResSetData(id, name, unlockLevel, dropProb, workshop, skill, isDefault);
            }).ToList();
            return moduleSets;
        }

        public static Dictionary<string, ResTooltipData> LoadTooltips(string text)
        {
#if UP
            UPXDocument document = new UPXDocument(text);
#else
            XDocument document = XDocument.Parse(text);
#endif
            return document.Element("tooltips").Elements("tooltip").Select(e =>
                {
                    string id = e.GetString("id");
                    string tooltipText = e.Value;
                    return new ResTooltipData { Id = id, Text = tooltipText };
                }).ToDictionary(t => t.Id, t => t);
        }

        public static Dictionary<BonusType, ResBuffData> LoadBuffs(string text)
        {
#if UP
            UPXDocument document = new UPXDocument(text);
#else
            XDocument document = XDocument.Parse(text);
#endif
            return document.Element("buffs").Elements("buff").Select(e =>
                {
                    return new ResBuffData
                    {
                        bonusType = (BonusType)Enum.Parse(typeof(BonusType), e.Attribute("type").Value),
                        description = e.Value.Trim(),
                        icon = e.Attribute("icon").Value,
                        isDebuff = bool.Parse(e.Attribute("is_debuff").Value)
                    };
                }).ToDictionary(b => b.bonusType, b => b);
        }

        public static Dictionary<string, string> LoadStrings(string xml)
        {
#if UP
            UPXDocument document = new UPXDocument(xml);
#else
            XDocument document = XDocument.Parse(xml);
#endif
            return document.Element("strings").Elements("string").Select(e =>
                {
                    string key = e.Attribute("key").Value;
                    string content = e.Value;
                    return new KeyValuePair<string, string>(key, content);
                }).ToDictionary(kv => kv.Key, kv => kv.Value);
        }

        public static Leveling LoadLeveling(string text)
        {
            var leveling = new Leveling();
            leveling.LoadFromXmlText(text);
            return leveling;
        }

        public static ResZones LoadZones(string text)
        {
            var zones = new ResZones();
            zones.Load(text);
            return zones;
        }

        public static ResWeapons LoadWeapons(string text)
        {
            var weapons = new ResWeapons();
            weapons.Load(text);
            return weapons;
        }

        public static ResHelp LoadHelp(string text)
        {
            ResHelp help = new ResHelp();
            help.Load(text);
            return help;
        }

        public static ResAsteroids LoadAsteroids(string text) {
            ResAsteroids asteroids = new ResAsteroids();
            asteroids.Load(text);
            return asteroids;
        }

        public static ResRaces LoadRaces(string text) {
            var resRaces = new ResRaces();
            resRaces.Load(text);
            return resRaces;
        }

        public static ResSchemes LoadSchemes(string text) {
            var resSchemes = new ResSchemes();
            resSchemes.Load(text);
            return resSchemes;
        }

        public static ResMiscInventoryItems LoadMiscInventoryItems(string text) {
            var miscItems = new ResMiscInventoryItems();
            miscItems.Load(text);
            return miscItems;
        }

        //public static ResGameEvents LoadGameEvents(string text) {
        //    var gameEvents = new ResGameEvents();
        //    gameEvents.Load(text);
        //    return gameEvents;
        //}

        public static ResPrefabsDB LoadPrefabsDB(string text) {
            var db = new ResPrefabsDB();
            db.Load(text);
            return db;
        }
    }
}
