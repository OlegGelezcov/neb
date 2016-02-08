using System.Xml.Linq;

namespace Nebula.Contracts {
    /// <summary>
    /// Location data for generation explore location contract
    /// </summary>
    public class LocationData : ElementBaseData {
        public LocationData(XElement element)
            : base(element) { }
    }
}
