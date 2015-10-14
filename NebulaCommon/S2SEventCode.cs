using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NebulaCommon {
    public enum S2SEventCode : byte {
        UpdateShipModel = 1,
        UpdateCharacter = 2,
        GroupUpdate = 100,
        GroupRemoved,
        GETInventoryItemStart,
        GETInventoryItemEnd,
        InvokeMethodStart,
        InvokeMethodEnd,
        PUTInventoryItemStart,
        PUTInventoryItemEnd,
        RaceStatusChanged,
        GETInventoryItemsStart,
        GETInventoryItemsEnd
    }
}
