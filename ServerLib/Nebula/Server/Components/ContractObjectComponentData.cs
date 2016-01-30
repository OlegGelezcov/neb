using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using System.Xml.Linq;

namespace Nebula.Server.Components {
    public class ContractObjectComponentData : ComponentData {

        public string contractId { get; private set; }
        public List<EventType> eventTypes { get; private set; }

        public override ComponentID componentID {
            get {
                return ComponentID.ContractObject;
            }
        }

        public ContractObjectComponentData(XElement element) {
            contractId = element.GetString("contract_id");
            string eventsStr = element.GetString("events");
            eventTypes = new List<EventType>();
            string[] eventStrArr = eventsStr.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach(string sEvent in eventStrArr) {
                EventType eType = (EventType)Enum.Parse(typeof(EventType), sEvent);
                if(eType != EventType.None) {
                    eventTypes.Add(eType);
                }
            }
        }
    }
}
