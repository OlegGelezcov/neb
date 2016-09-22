using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Quests.Drop {
    public class DropInfo {
        public DropItem item { get; private set; }
        public DropSource source { get; private set; }

        public DropInfo(DropItem item, DropSource source) {
            this.item = item;
            this.source = source;
        }

    }
}
