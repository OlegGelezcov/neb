using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.DSL {
    public abstract class EqualCondition<T> : Condition  {

        protected readonly T mValue;

        public EqualCondition(IConditionContext context, string name, T val)
            : base(context, name) {
            mValue = val;
        }

        //public override bool Check() {
        //    return (object)mValue == context.GetVariable(mVariableName);
        //}
    }
}
