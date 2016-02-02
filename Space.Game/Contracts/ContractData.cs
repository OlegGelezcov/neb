using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Nebula.Contracts {
    public abstract class ContractData {
        public string id { get; private set; }
        public ContractCategory category { get; private set; }
        public int minLevel { get; private set; }

        public ContractData(XElement element ) {
            id = element.GetString("id");
            category = (ContractCategory)Enum.Parse(typeof(ContractCategory), element.GetString("category"));
            minLevel = element.GetInt("min_level");
        }
    }
}
