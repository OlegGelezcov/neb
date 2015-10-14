using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Common;
using GameMath;

namespace Space.Game.Resources
{
    public class MaterialRes
    {
        public List<OreData> Ores { get; private set; }

        public void Load(string basePath)
        {
            string fullPath = Path.Combine(basePath, "Data/Materials/ore.xml");
            XDocument document = XDocument.Load(fullPath);
            this.Ores = document.Element("materials").Elements("material").Select(e => 
            {
                return new OreData { Id = e.Attribute("id").Value, Name = e.Attribute("name").Value };
            }).ToList();
        }

        public OreData Ore(string id )
        {
            return this.Ores.Where(o => o.Id == id).FirstOrDefault();
        }

        public OreData RandomOre()
        {
            if (this.Ores != null && this.Ores.Count > 0)
            {
                int index = Rand.Int(this.Ores.Count - 1);
                return this.Ores[index];
            }
            else
            {
                return OreData.Empty;
            }
        }

        public OreData RandomOreExcept(List<string> exceptIds)
        {
            var resultOres = this.Ores.Where(o => exceptIds.Contains(o.Id) == false).ToList();

            if (resultOres.Count == 0)
                return null;

            int randomIndex = Rand.Int(resultOres.Count - 1);
            return resultOres[randomIndex];
        }

        public Dictionary<string, int> GenOres(int numEntries, int eachMaterialCount)
        {
            List<string> alreadyGeneratedOres = new List<string>();
            
            Dictionary<string, int> result = new Dictionary<string,int>();
            
            for(int i = 0; i < numEntries; i++ )
            {
                OreData data = this.RandomOreExcept(alreadyGeneratedOres);
                if (data == null)
                    break;
                result.Add(data.Id, eachMaterialCount);
                alreadyGeneratedOres.Add(data.Id);
            }

            return result;
        }


    }
}
