using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.DSL {
    public interface IConditionContext {
        object GetVariable(string name);
    }
}
