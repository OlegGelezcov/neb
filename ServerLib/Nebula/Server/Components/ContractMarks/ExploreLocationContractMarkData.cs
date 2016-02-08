using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Nebula.Server.Components {
    public class ExploreLocationContractMarkData : ContractMarkData {
        public string locationName { get; private set; }

        public ExploreLocationContractMarkData(XElement element ) : base(element) {
            locationName = element.GetString("location_name");
        }
        public override ComponentSubType subType {
            get {
                return ComponentSubType.explore_location_contract;
            }
        }
    }
}
