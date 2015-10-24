using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Inap {
    public interface IStoreItemCollection {
        BaseStoreItem GetItem(string id);

        List<BaseStoreItem> items { get; }

        StoreType type { get; }
    }
}
