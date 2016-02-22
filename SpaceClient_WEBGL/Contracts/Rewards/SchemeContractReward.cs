using Common;
using System;

namespace Nebula.Client.Contracts {
    public class SchemeContractReward : ContractReward {
        public ObjectColor color { get; private set; }
        public SchemeContractReward(UniXMLElement element) 
            : base(element) {
            color = (ObjectColor)Enum.Parse(typeof(ObjectColor), element.element.GetString("color"));
        }
    }
}
