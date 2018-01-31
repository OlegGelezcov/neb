using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Quests {
    public interface IQuestConditionContext {
        IKilledNpc KilledNpc { get; }
        IKilledPlayer KilledPlayer { get; }
        int PlayerLevel { get; }
        bool IsQuestCompleted(string questId);
        T GetVariable<T>(string name, T defaultValue = default(T));
        void ResetVariable<T>(string name);
        bool IsCreatedStructure(QuestStructureType type);
        string CapturedSystem { get; }
    }

    public interface IKilledNpc {
        int Level { get; }
        Workshop Class { get; }
        ObjectColor Color { get; }
    }

    public interface IKilledPlayer {
        Race Race { get; }
    }
}
