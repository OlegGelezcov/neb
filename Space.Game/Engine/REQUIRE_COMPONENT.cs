using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Engine {
    [AttributeUsage(AttributeTargets.Class, AllowMultiple =true, Inherited =true)]
    public class REQUIRE_COMPONENT : Attribute {

        public Type Type;

        public REQUIRE_COMPONENT(Type type) {
            this.Type = type;
        }
    }
}
