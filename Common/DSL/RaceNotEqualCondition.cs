﻿using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.DSL {
    public class RaceNotEqualCondition : NotEqualCondition<Race> {

        public RaceNotEqualCondition(IConditionContext context, string name, Race race)
            : base(context, name, race) { }

        public override bool Check() {
            return ((Race)context.GetVariable(mVariableName) != mValue);
        }
    }
}