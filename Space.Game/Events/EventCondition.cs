using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Space.Game.Events
{
    public abstract class EventCondition
    {
        public abstract bool Check(IVarInterpretator interpretator);
    }
}

