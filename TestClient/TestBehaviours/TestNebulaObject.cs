using Nebula.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestClient.TestAttributes;

namespace TestClient.TestBehaviours {

    public class TestNebulaObject : NebulaObject {

        public TestNebulaObject(IBaseWorld world, Dictionary<byte, object> tags, float size, int subZone, params Type[] components)
            : base(world, tags, size, subZone, components) {

        }

        public void TestAttribute() {
            foreach(Attribute at in Attribute.GetCustomAttributes(GetComponent<DerivedBehaviour>().GetType(), true)) {
                if(at is RequireComponent) {
                    RequireComponent rc = at as RequireComponent;
                    Console.WriteLine("require type: " + rc.Type.Name);
                }
            }
        }
    } 
}
