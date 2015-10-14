using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Common
{
    /*
    public static class GenericEventProps
    {
        //public const string SourceId = "SourceId";                                              //string
        //public const string TargetId = "TargetId";                                              //string
        //public const string SourceType = "SourceType";                                          //byte
        //public const string TargetType = "TargetType";                                          //byte
        public const string Type = "Type";                                                      //byte
        //public const string IsHItted = "IsHitted";
        public const string WorldEventId = "WorldEventId";
        public const string WorldEventReward = "WorldEventReward";
        public const string WorldEventTotalCount = "WorldEventTotalCount";
        public const string WorldEventCurrentCount = "WorldEventCurrentCount";
        public const string WorldEventActive = "WorldEventActive";
        public const string WorldId = "WorldId";

        //for chat
        public const string ChatMessageGroup = "ChatMessageGroup";              //(byte)(ChatGroup)
        public const string ChatMessage = "ChatMessage";                        //string
        public const string ChatSourceLogin = "ChatSourceLogin";
        public const string ChatMessageSourceName = "ChatMessageSourceName";    //string

        public const string ChatMessageId = "ChatMessageId";
        public const string ChatMessageTime = "ChatMessageTime";
        public const string ChatReceiverLogin = "ChatReceiverLogin";

        public const string InventoryItemId = "InventoryItemId";
        public const string InventoryItemType = "InventoryItemType";
        public const string InventoryItemName = "InventoryItemName";
        public const string InventoryItemLevel = "InventoryItemLevel";
        public const string InventoryItemDamage = "InventoryItemDamage";
        public const string InventoryItemCooldown = "InventoryItemCooldown";
        public const string InventoryItemOptimalDistance = "InventoryItemOptimalDistance";
        public const string InventoryItemWorkshop = "InventoryItemWorkshop";
        public const string InventoryItemCount = "InventoryItemCount";
        public const string InventoryItemTargetTemplateId = "InventoryItemTargetTemplateId";
        public const string InventoryItemTargetModuleSlotType = "InventoryItemTargetModuleSlotType";
        public const string InventoryItemCritChance = "InventoryItemCritChance";
        public const string InventoryItemCritDamage = "InventoryItemCritDamage";
        public const string InventoryItemRange = "InventoryItemRange";
        public const string InventoryItemFarDist = "InventoryItemFarDist";
        public const string InventoryItemNearDist = "InventoryItemNearDist";
        public const string InventoryItemFarProb = "InventoryItemFarProb";
        public const string InventoryItemNearProb = "InventoryItemNearProb";
        public const string InventoryItemMaxSpeed = "InventoryItemMaxSpeed";
        public const string InventoryItemMaterialType = "InventoryItemMaterialType";
        public const string InventoryItemMaterialTemplateId = "InventoryItemMaterialTemplateId";
        public const string InventoryItemCraftMaterials = "InventoryItemCraftMaterials";
        public const string InventoryItemColor = "InventoryItemColor";


        public const string InventoryMaxSlots = "InventoryMaxSlots";
        public const string InventorySlotsUsed = "InventorySlotsUsed";
        public const string InventoryItems = "InventoryItems";

        //---------------------------------------------------------------
        public const string id = "id";
        public const string type = "type";
        public const string level = "level";
        public const string name = "name";
        public const string workshop = "workshop";
        public const string set = "set";
        public const string count = "count";
        public const string skill = "skill";
        public const string hp = "hp";
        public const string hold = "hold";
        public const string resist = "resist";
        public const string speed = "speed";
        public const string damage_bonus = "damage_bonus";
        public const string cooldown_bonus = "cooldown_bonus";
        public const string distance_bonus = "distance_bonus";
        public const string color = "color";
        public const string info = "info";
        public const string templateId = "templateId";
        public const string prefab = "prefab";
        public const string hold_type = "hold_type";
        public const string hold_max_slots = "hold_max_slots";
        public const string crit_chance = "crit_chance";
        public const string crit_damage = "crit_damage";
        public const string has_weapon = "has_weapon";
        public const string ready = "ready";

        public const string light_ready = "light_ready";
        public const string heavy_ready = "heavy_ready";

        public const string damage = "damage";
        public const string light_damage = "light_damage";
        public const string heavy_damage = "heavy_damage";

        public const string range = "range";
        public const string cooldown = "cooldown";
        public const string light_cooldown = "light_cooldown";
        public const string heavy_cooldown = "heavy_cooldown";

        public const string hit_prob = "hit_prob";
        public const string source = "source";
        public const string timer = "timer";
        public const string light_timer = "light_timer";
        public const string heavy_timer = "heavy_timer";

        public const string items = "items";
        public const string exp = "exp";
        public const string group = "group";
        public const string craft_materials = "craft_materials";
        public const string duration = "duration";
        public const string skill_type = "skill_type";
        public const string slot_type = "slot_type";
        public const string energy = "energy";
        public const string require_weapon_slot = "require_weapon_slot";
        public const string success = "success";
        public const string ison = "ison";
        public const string data = "data";
        public const string source_type = "source_type";
        public const string target = "target";
        public const string target_type = "target_type";
        public const string cast_info = "cast_info";
        public const string fire_blocked = "fire_blocked";
        public const string optimal_distance = "optimal_distance";
        //public const string energy = "energy";
        public const string energy_restoration = "energy_restoration";

        //----------------------------------------
        public const string actual_damage = "actual_damage";
        public const string overheating_guns = "overheating_guns";
        public const string message = "message";
        public const string status = "status";

        public const string alive = "alive";
        public const string game_ref_id = "game_ref_id";
        public const string display_name = "display_name";
        public const string characters = "characters";
        public const string model = "model";
        public const string selected_character_id = "selected_character_id";
        public const string race = "race";
        public const string initial_owned_race = "initial_owned_race";
        public const string owned_race = "owned_race";
        public const string zone_info = "zone_info";
        public const string position = "position";
        public const string radius = "radius";
        public const string active = "active";
        public const string last_fire_time = "last_fire_time";
        public const string far_dist = "far_dist";
        public const string near_dist = "near_dist";
        public const string max_hit_speed = "max_hit_speed";
        public const string max_fire_distance = "max_fire_distance";
        public const string time = "time";
        public const string action = "action";
        public const string health = "health";
        public const string max_health = "max_health";
        public const string destoyed = "destoyed";
        public const string sub_type = "sub_type";
        public const string weapon = "weapon";
        public const string far_prob = "far_prob";
        public const string near_prob = "near_prob";

        //----------------------------------------------
        public const string coins = "coins";
        public const string total_count = "total_count";
        public const string killed_counter = "killed_counter";
        public const string description = "description";
        public const string specific_info = "specific_info";
        public const string target_world = "target_world";
        public const string fire_allowed = "fire_allowed";
        public const string inputs = "inputs";
        public const string target_id = "target_id";
        public const string is_hitted = "is_hitted";
        public const string source_id = "source_id";
        public const string is_critical = "is_critical";
        public const string difficulty = "difficulty";
        public const string has_skill = "has_skill";
        public const string use_time = "use_time";
        public const string is_on = "is_on";
        public const string inventory = "inventory";

        public const string stage_id = "stage_id";
        public const string start_text = "start_text";
        public const string task_text = "task_text";
        public const string is_final = "is_final";
        public const string is_success = "is_success";
        public const string timeout = "timeout";
        public const string current_stage = "current_stage";

        public const string heavy_shot_info = "heavy_shot_info";
        public const string light_shot_info = "light_shot_info";

        public const string shot_type = "shot_type";
        public const string error_message_id = "error_message_id";
        public const string damage_type = "damage_type";

        public const string slots_count = "slots_count";
        public const string world_id = "world_id";
        public const string zone_type = "zone_type";
        public const string num_slots = "num_slots";
        public const string production_speed = "production_speed";
        public const string protection_interval = "protection_interval";
        public const string attached_object = "attached_object";

        public const string sender_type = "sender_type";
        public const string sender_id = "sender_id";
        public const string receiver_type = "receiver_type";
        public const string receiver_id = "receiver_id";
        public const string title = "title";
        public const string body = "body";
        public const string attachments = "attachments";
        public const string readed = "readed";
        public const string mails = "mails";
        public const string sender_name = "sender_name";
        public const string receiver_name = "receiver_name";

        public const string container_id = "container_id";
        public const string container_type = "container_type";
        public const string container_item_id = "container_item_id";
        public const string container_item_type = "container_item_type";

        public const string is_god = "is_god";

        public const string request_type = "request_type";
        public const string is_leader = "is_leader";
        public const string buffs = "buffs";
        public const string item_id = "item_id";

        public const string from_character_id = "from_character_id";
        public const string from_dispay_name = "from_dispay_name";
        public const string to_exclude_character_id = "to_exclude_character_id";
        public const string to_exclude_display_name = "to_exclude_display_name";
        public const string members = "members";
        public const string is_opened = "is_opened";
    }*/
}
