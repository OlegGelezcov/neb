using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Common;

namespace Nebula.Server.Components {
    public class DropListComponentData : ComponentData {

        public XElement parentElement { get; private set; }

        public override ComponentID componentID {
            get {
                return ComponentID.DropList;
            }
        }

        public DropListComponentData(XElement element) {
            parentElement = element;
        }


    }
}
