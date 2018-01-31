using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common {
    public enum QuestConditionType : byte {
        player_level_ge,
        npc_killed_with_level,
        npc_killed_with_class,
        npc_killed_with_color,
        quest_completed,
        module_crafted,
        collect_ore,
        create_structure,
        level_reached,
        create_companion,
        player_killed,
        system_captured
    }
}
