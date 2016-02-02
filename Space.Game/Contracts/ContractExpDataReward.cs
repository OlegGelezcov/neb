using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Nebula.Contracts {
    public class ContractExpDataReward : ContractDataReward {

        public int count { get; private set; }

        public ContractExpDataReward(XElement element)
            : base(element) {
            count = element.GetInt("count");
        }
    }
}
