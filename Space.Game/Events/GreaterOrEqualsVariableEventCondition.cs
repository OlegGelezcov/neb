using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Space.Game.Events
{
    public class GreaterOrEqualsVariableEventCondition<T>:  VariableEventCondition<T> where T : IEquatable<T>, IComparable<T>
    {
        public GreaterOrEqualsVariableEventCondition(string variableName, T checkValue)
            : base(variableName, checkValue)
        {}

        public override bool Check(IVarInterpretator interpretator)
        {
            int result = ((T)interpretator.GetVariable(VariableName)).CompareTo(CheckValue);
            return result >= 0;
        }
    }
}
