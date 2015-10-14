using Nebula.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestClient.TestAttributes;

namespace TestClient.TestBehaviours {

    [REQUIRE_COMPONENT(typeof(ABehaviour))]
    public class FirstBehaviour : NebulaBehaviour {

        private float timer;

        public override void Start() {
            Console.WriteLine("FirstBehaviour Start()");
        }

        public override void Update(float deltaTime) {
            timer += deltaTime;
            Console.WriteLine("first behaviour timer " + timer );
        }

        public void TestMethod() {
            timer += 0.05f;
            Console.WriteLine("timer incremented in first behaviour " + timer);
        }

        public override int behaviourId {
            get {
                throw new NotImplementedException();
            }
        }
    }
}
