using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Quests.Drop {
    public class DropSource {
        public ItemType itempType { get; private set; }

        public DropSource(ItemType type) {
            this.itempType = type;
        }
    }
}
