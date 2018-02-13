namespace Space.Game.Ship
{
    using System;
    using System.Xml.Linq;
    using System.Collections.Generic;
    using System.Linq;
    using Common;
    using GameMath;
    using Space.Game.Resources;
    using System.IO;

    public class ModuleInfoStorage : IResourceLoader
    {
        private Dictionary<Workshop, List<ModuleInfo>> modules;
        private bool loaded;


        public bool Load(string basePath) {
            string fullPath = Path.Combine(basePath, "Data/Drop/modules.xml");
            XDocument document = XDocument.Load(fullPath);
            this.modules = new Dictionary<Workshop, List<ModuleInfo>>();


            var dump = document.Element("modules").Elements("module").Select(me => {

                Workshop workshop = (Workshop)Enum.Parse(typeof(Workshop), me.GetString("workshop"));
                List<string> sets = new List<string>();
                string[] setIDs = me.GetString("set").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach(string s in setIDs ) {
                    sets.Add(s.Trim());
                }

                ModuleInfo module = new ModuleInfo {
                    Id = me.GetString("id"),
                    Name = me.GetString("name"),
                    Model = me.GetString("model"),
                    allowedSets = sets,
                    Type = (ShipModelSlotType)Enum.Parse(typeof(ShipModelSlotType), me.GetString("type")),
                    Workshop = workshop,
                };
                if (!this.modules.ContainsKey(workshop)) {
                    this.modules.Add(workshop, new List<ModuleInfo>());
                }
                this.modules[workshop].Add(module);
                return module;
            }).ToList();
            this.loaded = true;
            return this.loaded;
        }
        

        public Dictionary<Workshop, List<ModuleInfo>> Modules
        {
            get
            {
                return this.modules;
            }
        }

        public ModuleInfo Module(string id)
        {
            foreach (var kv in this.modules) {
                foreach (var m in kv.Value) {
                    if (m.Id == id) {
                        return m;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Return list of modules with given workshop which has any sets
        /// </summary>
        public List<ModuleInfo> ModulesWithSet(Workshop workshop) {
            List<ModuleInfo> filteredModules = null;
            if(modules.TryGetValue(workshop, out filteredModules)) {
                return filteredModules.Where(m => m.hasSets).ToList();
            }
            return new List<ModuleInfo>();
        }

        public List<ModuleInfo> ModulesWithSet(Workshop workshop, ShipModelSlotType type) {
            List<ModuleInfo> filteredModules = null;
            if (modules.TryGetValue(workshop, out filteredModules)) {
                return filteredModules.Where(m => m.hasSets && m.Type == type).ToList();
            }
            return new List<ModuleInfo>();
        }


        public ModuleInfo Module(Workshop workshop, ShipModelSlotType type, string id )
        {
            List<ModuleInfo> filteredModules = null;
            if (this.modules.TryGetValue(workshop, out filteredModules)) {
                return filteredModules.Where(m => m.Id == id && m.Type == type).FirstOrDefault();
            }
            return null;
        }

        public ModuleInfo Module(Workshop workshop, ShipModelSlotType type) {
            List<ModuleInfo> filteredModules = null;
            if (this.modules.TryGetValue(workshop, out filteredModules)) {
                List<ModuleInfo> modules = filteredModules.Where(m => m.Type == type).ToList();
                if(modules.Count > 0 ) {
                    return modules.AnyElement();
                }
            }
            return null;
        }

        public ModuleInfo RandomModule(Workshop workshop)
        {
            List<ModuleInfo> filteredModules = null;
            if (this.modules.TryGetValue(workshop, out filteredModules)) {
                return filteredModules[Rand.Int(filteredModules.Count - 1)];
            }
            return null;
        }

        public ModuleInfo RandomModule(Workshop workshop, ShipModelSlotType type)
        {
            List<ModuleInfo> filteredModules = null;
            if (this.modules.TryGetValue(workshop, out filteredModules)) {
                List<ModuleInfo> typedModules = filteredModules.Where(m => m.Type == type).ToList();
                if (typedModules.Count > 0) {
                    return typedModules[Rand.Int(typedModules.Count - 1)];
                }
            }
            return null;
        }



        public bool Loaded {
            get { return this.loaded; }
        }
    }
}
