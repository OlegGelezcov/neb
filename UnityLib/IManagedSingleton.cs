using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula {
    public interface IManagedSingleton {
        string singletonName { get; }
        void ManagedDestroy();
    }
}
