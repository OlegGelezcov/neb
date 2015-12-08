using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.DSL {
    public abstract class NotEqualCondition<T> : Condition  {

        protected readonly T mValue;

        public NotEqualCondition(IConditionContext context, string varName, T val) 
            : base(context, varName) {
            mValue = val;
        }

        //public override bool Check() {
        //    return ((object)mValue != context.GetVariable(mVariableName));
        //}
    }
}
