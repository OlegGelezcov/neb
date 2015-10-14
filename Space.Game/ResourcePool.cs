using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;

namespace Space.Game
{
    public class ResourcePool
    {
        private string baseResourcePath;
        private Dictionary<string, Res> resources;
        private List<string> zoneIds;


        public ResourcePool(string baseResourcePath)
        {
            this.baseResourcePath = baseResourcePath;
            this.resources = new Dictionary<string, Res>();
            this.LoadZoneIds();
        }

        private void LoadZoneIds()
        {
            string directoryPath = Path.Combine(this.baseResourcePath, "Data/Zones");
            zoneIds = new List<string>();
            string[] files = Directory.GetFiles(directoryPath, "*.xml", SearchOption.AllDirectories);
            foreach (string file in files) {
                try {
                    zoneIds.AddRange(LoadZoneIdsFromFile(file));
                } catch (Exception exception) {
                    Res.logger.Log(exception.Message);
                    Res.logger.Log(exception.StackTrace);
                }
            }
        }

        private List<string> LoadZoneIdsFromFile(string file ) {
            XDocument document = XDocument.Load(file);
            return document.Element("zones").Elements("zone").Select(e => e.Attribute("id").Value).ToList();
        }

        public Res Resource(string id)
        {
            

            if(!this.resources.ContainsKey(id))
            {
                Res resource = new Res(this.baseResourcePath);
                resource.Load();

                this.resources.Add(id, resource);
            }
            return this.resources[id];
        }

        public List<string> ZoneIds()
        {
            return this.zoneIds;
        }
    }
}
