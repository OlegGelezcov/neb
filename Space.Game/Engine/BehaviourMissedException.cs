using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Engine {
    public class BehaviourMissedException : Exception {

        public BehaviourMissedException(string name, Type type)
            : base(string.Format("{0} miss requered behaviour: {1}", name, type.ToString())) { }
    }
}
