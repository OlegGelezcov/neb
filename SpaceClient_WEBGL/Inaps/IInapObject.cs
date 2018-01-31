using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Client.Inaps {
    public interface IInapObject {
        string Icon { get; }
        string Name { get; }
        string Description { get; }
        int Price { get; }
        CoinType CoinType { get; }
    }
}
