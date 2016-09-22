using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Quests {
    public abstract class VariableValueCondition : QuestCondition {

        public VariableValueCondition(string name)
            : base(name) { }

        public abstract string variableName { get; }
    }
}
