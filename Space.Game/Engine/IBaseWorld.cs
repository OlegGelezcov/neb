using Space.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Engine {
    public interface IBaseWorld {

        bool TryGetObject(byte objectType, string objectId, out NebulaObject obj);
        void RemoveObject(byte objectType, string objectId);
        bool AddObject(NebulaObject obj);
        List<NebulaObject> Filter(Func<NebulaObject, bool> filter);

        IRes Resource();
    }
}
