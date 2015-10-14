using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nebula.Server.Components;
using Space.Game;

namespace Nebula.Game.Spawn {
    public class OutpostSpawnCheck : SpawnCheck {

        public override bool Check(MmoWorld world, NebulaObjectData data) {

            if(world.invulnerable) {
                return true;
            }
            return false;

        }
    }
}
