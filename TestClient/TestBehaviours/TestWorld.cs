using Nebula.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Space.Game;

namespace TestClient.TestBehaviours {
    public class TestWorld : IBaseWorld {
        public bool AddObject(NebulaObject obj) {
            return true;
        }

        public List<NebulaObject> Filter(Func<NebulaObject, bool> filter) {
            return new List<NebulaObject>();
        }

        public void RemoveObject(byte objectType, string objectId) {
            
        }

        public IRes Resource() {
            return null;
        }

        public bool TryGetObject(byte objectType, string objectId, out NebulaObject obj) {
            obj = null;
            return true;
        }
    }
}
