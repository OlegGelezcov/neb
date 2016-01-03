using Nebula.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Game.Pets {
    public abstract class Condition {
        public abstract bool Check(NebulaObject source);
        public abstract void Renew();
    }
}
