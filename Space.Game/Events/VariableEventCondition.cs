using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Space.Game.Events
{
    public abstract class VariableEventCondition<T> : EventCondition 
    {

        private readonly T checkValue;
        private readonly string variableName;

        public VariableEventCondition( string variableName, T checkValue)
        {
            this.variableName = variableName;
            this.checkValue = checkValue;
        }

        public T CheckValue
        {
            get
            {
                return this.checkValue;
            }
        }



        public string VariableName
        {
            get
            {
                return this.variableName;
            }
        }
    }
}

