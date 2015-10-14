using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestClient.TestBehaviours {
    public class TestMain {

        public static void TestAccessToDerivedBehaviour() {
            TestWorld world = new TestWorld();
            Dictionary<byte, object> tags = new Dictionary<byte, object>();
            Type[] components = new Type[] { typeof(SecondBehaviour), typeof(DerivedBehaviour) };
            TestNebulaObject nebulaObject = new TestNebulaObject(world, tags, 1, 0, components);

            if(nebulaObject.GetComponent<FirstBehaviour>()) {
                Console.WriteLine("Access to derived by base is OK");
            } else {
                Console.WriteLine("Access to derived by base ERROR");
            }

            if(nebulaObject.GetComponent<DerivedBehaviour>()) {
                Console.WriteLine("Access to derived by derived is OK");
            } else {
                Console.WriteLine("Access to derived by derived is ERROR");
            }

            for(int i = 0; i < 5; i++) {
                nebulaObject.Update(0.01f);
            }
        }

        public static void TestAttributes() {
            TestWorld world = new TestWorld();
            Dictionary<byte, object> tags = new Dictionary<byte, object>();
            Type[] components = new Type[] {typeof(SecondBehaviour), typeof(ABehaviour), typeof(BBehaviour) };
            TestNebulaObject nebulaObject = new TestNebulaObject(world, tags, 1, 0, components);
            for(int i = 0; i < 10; i++) {
                nebulaObject.Update(0.1f);
            }
        }


    }
}
