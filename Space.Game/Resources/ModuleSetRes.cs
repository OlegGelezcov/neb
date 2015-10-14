using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using Common;
using System.Globalization;

namespace Space.Game.Resources
{
    public class ModuleSetRes : IResourceLoader
    {
        private bool loaded;
        private List<ModuleSetData> moduleSets;

        public int ModuleSetsCount()
        {
            return this.moduleSets.Count;
        }

        public bool Load(string basePath)
        {
            string fullPath = Path.Combine(basePath, "Data/Drop/module_set.xml");
            XDocument document = XDocument.Load(fullPath);

            this.moduleSets = document.Element("sets").Elements("set").Select(setElement =>
            {
                string id = setElement.Attribute("id").Value;
                string name = setElement.Attribute("name").Value;
                int unlockLevel = setElement.Attribute("unlock_level").ToInt();
                float dropProb = setElement.Attribute("drop_prob").ToFloat();
                bool isDefault = setElement.GetBool("is_default");
                int skill = int.Parse(setElement.Attribute("skill").Value, NumberStyles.HexNumber);
                Workshop workshop = (Workshop)Enum.Parse(typeof(Workshop), setElement.GetString("workshop"));

                return new ModuleSetData(id, name, unlockLevel, dropProb, isDefault, skill, workshop);
            }).ToList();
            this.loaded = true;

            return this.loaded;
        }

        public bool Loaded
        {
            get { return this.loaded; }
        }



        public ModuleSetData Set(string setID )
        {
            return this.moduleSets.Where(s => s.Id == setID).FirstOrDefault();
        }

        public List<ModuleSetData> WorkshopSets(Workshop w, int level) {
            return moduleSets.Where(s => s.Workshop == w && s.UnlockLevel <= level).ToList();
        }
    }
}
