//namespace Nebula.Resources {
//    // StringCache.cs
//    // Nebula
//    // 
//    // Created by Oleg Zhelestcov on Wednesday, December 17, 2014 5:38:20 PM
//    // Copyright (c) 2014 KomarGames. All rights reserved.
//    //
//    using UnityEngine;
//    using System.Collections;
//    using System.Collections.Generic;
//    using Common;
//    using Nebula.Client.Res;
//    using ServerClientCommon;

//    public static class StringCache {

//        private static ItemNameResolver sNameResolver = new ItemNameResolver();
//        private static BuffDescriptionResolver sBuffDescriptionResolver = new BuffDescriptionResolver();
//        private static readonly StringSubCache<string> slotNames = new StringSubCache<string>();
//        private static readonly StringSubCache<int> raceCommandStatusText = new StringSubCache<int>();

//        public static string Get(string key) {
//            return BaseStringCache.Get(key);
//        }


//        public static ItemNameResolver nameResolver {
//            get {
//                return sNameResolver;
//            }
//        }

//        private static readonly StringSubCache<Race> raceStrings = new StringSubCache<Race>();
//        private static readonly StringSubCache<Workshop> workshopStrings = new StringSubCache<Workshop>();
//        private static readonly StringSubCache<ChatGroup> chatGroupNames = new StringSubCache<ChatGroup>();
//        private static readonly StringSubCache<ShipModelSlotType> moduleTypeNames = new StringSubCache<ShipModelSlotType>();
//        private static readonly StringSubCache<ModuleSetBonusType> setBonusTypes = new StringSubCache<ModuleSetBonusType>();

//        private static readonly InventoryItemDescriptionResolver sInventoryItemDescriptionResolver = new InventoryItemDescriptionResolver();
//        private static readonly InventoryItemNameResolver sInventoryItemNameResolver = new InventoryItemNameResolver();
//        private static readonly SkillDescriptionResolver sSkillDescriptionResolver = new SkillDescriptionResolver();
//        private static readonly RPCErrorCodeToStringResolver sRPCErrorCodeResolver = new RPCErrorCodeToStringResolver();
//        private static readonly StringSubCache<GuildMemberStatus> sGuildStatus = new StringSubCache<GuildMemberStatus>();
//        private static readonly StringSubCache<string> sConsumableNames = new StringSubCache<string>();
//        private static readonly StringSubCache<string> sConsumableDescriptions = new StringSubCache<string>();


//        public static BuffDescriptionResolver buffDescriptionResolver {
//            get {
//                return sBuffDescriptionResolver;
//            }
//        }

//        public static RPCErrorCodeToStringResolver rpcErrorCodeResolver {
//            get {
//                return sRPCErrorCodeResolver;
//            }
//        }


//        public static InventoryItemDescriptionResolver inventoryItemDescriptionResolver {
//            get {
//                return sInventoryItemDescriptionResolver;
//            }
//        }

//        public static InventoryItemNameResolver inventoryItemNameResolver {
//            get {
//                return sInventoryItemNameResolver;
//            }
//        }

//        public static SkillDescriptionResolver skillDescriptionResolver {
//            get {
//                return sSkillDescriptionResolver;
//            }
//        }

//        public static string SetBonus(ModuleSetBonusType bonusType) {
//            return setBonusTypes.String(bonusType, setBonusKeys[bonusType]);
//        }

//        public static string Race(Race race) {
//            string key = string.Empty;
//            switch (race) {
//                case Common.Race.Humans:
//                    key = "RACE_HUMANS";
//                    break;
//                case Common.Race.Borguzands:
//                    key = "RACE_BORGUZANDS";
//                    break;
//                case Common.Race.Criptizoids:
//                    key = "RACE_CRIPTIZIDS";
//                    break;
//                case Common.Race.None:
//                    key = "RACE_NONE";
//                    break;
//            }

//            return raceStrings.String(race, key).Trim();
//        }

//        public static string ChatGroupName(ChatGroup chatGroup) {
//            return chatGroupNames.String(chatGroup, chatGroupNameKeys[chatGroup]);
//        }

//        public static string Workshop(Workshop workshop) {
//            return workshopStrings.String(workshop, workshopNameKeys[workshop]).Trim();
//        }

//        public static string ModuleType(ShipModelSlotType moduleType) {
//            return moduleTypeNames.String(moduleType, moduleTypeNameKeys[moduleType]);
//        }

//        private static readonly Dictionary<Common.Workshop, string> workshopNameKeys = new Dictionary<Common.Workshop, string> {
//        {Common.Workshop.DarthTribe, "WORKSHOP_DARTHTRIBE" },
//        {Common.Workshop.RedEye, "WORKSHOP_REDEYE" },
//        {Common.Workshop.Equilibrium, "WORKSHOP_EQUILIBRIUM"},
//        //{Common.Workshop.Evasive, "WORKSHOP_EVASIVE"},
//        {Common.Workshop.BigBang, "WORKSHOP_BIGBANG"},
//        //{Common.Workshop.Serenity, "WORKSHOP_SERENITY" },
//        {Common.Workshop.Rakhgals, "WORKSHOP_RAKHGALS"},
//        {Common.Workshop.Phelpars, "WORKSHOP_PHELPARS"},
//        {Common.Workshop.Zoards, "WORKSHOP_ZOARDS"},
//        {Common.Workshop.Lerjees, "WORKSHOP_LERJEES"},
//        {Common.Workshop.Yshuar, "WORKSHOP_YSHUAR"},
//        {Common.Workshop.KrolRo, "WORKSHOP_KROLRO"},
//        {Common.Workshop.Arlen, "WORKSHOP_ARLEN"},
//        {Common.Workshop.Dyneira, "WORKSHOP_DYNEIRA"},
//        //{Common.Workshop.Molvice, "WORKSHOP_MOLVICE"}
//    };

//        private static readonly Dictionary<ChatGroup, string> chatGroupNameKeys = new Dictionary<ChatGroup, string> {
//        {ChatGroup.group,               "CHAT_GROUP_GROUP"          },
//        {ChatGroup.whisper,             "CHAT_GROUP_WHISPER"        },
//        {ChatGroup.zone,                "CHAT_GROUP_ZONE"           },
//        {ChatGroup.guild,               "CHAT_GROUP_GUILD"          }
//    };

//        private static readonly Dictionary<ShipModelSlotType, string> moduleTypeNameKeys = new Dictionary<ShipModelSlotType, string> {
//        {ShipModelSlotType.CB, "CB_NAME" },
//        {ShipModelSlotType.CM, "CM_NAME" },
//        {ShipModelSlotType.DF, "DF_NAME" },
//        {ShipModelSlotType.DM, "DM_NAME" },
//        {ShipModelSlotType.ES, "ES_NAME" }
//    };

//        private static readonly Dictionary<ModuleSetBonusType, string> setBonusKeys = new Dictionary<ModuleSetBonusType, string> {
//        { ModuleSetBonusType.crit_chance,           "BT_CC" },
//        { ModuleSetBonusType.crit_damage,           "BT_CRDAM" },
//        {ModuleSetBonusType.heal,                   "BT_HEAL" },
//        { ModuleSetBonusType.heavy_attack_damage,   "BT_HAD"},
//        { ModuleSetBonusType.hold,                  "BT_ISC"},
//        {ModuleSetBonusType.hp,                     "BT_HP" },
//        {ModuleSetBonusType.optimal_distance,       "BT_OD" },
//        { ModuleSetBonusType.resistance,            "BT_RES" },
//        {ModuleSetBonusType.speed,                  "BT_SPEED" }
//    };

//        public static string SlotTypeName(ShipModelSlotType slotType, Workshop workshop) {
//            var race = CommonUtils.RaceForWorkshop(workshop);
//            string prefix = string.Empty;
//            switch (race) {
//                case Common.Race.Humans:
//                    prefix = "H_M_";
//                    break;
//                case Common.Race.Borguzands:
//                    prefix = "B_M_";
//                    break;
//                case Common.Race.Criptizoids:
//                    prefix = "C_M_";
//                    break;
//            }

//            string key = string.Empty;
//            switch (slotType) {
//                case ShipModelSlotType.CB:
//                    key = prefix + "CB_NAME";
//                    break;
//                case ShipModelSlotType.CM:
//                    key = prefix + "CM_NAME";
//                    break;
//                case ShipModelSlotType.DF:
//                    key = prefix + "DF_NAME";
//                    break;
//                case ShipModelSlotType.DM:
//                    key = prefix + "DM_NAME";
//                    break;
//                case ShipModelSlotType.ES:
//                    key = prefix + "ES_NAME";
//                    break;
//            }

//            return slotNames.String(key, key);
//        }

//        public static string GetGuildStatus(GuildMemberStatus status) {
//            string path = string.Empty;
//            switch (status) {
//                case GuildMemberStatus.Member:
//                    path = "s_member";
//                    break;
//                case GuildMemberStatus.Moderator:
//                    path = "s_moder";
//                    break;
//                case GuildMemberStatus.Owner:
//                    path = "s_admin";
//                    break;
//            }
//            if (!string.IsNullOrEmpty(path)) {
//                return sGuildStatus.String(status, path);
//            }
//            return string.Empty;
//        }

//        public static string GetCommandRaceStatus(int commandStatus) {
//            string path = string.Empty;
//            switch (commandStatus) {
//                case RaceCommandKey.COMMANDER:
//                    path = "s_commander";
//                    break;
//                case RaceCommandKey.ADMIRAL1:
//                case RaceCommandKey.ADMIRAL2:
//                    path = "s_admiral";
//                    break;
//            }
//            if (string.IsNullOrEmpty(path)) {
//                return string.Empty;
//            }
//            return raceCommandStatusText.String(commandStatus, path);
//        }

//        public static string GetConsumableName(string id) {
//            var info = DataResources.Instance.consumables.GetConsumableInfo(id);
//            if (info == null) {
//                return "(not found)";
//            }
//            return sConsumableNames.String(info.name, info.name);
//        }

//        public static string GetConsumableDescription(string id) {
//            var info = DataResources.Instance.consumables.GetConsumableInfo(id);
//            if (info == null) {
//                return "(not found)";
//            }
//            return sConsumableDescriptions.String(info.description, info.description);
//        }
//    }

//    public class SkillDescriptionResolver {
//        private StringSubCache<string> subCache = new StringSubCache<string>();

//        public string Resolve(Race race, ResSkillData data) {
//            string prefix = string.Empty;
//            switch (race) {
//                case Race.Humans: prefix = "H"; break;
//                case Race.Borguzands: prefix = "B"; break;
//                case Race.Criptizoids: prefix = "C"; break;
//            }
//            return subCache.String(prefix + data.id.ToString("X8").ToUpper(), prefix + data.id.ToString("X8").ToUpper());
//        }
//    }


//}