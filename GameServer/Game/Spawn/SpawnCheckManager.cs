using Common;
using Nebula.Server.Components;
using Space.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Game.Spawn {
    public class SpawnCheckManager {

        private Dictionary<string, SpawnCheck> mChecks = new Dictionary<string, SpawnCheck>();

        public SpawnCheckManager() {
            mChecks.Add("outpost", new OutpostSpawnCheck());
        }

        public bool Check(MmoWorld world, NebulaObjectData data, out bool hasCheck) {
            hasCheck = false;
            if(data.componentCollection.ContainsKey(ComponentID.NebulaObject)) {
                NebulaObjectComponentData nebData = data.componentCollection[ComponentID.NebulaObject] as NebulaObjectComponentData;
                if(!string.IsNullOrEmpty(nebData.badge)) {
                    if (mChecks.ContainsKey(nebData.badge.ToLower().Trim())) {
                        hasCheck = true;
                        return mChecks[nebData.badge.ToLower().Trim()].Check(world, data);
                    }
                }
            }
            return true;
        }
    }
}
