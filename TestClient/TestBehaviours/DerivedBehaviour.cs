using Nebula.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestClient.TestAttributes;

namespace TestClient.TestBehaviours {

    [REQUIRE_COMPONENT(typeof(BBehaviour))]
    public class DerivedBehaviour : FirstBehaviour {

        public override void Awake() {
            Console.WriteLine("DerivedBehaviour.Awake()");
        }

        public override void Start() {
            Console.WriteLine("DerivedBehaviour.Start()");
        }

        public override void Update(float deltaTime) {
            Console.WriteLine("DerivedBehaviour.Update(float)");
        }

        public void CheckAttributes() {
            foreach(var attr in GetType().GetCustomAttributes(true)) {
                Console.WriteLine(attr.GetType().Name);
            }

        }
    }
}
