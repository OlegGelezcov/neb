using Common;
using System.Xml.Linq;

namespace Nebula.Server.Components {
    public abstract class DropListComponentData : MultiComponentData {

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
