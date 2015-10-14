using Space.Game.Inventory;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Nebula.Game.Components {
    public interface IChest {
        bool TryGetObject(string playerID, string inventoryObjectID, out ServerInventoryItem obj);
        bool TryGetActorObjects(string playerID, out ConcurrentDictionary<string, ServerInventoryItem> playerObjects);
        bool TryRemoveActorObjectids(string playerID, List<string> inventoryObjectIDs);
        bool TryRemoveObject(string playerID, string inventoryObjectID);
        Hashtable GetInfoForActor(string actorId);
        object[] ContentRaw(string playerID);
    }
}
