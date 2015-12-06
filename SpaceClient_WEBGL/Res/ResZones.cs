namespace Nebula.Client.Res {
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    public class ResZones
    {
        private Dictionary<string, ResZoneInfo> zones;

        public ResZones()
        {
            this.zones = new Dictionary<string, ResZoneInfo>();
        }

        public void Load(string xml)
        {
            XDocument document = XDocument.Parse(xml);
            this.zones = document.Element("zones").Elements("zone").Select(e =>
                {
                    string id = e.Attribute("id").Value;
                    string scene = e.Attribute("scene").Value;
                    string displayName = e.Attribute("display_name").Value;
                    return new ResZoneInfo(id, scene, displayName);
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
    }
}
