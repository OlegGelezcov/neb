using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common {
    public enum QuestType : byte {
        kill_npc = 1,
        craft_module = 2,
        collect_ore = 3,
        create_structure = 4,
        reach_level = 5,
        create_companion = 6,
        kill_player = 7,
        capture_system = 8
    }
}
