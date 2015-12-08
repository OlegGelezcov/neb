using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.DSL {
    public  class RaceEqualCondition : EqualCondition<Race>{
        public RaceEqualCondition(IConditionContext context, string name, Race race)
            : base(context, name, race) { }

        public override bool Check() {
            return ((Race)context.GetVariable(mVariableName)) == mValue;
        }
    }
}
