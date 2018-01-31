using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Quests {
    public interface IQuestConditionContext {
        IKilledNpc KilledNpc { get; }
        int PlayerLevel { get; }
    }

    public interface IKilledNpc {
        int Level { get; }
        Workshop Class { get; }
        ObjectColor Color { get; }
    }
}
