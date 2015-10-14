using Nebula.Server.Components;
using Space.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Game.Spawn {
    public abstract class SpawnCheck {
        public abstract bool Check(MmoWorld world, NebulaObjectData data);
    }
}
