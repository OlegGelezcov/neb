using Nebula.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestClient.TestBehaviours {

    [REQUIRE_COMPONENT(typeof(FirstBehaviour))]
    public class SecondBehaviour : NebulaBehaviour {

        private float timer = 0f;

        public override void Start() {
            Console.WriteLine("Second behaviour start");
        }

        public override void Update(float deltaTime) {
            timer += deltaTime / 2;
            Console.WriteLine("second timer " + timer);
        }

        public void SomeMethod() {
            timer += 0.01f;
            Console.WriteLine("Timer incremented in second behaviour");
        }

        public override int behaviourId {
            get {
                throw new NotImplementedException();
            }
        }
    }
}
