using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestClient.TestAttributes {
    [AttributeUsage(AttributeTargets.Class, AllowMultiple =true, Inherited =true)]
    public class RequireComponent : System.Attribute {

        public Type Type;

        public RequireComponent(Type type) {
            this.Type = type;
        }
    }
}
