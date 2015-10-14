using Nebula.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Space.Game;

namespace DSL_TEST.AddingAndRemovingComponents {
    public class AddingAndRemovingComponentsTest {

        private World world;

        public AddingAndRemovingComponentsTest() {
            world = new World();
        }

        public void Test() {
            NebulaObject obj = CreateObject();
            Update(0.1f, 3, obj);
            OutComponents(obj);
            obj.AddComponent<ThirdComponent>();
            Update(0.1f, 3, obj);
            OutComponents(obj);
            obj.RemoveComponent<ThirdComponent>();
            Update(0.1f, 3, obj);
            OutComponents(obj);
            obj.AddComponent<FourComponent>();
            Update(0.1f, 3, obj);
            OutComponents(obj);
            obj.RemoveComponent<ThirdComponent>();
            Update(0.1f, 3, obj);
            OutComponents(obj);
        }


        private  NebulaObject CreateObject() {
            return new NebulaObject(world, new Dictionary<byte, object>(), 1, 0, new Type[] { typeof(FirstComponent), typeof(SecondComponent) }); 
        }

        private void Update(float deltaTime, int times, NebulaObject targetObject) {
            for(int i = 0; i < times; i++) {
                targetObject.Update(deltaTime);
            }
            Console.WriteLine("========================================");
        }

        private void OutComponents(NebulaObject targetObject) {
            var components = targetObject.componentIds;
            StringBuilder builder = new StringBuilder();
            for(int i = 0; i < components.Length; i++) {
                builder.Append(components[i].ToString() + ", ");
            }
            Console.WriteLine(builder.ToString());
            Console.WriteLine("---------------------------------------");
        }
    }

    public class FirstComponent : NebulaBehaviour {
        public override int behaviourId {
            get {
                return 1001;
            }
        }

        public override void Update(float deltaTime) {
            Console.WriteLine("update first..."); ;
        }
    }

    public class SecondComponent : NebulaBehaviour {
        public override int behaviourId {
            get {
                return 1002;
            }
        }

        public override void Update(float deltaTime) {
            Console.WriteLine("update second...");
        }
    }

    public class ThirdComponent : NebulaBehaviour {

        private ConsoleColor mOldColor;

        public override int behaviourId {
            get {
                return 1003;
            }
        }

        public override void Update(float deltaTime) {
            SetColor(ConsoleColor.Yellow);
            Console.WriteLine("update third...");
            RestoreColor();
        }

        protected void SetColor(ConsoleColor color) {
            mOldColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
        }

        protected void RestoreColor() {
            Console.ForegroundColor = mOldColor;
        }

        public override void Awake() {
            SetColor(ConsoleColor.Yellow);
            Console.WriteLine("awake third...");
            RestoreColor();
        }

        public override void Start() {
            SetColor(ConsoleColor.Yellow);
            Console.WriteLine("start third...");
            RestoreColor();
        }
    }

    public class FourComponent : ThirdComponent {

        public override void Update(float deltaTime) {
            SetColor(ConsoleColor.Green);
            Console.WriteLine("update four...");
            RestoreColor();
        }

        public override void Awake() {
            SetColor(ConsoleColor.Green);
            Console.WriteLine("awake four...");
            RestoreColor();
        }

        public override void Start() {
            SetColor(ConsoleColor.Green);
            Console.WriteLine("start four...");
            RestoreColor();
        }
    }

    public class World : IBaseWorld {

        private Res resource = new Res("");

        public bool AddObject(NebulaObject obj) {
            return true;
        }

        public List<NebulaObject> Filter(Func<NebulaObject, bool> filter) {
            return new List<NebulaObject>();
        }

        public void RemoveObject(byte objectType, string objectId) {
            
        }

        public IRes Resource() {
            return resource;
        }

        public bool TryGetObject(byte objectType, string objectId, out NebulaObject obj) {
            obj = null;
            return true;
        }
    }
}
