using Common;
using Nebula.Game.Activators;
using Nebula.Game.Components.Activators;
using Space.Game;
using Space.Game.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Game {
    public class QuestItemUseChecker {

        public RPCErrorCode Check(MmoActor player, ServerInventoryItem item ) {
            switch(item.Object.Id) {
                case "analyzer001":
                    //return CheckAnaluzer001(player);
                    return RPCErrorCode.Ok;
                default:
                    return RPCErrorCode.InvalidObjectType;
            }
        }

        private RPCErrorCode CheckAnaluzer001(MmoActor player) {
            var activators = player.nebulaObject.mmoWorld().GetItems((item) => {
                var activator = item.GetComponent<ActivatorObject>();
                if (activator != null) {
                    if (item.badge == "shipwreck") {
                        return true;
                    }
                }
                return false;
            });

            if(activators.Count == 0 ) {
                return RPCErrorCode.NoValidObjects;
            }

            foreach(var kvp in activators ) {
                var item = kvp.Value;
                float dist = player.transform.DistanceTo(kvp.Value.transform);
                var activator = item.GetComponent<ActivatorObject>();
                if (dist < activator.radius  + item.size ) {
                    return RPCErrorCode.Ok;
                }
            }
            return RPCErrorCode.DistanceIsFar;
        }
    }
}
