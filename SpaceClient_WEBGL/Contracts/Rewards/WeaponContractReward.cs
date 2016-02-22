using Common;

using System;
#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
using Nebula.Client.UP;
#endif
namespace Nebula.Client.Contracts {
    public class WeaponContractReward : ContractReward {
        public ObjectColor color { get; private set; }

#if UP
        public WeaponContractReward(UPXElement element) 
            : base(element) {
            color = (ObjectColor)Enum.Parse(typeof(ObjectColor), element.GetString("color"));
        }
#else
        public WeaponContractReward(XElement element) 
            : base(element) {
            color = (ObjectColor)Enum.Parse(typeof(ObjectColor), element.GetString("color"));
        }
#endif
    }
}
