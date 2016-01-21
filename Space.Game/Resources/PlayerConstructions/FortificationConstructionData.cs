using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Nebula.Resources.PlayerConstructions {
    public class FortificationConstructionData : AttackerDefensiveConstructionData {
        public FortificationConstructionData(XElement element)
            : base(element) { }
    }

}
