using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Space.Game.Events
{
    public interface IVarInterpretator
    {
        object GetVariable(string name);
    }
}

