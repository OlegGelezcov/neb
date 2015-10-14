using Space.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.DSL {
    public abstract class Condition {

        public IConditionContext context { get; private set; }
        protected readonly string mVariableName;

        public Condition(IConditionContext context, string variableName) {
            this.context = context;
            this.mVariableName = variableName;
        }

        public abstract bool Check();
    }
}
