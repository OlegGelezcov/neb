using Common;
using System;
using System.Xml.Linq;

namespace Nebula.Contracts {
    public class ContractSchemeDataReward : ContractDataReward {
        public ObjectColor color { get; private set; }
        public ContractSchemeDataReward(XElement element)
            : base(element) {
            color = (ObjectColor)Enum.Parse(typeof(ObjectColor), element.GetString("color"));
        }
    }
}
