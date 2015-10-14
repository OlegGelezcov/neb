using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Space.Game.Events
{
    public class EqualsVariableEventCondition<T> : VariableEventCondition<T> where T : IEquatable<T>
    {
        public EqualsVariableEventCondition(string variableName, T checkValue) 
            : base( variableName, checkValue)
        { }

        public override bool Check(IVarInterpretator interpretator)
        {
            if (((T)interpretator.GetVariable(VariableName)).Equals(CheckValue))
                return true;
            return false;
        }
    }
}

