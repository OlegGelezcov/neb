using System.Xml.Linq;

namespace Nebula.Server.Components {
    public class NormalDropListComponentData : DropListComponentData {

        public NormalDropListComponentData(XElement element)
            : base(element) { }


        public override ComponentSubType subType {
            get {
                return ComponentSubType.normal_drop_list;
            }
        }
    }
}
