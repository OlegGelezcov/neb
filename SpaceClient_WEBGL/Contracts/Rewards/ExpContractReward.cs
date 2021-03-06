﻿using Common;
#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
using Nebula.Client.UP;
#endif

namespace Nebula.Client.Contracts {
    public class ExpContractReward : ContractReward  {
        public int count { get; private set; }

#if UP
        public ExpContractReward(UPXElement element) 
            : base(element) {
            count = element.GetInt("count");
        }
#else
        public ExpContractReward(XElement element) 
            : base(element) {
            count = element.GetInt("count");
        }
#endif
    }
}
