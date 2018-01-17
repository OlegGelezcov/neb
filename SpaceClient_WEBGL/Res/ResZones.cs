namespace Nebula.Client.Res {
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
#if UP
    using Nebula.Client.UP;
#else
    using System.Xml.Linq;
#endif

    public class ResZones
    {
        private Dictionary<string, ResZoneInfo> zones;

        public ResZones()
        {
            this.zones = new Dictionary<string, ResZoneInfo>();
        }

        public void Load(string xml)
        {
#if UP
            UPXDocument document = new UPXDocument(xml);
#else
            XDocument document = XDocument.Parse(xml);
#endif
            this.zones = document.Element("zones").Elements("zone").Select(e =>
                {
                    string id = e.Attribute("id").Value;
                    string scene = e.Attribute("scene").Value;
                    string displayName = e.Attribute("display_name").Value;
                    bool isAvailable = e.GetBool("available", false);
                    return new ResZoneInfo(id, scene, displayName, isAvailable);
                }).ToDictionary(z => z.Id(), z => z);
            
        }

        
        public ResZoneInfo Zone(string id)
        {
            if (this.zones.ContainsKey(id))
                return this.zones[id];
            return ResZoneInfo.Null();
        }

        public ResZoneInfo ZoneForScene(string scene)
        {
            foreach (var pZone in this.zones)
                if (pZone.Value.Scene() == scene)
                    return pZone.Value;
            return ResZoneInfo.Null();
        }

        public Dictionary<string, ResZoneInfo> Zones {
            get {
                return zones;
            }
        }

        public List<ResZoneInfo> ZoneList {
            get {
                return new List<ResZoneInfo>(Zones.Values);
            }
        }

        public override string ToString() {
            var zoneList = ZoneList;
            string summary = string.Format("Total => {0}, Available Count => {1}", zoneList.Count, zoneList.Count(z => z.IsAvailable));
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(summary);
            builder.AppendLine("---------------------");
            foreach(var zone in ZoneList ) {
                builder.AppendLine(zone.ToString());
            }
            return builder.ToString();
        }
    }
}
