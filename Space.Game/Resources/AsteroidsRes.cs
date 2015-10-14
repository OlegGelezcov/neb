using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using Common;


namespace Space.Game.Resources
{
    public class AsteroidsRes
    {
        public List<AsteroidData> Asteroids { get; private set; }

        public void Load(string basePath )
        {
            string fullPath = Path.Combine(basePath, "Data/asteroids.xml");
            XDocument document = XDocument.Load(fullPath);
            this.Asteroids = document.Element("asteroids").Elements("asteroid").Select(e =>
            {
                var materials = e.Element("materials").Elements("material").Select(me =>
                {
                    return new AsteroidMaterialData
                    {
                        Id = me.Attribute("id").Value,
                        Type = (MaterialType)System.Enum.Parse(typeof(MaterialType), me.Attribute("type").Value),
                        MaxCount = int.Parse(me.Attribute("max_count").Value),
                        Prob = me.GetFloat("prob")
                    };
                }).ToList();
                return new AsteroidData
                {
                    ContentData = materials,
                    Id = e.Attribute("id").Value,
                    Name = e.Attribute("name").Value,
                    Quality = int.Parse(e.Attribute("quality").Value)
                };
            }).ToList();
        }

        /// <summary>
        /// Return asteroid data by id
        /// </summary>
        public AsteroidData Data(string asteroidDataId )
        {
            return this.Asteroids.Where(a => a.Id == asteroidDataId).FirstOrDefault();
        }
    }
}
