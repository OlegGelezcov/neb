using Common;
using System;
using System.Xml.Linq;

namespace Nebula.Contracts {
    public class ContractWeaponDataReward : ContractDataReward {

        public ObjectColor color { get; private set; }

        public ContractWeaponDataReward(XElement element)
            : base(element) {
            color = (ObjectColor)Enum.Parse(typeof(ObjectColor), element.GetString("color"));
        }


    }
}
